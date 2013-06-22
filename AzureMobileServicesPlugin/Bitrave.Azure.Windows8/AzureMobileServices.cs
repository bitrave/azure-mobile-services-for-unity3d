using System.Windows;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Windows.ApplicationModel.Core;

namespace Bitrave.Azure
{
    public class AzureMobileServices
    {
        public AzureMobileServices() { }
        private MobileServiceClient _mobileClient;

        // On error is for methods where there is no generic callback
        // for example: Insert, Update, Delete
        public Action<AzureResponse<object>> OnError;

        // "https://bitraveservices.azure-mobile.net/", "ePYhutMVmiUPWhFAJaRYTJsPFTiuAB20"
        public AzureMobileServices(string url, string token)
        {
            try
            {
                _mobileClient = new MobileServiceClient(url, token);
            }
            catch (Exception e)
            {
                var response = new AzureResponse<object>(e);
                if (OnError != null) OnError(response);
            }
        }

        public bool Initialised()
        {
            if (_mobileClient == null) return false;
            return true;
        }


        public async void LoginAsync(AuthenticationProvider provider, Action<AzureResponse<Bitrave.Azure.MobileServiceUser>> callback )
        {
#if !WINDOWS_PHONE 
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
#else
         //   Deployment.Current.Dispatcher.BeginInvoke( async () => 
#endif
            // There is an issue with Unity building out the WP8 
            // version using the MobileServices UI DLL
            // so removing this for build reasons.
#if !WINDOWS_PHONE
                        {
                try
                {
                    var authProvider = (MobileServiceAuthenticationProvider)provider;
                   
                    var user = await _mobileClient.LoginAsync(authProvider);
                    var unityUser = new Bitrave.Azure.MobileServiceUser(user.UserId)
                    {
                        MobileServiceAuthenticationToken = user.MobileServiceAuthenticationToken
                    };
                    
                    var response = new AzureResponse<Bitrave.Azure.MobileServiceUser>(AzureResponseStatus.Success, unityUser);
                    callback(response);
                }
                catch (InvalidOperationException e)
                {
                    var response = new AzureResponse<MobileServiceUser>(e);
                    callback(response);
                }
            });
#endif 
        }

        public async void Insert<T>(T item) where T : class 
        {
            try
            {
                await _mobileClient.GetTable<T>().InsertAsync(item);
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR:" + e);
                var response = new AzureResponse<object>(e);
                if (OnError != null) OnError(response);
            }
        }

        public async void Update<T>(T item) where T : class 
        {
            try
            {
                await _mobileClient.GetTable<T>().UpdateAsync(item);
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR:" + e);
                var response = new AzureResponse<object>(e);
                if (OnError != null) OnError(response);
            }
        }

        public async void Delete<T>(T item) where T : class 
        {
            try
            {
                await _mobileClient.GetTable<T>().DeleteAsync(item);
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR:" + e);
                var response = new AzureResponse<object>(e);
                if (OnError != null) OnError(response);
            }
        }

        public async void Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Action<AzureResponse<List<T>>> callback) where T : class 
        {
            try
            {
                var list = await _mobileClient.GetTable<T>().Where(predicate).ToListAsync();
                var response = new AzureResponse<List<T>>(AzureResponseStatus.Success, list);
                callback(response);
            }
            catch (Exception e)
            {
                var response = new AzureResponse<List<T>>(e);
                callback(response);
            }
        }

        public async void Lookup<T>(int id, Action<AzureResponse<T>> callback) where T : class 
        {
            try
            {
                var value = await _mobileClient.GetTable<T>().LookupAsync(id);
                var response = new AzureResponse<T>(AzureResponseStatus.Success, value);
                callback(response);
            }
            catch (Exception e)
            {
                var response = new AzureResponse<T>(e);
                callback(response);
            }
        }

        public async void Read<T>(Action<AzureResponse<List<T>>> callback) where T : class 
        {
            try
            {
                var value = await _mobileClient.GetTable<T>().ReadAsync();
                var response = new AzureResponse<List<T>>(AzureResponseStatus.Success, value.ToList());
                callback(response);
            }
            catch (Exception e)
            {
                var response = new AzureResponse<List<T>>(e);
                callback(response);
            }
        }


    }
}
