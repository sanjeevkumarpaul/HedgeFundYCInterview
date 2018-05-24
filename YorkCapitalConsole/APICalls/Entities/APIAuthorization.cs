using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
{
    public class APIAuthorization
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public bool IsTokenAHeader { get; set; }
        public APIAuthenticationType Type { get; set; }
    }
}
