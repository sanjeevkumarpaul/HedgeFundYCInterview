using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PostSharp.Aspects;
using PostSharp.Serialization;

namespace Postsharp.AspectEntities
{
    [PSerializable]
    public class MethodInjectionAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Console.WriteLine("On Entry of Method.");
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Console.WriteLine("I am here - After return of the method.");
        }
    }
}
