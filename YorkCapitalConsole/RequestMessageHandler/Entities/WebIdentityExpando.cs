﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wrappers;

namespace RequestMessageHandler.Entities
{
    internal class WebIdentityExpando : WrapExpandos
    {
        public string Expansions { get; set; }
    }
}
