using System;
using System.Collections.Generic;
namespace XTramile.Infrastructure.IServiceProvider
{
    public interface IXtramileCacheService<T> where T : class
    { 
        void CreateEntry(object key,T data);
        void CreateEntry(object key,IEnumerable<T> data);
        void Remove(object key);
        List<T> TryGetValue(object key);
        T TryGetValueData(object key);
    }



}
