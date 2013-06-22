using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static IAzureMobileServices Create(string url, string token)
        {
            var assembly = System.Reflection.Assembly.Load(new AssemblyName("Bitrave.Azure"));
            Type t = assembly.GetType("AzureMobileServices");
            var azure = (IAzureMobileServices)Activator.CreateInstance(t);

            azure.Initialise(url, token);
            return azure;
        }
    }

    // THese are copied from their Azure counterparts to give the same structure

    // Summary:
    //     Authentication providers supported by Mobile Services.
    public enum AuthenticationProvider
    {
        // Summary:
        //     Microsoft Account authentication provider.
        MicrosoftAccount = 0,
        //
        // Summary:
        //     Google authentication provider.
        Google = 1,
        //
        // Summary:
        //     Twitter authentication provider.
        Twitter = 2,
        //
        // Summary:
        //     Facebook authentication provider.
        Facebook = 3,
    }


    public enum AzureResponseStatus
    {
        Success,
        Failure
    }
    public class AzureResponse<T>
    {
        public AzureResponse(AzureResponseStatus status, T responseData)
        {
            Status = status;
            ResponseData = responseData;
        }
        public AzureResponse(Exception e)
        {
            Error = e;
            Status = AzureResponseStatus.Failure;
        }
        public AzureResponseStatus Status { get; private set; }
        public Exception Error { get; private set; }
        public T ResponseData { get; internal set; }
    }
    
    // A copy of the structure so we can use it from Unity
    public class MobileServiceUser
    {
        public MobileServiceUser(string userId)
        {
            UserId = userId;
        }
        public string MobileServiceAuthenticationToken { get; set; }
        public string UserId { get; set; }
    }
}
