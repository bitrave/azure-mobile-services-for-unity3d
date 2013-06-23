using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitrave.Azure
{
    /// <summary>
    /// Why does this factory exist?  To get access to the azure mobile services, and to 
    /// get around dependency pain in Unity.  The factory uses reflection to remove the dependencies
    /// on some Azure libraries that cause Unity builds to fail for Windows Phone.
    /// </summary>
    public class AzureMobileServicesFactory
    {
        const string TypeName = "Bitrave.Azure.AzureMobileServices";

        public static IAzureMobileServices Create(string url, string token)
        {
            IAzureMobileServices azure = null;

#if WINDOWS_PHONE || UNITY_EDITOR
            var type = Type.GetType(TypeName);
            azure = Activator.CreateInstance(type, new object[] { url, token }) as AzureMobileServices;
#endif

#if METRO
            // foo
#endif
            return azure;
        }
    }
}
