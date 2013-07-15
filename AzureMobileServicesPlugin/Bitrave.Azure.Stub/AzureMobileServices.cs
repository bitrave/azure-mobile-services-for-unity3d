using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bitrave.Azure
{
    public class AzureMobileServices : IAzureMobileServices
    {
        public bool Initialised()
        {
            throw new NotImplementedException();
        }

        public AzureMobileServices(string url, string token)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        public void LoginAsync(AuthenticationProvider provider, Action<AzureResponse<MobileServiceUser>> callback)
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        public void Where<T>(Expression<Func<T, bool>> predicate, Action<AzureResponse<List<T>>> callback) where T : class
        {
            throw new NotImplementedException();
        }

        public void Lookup<T>(int id, Action<AzureResponse<T>> callback) where T : class
        {
            throw new NotImplementedException();
        }

        public void Read<T>(Action<AzureResponse<List<T>>> callback) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
