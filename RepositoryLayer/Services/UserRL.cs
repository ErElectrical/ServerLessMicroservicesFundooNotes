﻿using CommonLayer.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Authorisation;
using UserRegistration.Model;
using UserServices.Authorisation;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly CosmosClient _cosmosClient;

        public UserRL(CosmosClient _cosmosClient)
        {
            this._cosmosClient = _cosmosClient;
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

        //public string ForgetPassword(ForgetPasswordDetails details)
        //{
        //    GenrateToken auth = new GenrateToken();
        //    try
        //    {

        //        var option = new FeedOptions { EnableCrossPartitionQuery = true };
        //        Uri collectionUri = UriFactory.CreateDocumentCollectionUri("FundooNotesDb", "UserDetails");
        //        var document = this.client.CreateDocumentQuery<UserDetails>(collectionUri, option).Where(t => t.Email == details.Email)
        //                .AsEnumerable().FirstOrDefault();
        //        if (document != null)
        //        {
        //            var token = auth.IssuingToken(document.Id.ToString());
        //            new MsMq().Sender(token);
        //            return token ;
        //        }
        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);

        //    }

        //}    

        public string UserLogin(LoginDetails details)
        {
            GenrateToken auth = new GenrateToken();
            

            try
            {
                var option = new FeedOptions { EnableCrossPartitionQuery = true };

                var container = this._cosmosClient.GetContainer("FundooNotesDb", "UserDetails");
                var document = container.GetItemLinqQueryable<UserDetails>(true)
                               .Where(b => b.Email == details.Email && b.Password == details.Password)
                               .AsEnumerable()
                               .FirstOrDefault();

                if (document != null)
                {
                    var token = auth.IssuingToken(document.Id.ToString());
                    return token;
                }
                return string.Empty;
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
