using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Tools.WampReader
{
    public class ClientAuthenticator : IWampClientAuthenticator
    {
        private readonly string _authMethod;
        private readonly string _signature;

        public ClientAuthenticator(string authMethod, string authId, string signature)
        {
            _authMethod = authMethod;
            _signature = signature;
            AuthenticationId = authId;
        }

        public AuthenticationResponse Authenticate(string authMethod, ChallengeDetails extra)
        {
            AuthenticationResponse result = new AuthenticationResponse {Signature = _signature};
            return result;
        }

        public string[] AuthenticationMethods => new[] {_authMethod};
        public string AuthenticationId { get; }
    }
}
