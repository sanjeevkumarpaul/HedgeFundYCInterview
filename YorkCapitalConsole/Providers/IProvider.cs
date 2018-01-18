using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers
{
    public interface IProvider<Instance> where Instance : class
    {
        Instance CreateInstance();
        Instance Load(object dynvalues);
    }
}
