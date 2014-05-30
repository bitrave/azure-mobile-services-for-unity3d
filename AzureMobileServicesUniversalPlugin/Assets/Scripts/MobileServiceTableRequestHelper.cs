#region License
// Copyright (c) 2014 Bit Rave Pty Ltd
//
// 1. OWNERSHIP, LICENSE GRANT
// Subject to the terms below (the 'License Agreement'), Bit Rave Pty Ltd ('We', 'Us') 
// grants you to install and use Azure Mobile Services for Unity (the 'Software').
//
// This is a license agreement and not an agreement for sale. We reserve ownership 
// of all intellectual property rights inherent in or relating to the Software, 
// which include, but are not limited to, all copyright, patent rights, all rights 
// in relation to registered and unregistered trademarks (including service marks), 
// confidential information (including trade secrets and know-how) and all rights 
// other than those expressly granted by this Agreement.
//
// Subject to the terms and conditions of this License Agreement, We grant to You 
// a non-transferable, non-exclusive license for a Designated User (as defined below) 
// within Your organization to install and use the Software on any workstations used 
// exclusively by such Designated User and for You to distribute the Software as part 
// of your Unity applications or games, solely in connection with distribution of 
// the Software in accordance with sections 3 and 4 below. This license is not 
// sublicensable except as explicitly set forth herein. "Designated User(s)" shall 
// mean Your employee(s) acting within the scope of their employment or Your consultant(s) 
// or contractor(s) acting within the scope of the services they provide for You or on Your behalf.

// 2. PERMITTED USES, SOURCE CODE, MODIFICATIONS
// We provide You with source code so that You can create Modifications of the original Software, 
// where Modification means: a) any addition to or deletion from the contents of a file included 
// in the original Software or previous Modifications created by You, or b) any new file that 
// contains any part of the original Software or previous Modifications. While You retain all 
// rights to any original work authored by You as part of the Modifications, We continue to own 
// all copyright and other intellectual property rights in the Software.

// 3. DISTRIBUTION
// You may distribute the Software in any applications, frameworks, or elements (collectively 
// referred to as "Applications") that you develop using the Software in accordance with this 
// License Agreement, provided that such distribution does not violate the restrictions set 
// forth in section 4 of this agreement.

// You will not owe Us any royalties for Your distribution of the Software in accordance with 
// this License Agreement.

// 4. PROHIBITED USES
// You may not redistribute the Software or Modifications other than by including the Software 
// or a portion thereof within Your own product, which must have substantially different 
// functionality than the Software or Modifications and must not allow any third party to use 
// the Software or Modifications, or any portions thereof, for software development or application 
// development purposes. You are explicitly not allowed to redistribute the Software or 
// Modifications as part of any product that can be described as a development toolkit or library 
// or is intended for use by software developers or application developers and not end-users.

// 5. TERMINATION
// This Agreement shall terminate automatically if you fail to comply with the limitations 
// described in this Agreement. No notice shall be required to effectuate such termination. 
// Upon termination, you must remove and destroy all copies of the Software. 

// 6. DISCLAIMER OF WARRANTY
// YOU AGREE THAT WE HAVE MADE NO EXPRESS WARRANTIES, ORAL OR WRITTEN, TO YOU REGARDING THE 
// SOFTWARE AND THAT THE SOFTWARE IS BEING PROVIDED TO YOU 'AS IS' WITHOUT WARRANTY OF ANY KIND.
//  WE DISCLAIM ANY AND ALL OTHER WARRANTIES, WHETHER EXPRESSED, IMPLIED, OR STATUTORY. YOUR RIGHTS
//  MAY VARY DEPENDING ON THE STATE IN WHICH YOU LIVE. WE SHALL NOT BE LIABLE FOR INDIRECT, 
// INCIDENTAL, SPECIAL, COVER, RELIANCE, OR CONSEQUENTIAL DAMAGES RESULTING FROM THE USE OF THIS PRODUCT.

