using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Model;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        Task<UserDetails> createUser(UserDetails details);
    }
}
