using CommonLayer.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using RepositoryLayer.Authorisation;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Model;
using UserServices.Authorisation;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly CosmosClient _cosmosClient;
        private readonly ITokenServices jwt;

        public UserRL(CosmosClient _cosmosClient, ITokenServices jwt)
        {
            this._cosmosClient = _cosmosClient;
            this.jwt = jwt;
        }
        public async Task<UserDetails> createUser(UserDetails details)
        {
            if(details == null)
            {
                throw new NullReferenceException();
            }
            try
            {
                    var user = new UserDetails()
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = details.FirstName,
                        LastName = details.LastName,
                        Email = details.Email,
                        Password = details.Password

                    };

                    var container = this._cosmosClient.GetContainer("FundooNotesDb", "UserDetails");
                    return await container.CreateItemAsync<UserDetails>(user, new PartitionKey(user.Id));
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public string ForgetPassword(ForgetPasswordDetails details)
        {
            try
            {

                var option = new FeedOptions { EnableCrossPartitionQuery = true };
                var container = this._cosmosClient.GetContainer("FundooNotesDb", "UserDetails");
                var document = container.GetItemLinqQueryable<UserDetails>(true)
                               .Where(b => b.Email == details.Email )
                               .AsEnumerable()
                               .FirstOrDefault();
                if (document != null)
                {
                    var token = this.jwt.GetToken(document.Id, document.Email);
                    //new MsMq().Sender(token);
                    return token;
                }
                return string.Empty;
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);

            }

        }

        public async Task<UserDetails> ResetPassword(ResetPassWordDetails details)
        {
            if(details == null)
            {
                throw new NullReferenceException();
            }
            try
            {


                if (details.Password.Equals(details.ConfirmPassword))
                {
                    var container = this._cosmosClient.GetContainer("FundooNotesDb", "UserDetails");
                    var document = container.GetItemLinqQueryable<UserDetails>(true)
                                   .Where(b => b.Email == details.Email)
                                   .AsEnumerable()
                                   .FirstOrDefault();
                    if (document != null)
                    {
                        //UserDetails user = new UserDetails();
                        //user.Id = document.Id;
                        //user.Password = details.Password;
                        //return await container.UpsertItemAsync<UserDetails>(user, new PartitionKey(document.Id));

                        ItemResponse<UserDetails> response = await container.ReadItemAsync<UserDetails>(document.Id, new PartitionKey(document.Id));
                        var itembody = response.Resource;
                        itembody.Password = details.Password;
                        itembody.CreatedAt = DateTime.Now;
                        response = await container.ReplaceItemAsync<UserDetails>(itembody, itembody.Id, new PartitionKey(itembody.Id));
                        return response.Resource;
                        
                    }
                    

                }
                throw new NullReferenceException();
            }
            catch(CosmosException ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public LoginCredentials UserLogin(LoginDetails details)
        {
            LoginCredentials login = new LoginCredentials();

            try
            {
                var option = new FeedOptions { EnableCrossPartitionQuery = true };

                var container = this._cosmosClient.GetContainer("FundooNotesDb", "UserDetails");
                var document =  container.GetItemLinqQueryable<UserDetails>(true)
                               .Where(b => b.Email == details.Email && b.Password == details.Password)
                               .AsEnumerable()
                               .FirstOrDefault();
                
                
                if (document != null)
                {
                    login.userDetails = document;
                    login.token = this.jwt.GetToken(login.userDetails.Id, login.userDetails.Email);
                    return login;
                        
                }
                return login;

                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public UserDetails GetDetailsById(string Id)
        {
            UserDetails user = new UserDetails();
            try
            {
                var option = new FeedOptions { EnableCrossPartitionQuery = true };

                var container = this._cosmosClient.GetContainer("FundooNotesDb", "UserDetails");
                var document = container.GetItemLinqQueryable<UserDetails>(true)
                               .Where(b => b.Id == Id)
                               .AsEnumerable()
                               .FirstOrDefault();
                if(document != null)
                {
                    user.Id = document.Id;
                    user.FirstName = document.FirstName;
                    user.LastName = document.LastName;
                    user.Email = document.Email;
                    user.Password = document.Password;
                    user.CreatedAt = document.CreatedAt;
                    user.ModifiedAt = document.ModifiedAt;

                }
                return user;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public UserDetails GetDetailsByEmailId(string email)
        {
            UserDetails user = new UserDetails();
            try
            {
                var option = new FeedOptions { EnableCrossPartitionQuery = true };

                var container = this._cosmosClient.GetContainer("FundooNotesDb", "UserDetails");
                var document = container.GetItemLinqQueryable<UserDetails>(true)
                               .Where(b => b.Email == email)
                               .AsEnumerable()
                               .FirstOrDefault();
                if (document != null)
                {
                    user.Id = document.Id;
                    user.FirstName = document.FirstName;
                    user.LastName = document.LastName;
                    user.Email = document.Email;
                    user.Password = document.Password;
                    user.CreatedAt = document.CreatedAt;
                    user.ModifiedAt = document.ModifiedAt;

                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
