using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using XTramile.Infrastructure.IServiceProvider;
using XTramile.Infrastructure.Configuration;
namespace XTramile.Infrastructure.HelperService
{
    public class XtramileCacheService<T> : IXtramileCacheService<T> where T :class 
    { 
        private readonly IMemoryCache _cache;
        public XtramileCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public void CreateEntry(object key,T data)
        {
            var serializeJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(relative: TimeSpan.FromSeconds(CahceTimeSettings.TeenHours)) 
                .SetSlidingExpiration(TimeSpan.FromSeconds(CahceTimeSettings.TeenHours));  
            _cache.Set(key, serializeJson,cacheOptions);
        }
        public void CreateEntry(object key,IEnumerable<T> data)
        {
            var serializeJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(relative: TimeSpan.FromSeconds(CahceTimeSettings.OneHours)) 
                .SetSlidingExpiration(TimeSpan.FromSeconds(CahceTimeSettings.OneHours));  
            _cache.Set(key, serializeJson,cacheOptions);
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }

        public List<T> TryGetValue(object key)
        {
            var dataList = new List<T>();
            var dataCache = _cache.Get(key);
            if(dataCache != null)
            {
                dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(dataCache.ToString());
            }

            return dataList;
        }
        public T TryGetValueData(object key)
        {
            var dataCache = _cache.Get(key);
            if(dataCache != null)
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(dataCache.ToString());
                return data;
            }
            return null;
        }

    }

}
