using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using UnityEngine;

namespace Bitrave.Azure
{
    public class AzureMobileServices : IAzureMobileServices
    {
        private string _azureEndPoint;
        private string _applicationKey;

        public Action<AzureResponse<object>> OnError;

        public AzureMobileServices(string url, string token)
        {
            _azureEndPoint = url;
            _applicationKey = token;
        }

        public bool Initialised()
        {
            return _azureEndPoint != null && _applicationKey != null;
        }
        
        public void Insert<T>(T item) where T : class
        {
            var type = typeof(T);
            var ms = new MobileServiceRequestHelper<T>(_azureEndPoint, _applicationKey);
            type.GetProperty("Id").SetValue(item, null, null);

            ms.Post(item);
        }

        public void Update<T>(T item) where T : class
        {
            var type = typeof (T);
            var ms = new MobileServiceRequestHelper<T>(_azureEndPoint, _applicationKey);
            
            var id = (int)type.GetProperty("Id").GetValue(item, null);
            type.GetProperty("Id").SetValue(item, null, null);

            ms.Put(item, id);
            type.GetProperty("Id").SetValue(item, id, null);
        }

        public void LoginAsync(AuthenticationProvider provider, Action<AzureResponse<MobileServiceUser>> callback)
        {
            Debug.Log("Azure Mobile Services authentication is not supported in Unity editor.");
            var response = new AzureResponse<MobileServiceUser>(AzureResponseStatus.Success,
                                                                new MobileServiceUser("1234_USER_ID")
                                                                    {
                                                                        MobileServiceAuthenticationToken = "ABCD_TOKEN"
                                                                    });
            callback(response);
        }
        

        public void Delete<T>(T item) where T : class
        {
            var type = typeof(T);
            var ms = new MobileServiceRequestHelper<T>(_azureEndPoint, _applicationKey);
            var id = (int)type.GetProperty("Id").GetValue(item, null);

            ms.Delete(id);
        }

        /// <summary>
        /// Returns a filtered list.  NOTE: The predicate runs client side in
        /// the Unity Editor version, so you won't get the same behaviours.  It works server side on native deployments.  
        /// It retrieves the maximum result page size (usually first 50 entries) and then filters it.  
        /// </summary>
        public void Where<T>(Expression<Func<T, bool>> predicate, Action<AzureResponse<List<T>>> callback) where T : class
        {
            var ms = new MobileServiceRequestHelper<T>(_azureEndPoint, _applicationKey);
            var result = ms.Query(predicate.Compile());
            callback(result);
        }

        public void Lookup<T>(int id, Action<AzureResponse<T>> callback) where T : class
        {
            var ms = new MobileServiceRequestHelper<T>(_azureEndPoint, _applicationKey);
            var result = ms.Get(id);
            callback(result);
        }

        public void Read<T>(Action<AzureResponse<List<T>>> callback) where T : class
        {
            var ms = new MobileServiceRequestHelper<T>(_azureEndPoint, _applicationKey);
            var result = ms.Get();
            callback(result);
        }


       
    }


}
