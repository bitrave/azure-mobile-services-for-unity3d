using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bitrave.Azure
{
    public interface IAzureMobileServices
    {
        bool Initialised();
        
        void Insert<T>(T item) where T : class;
        void Update<T>(T item) where T : class;
        void LoginAsync(AuthenticationProvider provider, Action<AzureResponse<MobileServiceUser>> callback);
        void Delete<T>(T item) where T : class;

        /// <summary>
        /// Returns a filtered list.  NOTE: The predicate runs client side in
        /// the Unity Editor version, so you may not get expected behaviours (yet) in the Editor.  
        /// </summary>
        void Where<T>(Expression<Func<T, bool>> predicate, Action<AzureResponse<List<T>>> callback) where T : class;

        void Lookup<T>(int id, Action<AzureResponse<T>> callback) where T : class;
        void Read<T>(Action<AzureResponse<List<T>>> callback) where T : class;
    }
}