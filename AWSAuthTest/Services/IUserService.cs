using AWSAuthTest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AWSAuthTest.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
    }
}