// 7. LIMITATION OF LIABILITY
// YOU USE THIS PROGRAM SOLELY AT YOUR OWN RISK. IN NO EVENT SHALL WE BE LIABLE TO YOU FOR ANY DAMAGES,
// INCLUDING BUT NOT LIMITED TO ANY LOSS, OR OTHER INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF 
// ANY KIND ARISING OUT OF THE USE OF THE SOFTWARE, EVEN IF WE HAVE BEEN ADVISED OF THE POSSIBILITY OF
// SUCH DAMAGES. IN NO EVENT WILL WE BE LIABLE FOR ANY CLAIM, WHETHER IN CONTRACT, TORT, OR ANY OTHER
// THEORY OF LIABILITY, EXCEED THE COST OF THE SOFTWARE. THIS LIMITATION SHALL APPLY TO CLAIMS OF 
// PERSONAL INJURY TO THE EXTENT PERMITTED BY LAW.

// 8. MISCELLANEOUS
// The license granted herein applies only to the version of the Software available when acquired
// in connection with the terms of this Agreement. Any previous or subsequent license granted to
// You for use of the Software shall be governed by the terms and conditions of the agreement entered
// in connection with the acquisition of that version of the Software. You agree that you will comply
// with all applicable laws and regulations with respect to the Software, including without limitation
// all export and re-export control laws and regulations.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using UnityEngine;
using System.Linq.Expressions;
using Linq2Rest.Provider;

using System.Reflection;
using Bitrave.Azure.Auth;

