using APICalls.Enum;

namespace APICalls.Entities.Contracts
{
    public sealed class APIAuthorization
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public bool IsTokenAHeader { get; set; }
        public APIAuthenticationType Type { get; set; }
    }
}
