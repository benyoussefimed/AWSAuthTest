using AWSAuthTest.Models;
using AWSAuthTest.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AWSAuthTest.Helpers
{

    /// <summary>
    /// 
    /// </summary>
    public class AWSAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        private const string AuthorizationHeaderName = "Authorization";
        private const string AWSSchemeName = "AWS";

        private readonly IClientService _clientService;
        private readonly IUserService _userService;

        public AWSAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService,
            IClientService clientService)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
            _clientService = clientService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!AWSSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not AWS authentication header
                return AuthenticateResult.NoResult();
            }
            

            Client client = null;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                var credentials = authHeader.Parameter.Split(':');
                if (credentials.Length != 2)
                {
                    return AuthenticateResult.Fail("Invalid Basic authentication header");
                }
                var accessKeyId = credentials[0];
                var secretAccessKey = credentials[1];
                client = await _clientService.Authorisation(accessKeyId, secretAccessKey);
            }
            catch(Exception ex)
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (client == null)
                return AuthenticateResult.Fail("Invalid accessKeyId or signature");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, client.ClientId.ToString()),
                //new Claim(ClaimTypes.Name, user.Email),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        
    }
}