namespace Bitrave.Azure
{
    internal class MobileServiceTableRequestHelper<T> : MobileServiceBaseRequestHelper
        where T : class
    {
        private string _tableName;

        public MobileServiceTableRequestHelper(string azureEndPoint, string token, MobileServiceUser User)
            : base(azureEndPoint, token, User)
        {
            _azureEndPoint = azureEndPoint + "tables/";
            _tableName = typeof(T).Name;

            this.CreateClient();
        }

        public Dictionary<RestRequestAsyncHandle, Action<AzureResponse<T>>> _PostCallbacks = new Dictionary<RestRequestAsyncHandle,Action<AzureResponse<T>>>();
        public Dictionary<RestRequestAsyncHandle, T> _PostItems = new Dictionary<RestRequestAsyncHandle, T>();
 
        public void PostAsync(T requestData, Action<AzureResponse<T>> callback)
        {
            var json = MobileServiceBaseRequestHelper.SerializeObject(requestData);
            var request = (IRestRequest)(new RestRequest(_tableName, Method.POST));
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var postHandle = _client.ExecuteAsync<T>(request, PostAsyncHandler);
            
            _PostCallbacks.Add(postHandle, callback);
            _PostItems.Add(postHandle, requestData);
        }

        private void PostAsyncHandler(IRestResponse<T> restResponse, RestRequestAsyncHandle handle)
        {
            CheckError(restResponse);
            var postResponse = MobileServiceBaseRequestHelper.DeserialiseObject<T>(restResponse);
            
            var originalItem = _PostItems[handle];
            _PostItems.Remove(handle);

            var id = GetItemId(postResponse.ResponseData);
            SetItemId<T>(originalItem, id);

            var callback = _PostCallbacks[handle];
            _PostCallbacks.Remove(handle);

            if (callback != null) callback(postResponse);
        }

        public static int GetItemId<T>(T item) where T : class
        {
            var type = typeof(T);
            var prop = MobileServiceTableRequestHelper<T>.GetIdProperty();
            var id = (int)prop.GetValue(item, null);
            return id;
        }

        public static void SetItemId<T>(T item, int? id) where T : class
        {
            var type = typeof(T);
            var prop = MobileServiceTableRequestHelper<T>.GetIdProperty();
            prop.SetValue(item, id, null);
        }

#if UNITY_METRO && !UNITY_EDITOR
        public static PropertyInfo GetIdProperty()
        {
            var type = typeof(T).GetTypeInfo().DeclaredProperties;
            return null;
        }
#else
        public static PropertyInfo GetIdProperty()
        {
            var type = typeof(T);
            var idProperty = type.GetProperty("Id");
            return idProperty;
        }
#endif

        public Dictionary<RestRequestAsyncHandle, Action<AzureResponse<T>>> _PutCallbacks = new Dictionary<RestRequestAsyncHandle, Action<AzureResponse<T>>>();

        public void PutAsync(T requestData, int id, Action<AzureResponse<T>> callback)
        {
            var json = SerializeObject(requestData);
            var request = new RestRequest(_tableName + "/" + id, Method.PATCH);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var handle = _client.ExecuteAsync(request, PutAsyncHandler);
            
            // Store the handle
            _PutCallbacks.Add(handle, callback);
        }

        public void PutAsyncHandler(IRestResponse response, RestRequestAsyncHandle handle) 
        {
            var putResponse = DeserialiseObject<T>(response);
            putResponse.handle = handle;

            var callback = _PutCallbacks[handle];
            _PutCallbacks.Remove(handle);

            callback(putResponse);
        }

        private Dictionary<RestRequestAsyncHandle, Action<AzureResponse<object>>> _DeleteCallbacks = new Dictionary<RestRequestAsyncHandle, Action<AzureResponse<object>>>();

        public RestRequestAsyncHandle DeleteAsync(int id, Action<AzureResponse<object>> callback)
        {
            var request = new RestRequest(_tableName + "/" + id, Method.DELETE);
            request.AddHeader("Content-Type", "application/json");
            if (callback == null)
            {
                callback = DefaultHandler;
            }
            var handle = _client.ExecuteAsync<object>(request, DeleteAsyncHandler);
            _DeleteCallbacks.Add(handle, callback);
            return handle;
        }

        public void DefaultHandler(AzureResponse<object> response) { return; }

        private void DeleteAsyncHandler(IRestResponse<object> restResponse)
        {
            
        }
        
        private Dictionary<RestRequestAsyncHandle, Action<AzureResponse<List<T>>>> _ListCallbacks = new Dictionary<RestRequestAsyncHandle,Action<AzureResponse<List<T>>>>();
        private Dictionary<RestRequestAsyncHandle, Action<AzureResponse<T>>> _ItemCallbacks = new Dictionary<RestRequestAsyncHandle, Action<AzureResponse<T>>>();

        public RestRequestAsyncHandle GetAsync(Action<AzureResponse<List<T>>> callback)
        {
            var request = new RestRequest(_tableName, Method.GET);
            var handle = _client.ExecuteAsync<List<T>>(request, GetAsyncHandler);

            _ListCallbacks.Add(handle, callback);
            return handle;
        }

        public void GetAsyncHandler(IRestResponse<List<T>> restResponse, RestRequestAsyncHandle handle)
        {
            var response = DeserialiseObjectList(restResponse);
            var callback = _ListCallbacks[handle];
            response.handle = handle;
            _ListCallbacks.Remove(handle);
            callback(response);
        }

        public RestRequestAsyncHandle GetAsync(int id, Action<AzureResponse<T>> callback)
        {
            var request = new RestRequest(_tableName + "/" + id, Method.GET);
            var handle = _client.ExecuteAsync<T>(request, GetIdAsyncHandler);
            _ItemCallbacks.Add(handle, callback);
            return handle;
        }

        public void GetIdAsyncHandler(IRestResponse<T> restResponse, RestRequestAsyncHandle handle)
        {
            var response = DeserialiseObject<T>(restResponse);
            response.handle = handle;
            var callback = _ItemCallbacks[handle];
            _ItemCallbacks.Remove(handle);
            callback(response);
        }
                
        private static AzureResponse<List<T>> DeserialiseObjectList(IRestResponse restResponse)
        {
            AzureResponse<List<T>> response;
            if (restResponse.ResponseStatus == ResponseStatus.Completed)
            {
                var obj = JsonConvert.DeserializeObject<List<T>>(restResponse.Content);
                response = new AzureResponse<List<T>>(AzureResponseStatus.Success, obj);
            }
            else
            {
                response = new AzureResponse<List<T>>(restResponse.ErrorException);
            }
            return response;
        }

        public RestRequestAsyncHandle QueryAsync(Expression<Func<T, bool>> predicate, Action<AzureResponse<List<T>>> callback)
        {
            ODataExpressionConverter c = new ODataExpressionConverter();
            var query = c.Convert<T>(predicate);

            var request = new RestRequest(_tableName + "?$filter=" + query, Method.GET);

            var handle = _client.ExecuteAsync<List<T>>(request, GetAsyncHandler);
            _ListCallbacks.Add(handle, callback);
            
            return handle;
        }

    }
}
