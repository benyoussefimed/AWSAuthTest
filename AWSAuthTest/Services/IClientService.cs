using System.Collections.Generic;
using System.Threading.Tasks;
using AWSAuthTest.Models;

namespace AWSAuthTest.Services
{
    public interface IClientService
    {
        Task<Client> Authorisation(string accessKeyId, string secretAccessKey);
        Task<IEnumerable<Client>> GetAll();
    }
}