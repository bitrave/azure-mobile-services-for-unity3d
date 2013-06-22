using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace Bitrave.Azure
{

    internal class MobileServiceRequestHelper<T> where T : class
    {
        private RestClient _client;
        private string _tableName;
        private string _azureEndPoint;

        public Action<AzureResponse<object>> OnError;

        public MobileServiceRequestHelper(string azureEndPoint, string token)
        {
            // THIS IS INSECURE... PLEASE DO NOT BE STUPID FROM WITHIN THE EDITOR
            // TODO: Implement the proper workaround for certificate failure in Mono
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;

            _azureEndPoint = azureEndPoint + "tables/";
            _tableName = typeof(T).Name;

            _client = new RestClient(_azureEndPoint);
            //_client.AddDefaultHeader("Accept", "application/json");
            _client.AddDefaultHeader("X-ZUMO-APPLICATION", token);
        }

        public void Post(T requestData)
        {
            var json = SerializeObject(requestData);
            var request = new RestRequest(_tableName, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var restResponse = _client.Execute(request);

            CheckError(restResponse);

            if (restResponse.ResponseStatus == ResponseStatus.Completed)
            {
                var result = DeserialiseObject(restResponse);

                var type = typeof(T);
                var idProperty = type.GetProperty("Id");

                var id = idProperty.GetValue(result.ResponseData, null);
                idProperty.SetValue(requestData, id, null);
            }
        }

        private void CheckError(IRestResponse restResponse)
        {
            if (restResponse.ResponseStatus != ResponseStatus.Completed)
            {
                var error = new AzureResponse<object>(restResponse.ErrorException); ;
                OnError(error);
            }
        }

        public void Put(T requestData, int id)
        {
            var json = SerializeObject(requestData);
            var request = new RestRequest(_tableName + "/" + id, Method.PATCH);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var restResponse = _client.Execute(request);
            CheckError(restResponse);
        }

        private static string SerializeObject(T requestData)
        {
            var obj = JsonConvert.SerializeObject(requestData,
                                                  new JsonSerializerSettings()
                                                  {
                                                      NullValueHandling = NullValueHandling.Ignore
                                                  });
            return obj;
        }

        public void Delete(int id)
        {
            var request = new RestRequest(_tableName + "/" + id, Method.DELETE);
            request.AddHeader("Content-Type", "application/json");
            var restResponse = _client.Execute(request);
            CheckError(restResponse);
        }

        public AzureResponse<List<T>> Get()
        {
            var request = new RestRequest(_tableName, Method.GET);
            var restResponse = _client.Execute(request);
            var response = DeserialiseObjectList(restResponse);
            return response;
        }

        public AzureResponse<T> Get(int id)
        {
            var request = new RestRequest(_tableName + "/" + id, Method.GET);
            var restResponse = _client.Execute(request);
            var response = DeserialiseObject(restResponse);
            return response;
        }

        private static AzureResponse<T> DeserialiseObject(IRestResponse restResponse)
        {
            AzureResponse<T> response;
            if (restResponse.ResponseStatus == ResponseStatus.Completed)
            {
                var obj = JsonConvert.DeserializeObject<T>(restResponse.Content);
                response = new AzureResponse<T>(AzureResponseStatus.Success, obj);
            }
            else
            {
                response = new AzureResponse<T>(restResponse.ErrorException);
            }
            return response;
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

        //TODO: The predicate runs _client side so it's only for testing.  Query needs a predicate to query string framework, of which most don't support .NET 3.5 for Unity.
        public AzureResponse<List<T>> Query(Func<T, bool> predicate)
        {
            var serverResponse = Get();
            if (serverResponse.Status != AzureResponseStatus.Failure)
            {
                var filteredData = from t in serverResponse.ResponseData where predicate(t) select t;
                serverResponse.ResponseData = filteredData.ToList();
            }

            return serverResponse;
        }
    }
}
