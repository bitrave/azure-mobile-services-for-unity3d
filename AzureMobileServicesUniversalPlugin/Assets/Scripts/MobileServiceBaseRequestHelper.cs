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
    internal abstract class MobileServiceBaseRequestHelper
    {
        protected RestClient _client;
        protected string _azureEndPoint;
        protected string _baseEndPoint;
        protected string _token;
        protected MobileServiceUser _user = null;

        protected MobileServiceBaseRequestHelper(string azureEndPoint, string token, MobileServiceUser User)
        {
            // THIS IS INSECURE... PLEASE DO NOT BE STUPID FROM WITHIN THE EDITOR
            // TODO: Implement the proper workaround for certificate failure in Mono
#if UNITY_EDITOR || !UNITY_WP8
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;
#endif
            _baseEndPoint = azureEndPoint;
            _token = token;
            _user = User;
        }

        protected void CreateClient()
        {
            _client = new RestClient(_azureEndPoint);
            _client.AddDefaultHeader("Accept", "application/json");
            _client.AddDefaultHeader("X-ZUMO-APPLICATION", _token);

            if (_user != null)
            {
                _client.AddDefaultHeader("X-ZUMO-AUTH", _user.AuthenticationToken);
            }
        }

        private Action<AzureResponse<MobileServiceUser>> _LoginAsyncCallback = null;

        public void LoginAsync(AuthenticationProvider provider, string token, Action<AzureResponse<MobileServiceUser>> callback)
        {
            AuthenticationToken authToken = CreateToken(provider, token);
            _LoginAsyncCallback = callback;

            var path = "/login/" + provider.ToString().ToLower();
            var baseClient = new RestClient(_baseEndPoint);
            var request = new RestRequest(path, Method.POST);
            var json = SerializeObject(authToken);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var handle = baseClient.ExecuteAsync<MobileServiceUser>(request, LoginAsyncHandler);
        }

        private void LoginAsyncHandler(IRestResponse<MobileServiceUser> restResponse, RestRequestAsyncHandle handle)
        {
            var response = DeserialiseObject<MobileServiceUser>(restResponse);
            response.handle = handle;
            _LoginAsyncCallback(response);
        }

        protected static AuthenticationToken CreateToken(AuthenticationProvider provider, string token)
        {
            AuthenticationToken authToken = null;

            switch (provider)
            {
                case AuthenticationProvider.Facebook:
                case AuthenticationProvider.Google:
                case AuthenticationProvider.Twitter:
                    {
                        authToken = new FacebookGoogleAuthenticationToken() { access_token = token };
                        break;
                    }
                case AuthenticationProvider.MicrosoftAccount:
                    {
                        authToken = new MicrosoftAuthenticationToken() { authenticationToken = token };
                        break;
                    }
            }

            return authToken;
        }

        protected void CheckError(IRestResponse restResponse)
        {
            if (restResponse.ResponseStatus != ResponseStatus.Completed)
            {
                var error = new AzureResponse<object>(restResponse);
                Debug.LogError("Rest Error:" + restResponse.ErrorMessage);
            }
        }

        protected static string SerializeObject<X>(X requestData)
        {
            object o = requestData as object;

            var obj = JsonConvert.SerializeObject(o, Formatting.None,
                                                  new JsonSerializerSettings()
                                                  {
                                                      NullValueHandling = NullValueHandling.Ignore
                                                  });
            return obj;
        }

        protected static AzureResponse<X> DeserialiseObject<X>(IRestResponse restResponse)
        {
            AzureResponse<X> response;

            if (restResponse != null &&
                restResponse.ResponseStatus == ResponseStatus.Completed &&
                restResponse.StatusCode == HttpStatusCode.OK
                || restResponse.StatusCode == HttpStatusCode.Created)
            {
                var obj = JsonConvert.DeserializeObject<X>(restResponse.Content);
                response = new AzureResponse<X>(AzureResponseStatus.Success, obj);
            }
            else if (restResponse != null)
            {
                Debug.Log("Response is: " + restResponse.StatusCode);
                response = new AzureResponse<X>(restResponse);
            }
            else
            {
                response = new AzureResponse<X>(new Exception("Response was null from REST libraries."));
            }
            return response;
        }
    }
}
