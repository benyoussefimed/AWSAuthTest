using AWSAuthTest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSAuthTest.Services
{
    public class ClientService : IClientService
    {
        // client hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<Client> _clients = new List<Client>
        {
             new Client { ClientId = 1, AccessKeyId = "AKIAIOSFODNN7EXAMPLE", SecretAccessKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY" }
        };

        public async Task<Client> Authorisation(string accessKeyId, string secretAccessKey)
        {
            var client = await Task.Run(() => _clients.SingleOrDefault(x => x.AccessKeyId == accessKeyId && x.SecretAccessKey == secretAccessKey));

            // return null if user not found
            if (client == null)
                return null;

            // Authorisation successful so return user details without SecretAccessKey
            client.SecretAccessKey = null;
            return client;
        }

        public async Task<IEnumerable<Client>> GetAll()
        {
            // return clients without SecretAccessKey
            return await Task.Run(() => _clients.Select(x => {
                x.SecretAccessKey = null;
                return x;
            }));
        }
    }
}
