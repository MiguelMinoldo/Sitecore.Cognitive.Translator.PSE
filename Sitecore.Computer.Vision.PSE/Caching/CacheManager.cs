using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace Sitecore.Cognitive.Translator.PSE.Caching
{
    public class CacheManager
    {
        public static CachingService CustomCache { get; } = new CachingService("Cognitive[Translator]", 
            StringUtil.ParseSizeString(Settings.GetSetting("Sitecore.Cognitive.Translator.PSE.TranslateService.CacheSize", "50MB")));
        private static readonly Dictionary<string, object> CacheKeyDictionary = new Dictionary<string, object>();

        public static object Get(string key)
        {
            return CustomCache.Get(key);
        }

        public static void Set(string key, object value)
        {
            CustomCache.Set(key, value);
        }

        public void ClearCache(object sender, EventArgs args)
        {
            Log.Info($"Sitecore.Cognitive.Translator.PSE.TranslateService Cache Clearer.", this);
            CustomCache.Clear();
            Log.Info("Sitecore.Cognitive.Translator.PSE.TranslateService Cache Clearer done.", (object)this);
        }

        public static TObj GetCachedObject<TObj>(string cacheKey, Func<TObj> creator) where TObj : class
        {
            var obj = Get(cacheKey) as TObj;

            if (obj == null)
            {
                // get the lock object
                var lockObject = GetCacheLockObject(cacheKey);

                try
                {
                    lock (lockObject)
                    {
                        obj = creator.Invoke();

                        Set(cacheKey, obj);
                    }
                }
                finally
                {
                    RemoveCacheLockObject(cacheKey);
                }
            }

            return obj;
        }

        private static object GetCacheLockObject(string cacheKey)
        {
            lock (CacheKeyDictionary)
            {
                if (!CacheKeyDictionary.ContainsKey(cacheKey))
                {
                    CacheKeyDictionary.Add(cacheKey, new object());
                }

                return CacheKeyDictionary[cacheKey];
            }
        }

        private static void RemoveCacheLockObject(string cacheKey)
        {
            lock (CacheKeyDictionary)
            {
                if (CacheKeyDictionary.ContainsKey(cacheKey))
                {
                    CacheKeyDictionary.Remove(cacheKey);
                }
            }
        }
    }
}
