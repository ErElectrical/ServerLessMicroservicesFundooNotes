using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Model;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly CosmosClient _cosmosClient;
        private DocumentClient client;

        public UserRL(CosmosClient _cosmosClient, DocumentClient client)
        {
            this._cosmosClient = _cosmosClient;
            this.client = client;
        }
        public async Task createUser(UserDetails details)
        {
            try
            {


                if (details != null)
                {
                    var user = new UserDetails()
                    {
                        FirstName = details.FirstName,
                        LastName = details.LastName,
                        Email = details.Email,
                        Password = details.Password

                    };

                    var container = this._cosmosClient.GetContainer("FundooNotesDb", "UserDetails");
                    var result = await container.CreateItemAsync(user, new PartitionKey(user.Id.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public bool UserLogin(LoginDetails details)
        {
            try
            {
                var option = new FeedOptions { EnableCrossPartitionQuery = true };

                Uri collectionUri = UriFactory.CreateDocumentCollectionUri("FundooNotesDb", "UserDetails");
                var document = this.client.CreateDocumentQuery<UserDetails>(collectionUri, option).Where(t => t.Email == details.Email && t.Password == details.Password)
                        .AsEnumerable().FirstOrDefault();

                if (document != null)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
