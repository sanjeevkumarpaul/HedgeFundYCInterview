using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extensions;
using System.Reflection;

namespace Providers
{
    public class Provider<Instance> : IProvider<Instance> where Instance : class
    {
        private Instance _providerInstance;
        private IProviderConfiguration _config;

        public Provider(IProviderConfiguration config)
        {
            _config = config;
        }
               
        public Instance CreateInstance()
        {
            try
            {
                if (_providerInstance == null)
                {
                    var type = _config.FullQualifiedTypeName;
                    var assembly = type.RightTill(".");

                    _providerInstance = ((Instance) Activator.CreateInstance(assembly, type).Unwrap());                                       
                }

                return _providerInstance;
            }
            catch (Exception ex)
            {
                string insmsg = "";
                Exception inEx = ex.InnerException;
                while(inEx != null)
                {
                    if (inEx.InnerException != null) insmsg += inEx.InnerException.Message + " / ";
                    inEx = inEx.InnerException;
                }

                throw new ProviderException(insmsg);
            }
            finally
            {
                //TODO: if anything required.
            }           
        }
        
        public Instance Load(object dynvalues)
        {
            var instance = CreateInstance();

            SetInstanceProperties(instance, dynvalues);

            return instance;
        }


        private void SetInstanceProperties(Instance instance, object dynvalues)
        {
            if (dynvalues == null) return;

            var dynType = dynvalues.GetType();
            var insType = instance.GetType();

            foreach (var prop in dynType.GetProperties())
            {
                try
                {
                    PropertyInfo property = insType.GetProperty(prop.Name);
                    if (property != null)
                        property.SetValue(instance, prop.GetValue(dynvalues, null).ToString(), null);
                }
                catch { }
            }
        }
    }
}
