using APICalls.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Example
{
    public class Tokens : IAPIProspect
    {
        public string Token { get; set; }
        public dynamic OtherResponses { get; set; }
    }
}
