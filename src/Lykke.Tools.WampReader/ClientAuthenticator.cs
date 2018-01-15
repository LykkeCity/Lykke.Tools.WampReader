using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Tools.WampReader
{
    public class ClientAuthenticator : IWampClientAuthenticator
    {
        private readonly string _authMethod;

        public ClientAuthenticator(string authMethod, string authId)
        {
            _authMethod = authMethod;
            AuthenticationId = authId;
        }

        public AuthenticationResponse Authenticate(string authmethod, ChallengeDetails extra)
        {
            AuthenticationResponse result = new AuthenticationResponse();
            return result;
        }

        public string[] AuthenticationMethods => new[] {_authMethod};
        public string AuthenticationId { get; }
    }
}
