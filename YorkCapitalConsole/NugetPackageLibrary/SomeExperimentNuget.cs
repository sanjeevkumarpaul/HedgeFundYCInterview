using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetPackageLibrary
{
    // https://blogs.msdn.microsoft.com/mvpawardprogram/2016/06/28/creating-nuget-packages/
    //Steps
    //0. Open Command Promt, down the Nuget.exe from the above link and perform below actions.
    //1. nuget spec   - This will create a file with FolderName -
    //2. nuget pack NugetPackageLibrary.nuspec
    //3. You can upload at - https://www.nuget.org/packages/upload
    public class SomeExperimentNuget
    {
        public string HaveIt()
        {
            return "HaveIt";
        }
    }
}
