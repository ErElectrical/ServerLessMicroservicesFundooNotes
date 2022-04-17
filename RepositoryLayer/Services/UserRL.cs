using Microsoft.Azure.Cosmos;
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

        public UserRL(CosmosClient _cosmosClient)
        {
            this._cosmosClient = _cosmosClient;
        }
        public async Task<UserDetails> createUser(UserDetails details)
        {
            if(details != null)
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
                if(result != null)
                {
                    return user;
                }
                if(result == null)
                {
                    return null;
                }

           
            }
        }
    }
}
