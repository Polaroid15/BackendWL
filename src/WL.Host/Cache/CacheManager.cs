using System.Collections;
using System.Collections.Concurrent;

namespace WL.Host.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly ConcurrentDictionary<int, IDictionary> _container;
        private readonly ConcurrentDictionary<int, (Type, Type)> _keys;
        private readonly TimeSpan _expireTime;
        private readonly Timer _timer;
        private readonly Timer _timerMetric;
        //
        // private readonly Gauge _gauge = Metrics.CreateGauge("app_cache", "application cache", new GaugeConfiguration
        // {
        //     LabelNames = new[] { "key", "value" },
        // });

        public CacheManager()
            : this(new TimeSpan(0, 10, 0))
        {
        }

        public CacheManager(TimeSpan expireTime)
        {
            _container = new ConcurrentDictionary<int, IDictionary>();
            _keys = new ConcurrentDictionary<int, (Type, Type)>();
            _expireTime = expireTime;
            var period = expireTime.TotalSeconds > 1 ? expireTime / 3 : new TimeSpan(0, 0, 1);
            _timer = new Timer(Clear, _timer, new TimeSpan(0, 0, 0), period);
            _timerMetric = new Timer(CollectMetrics, _timerMetric, new TimeSpan(0, 0, 0), TimeSpan.FromMilliseconds(500));
        }

        public CacheManager(TimeSpan expireTime, TimeSpan clearPeriod)
        {
            _container = new ConcurrentDictionary<int, IDictionary>();
            _keys = new ConcurrentDictionary<int, (Type, Type)>();
            _expireTime = expireTime;
            _timer = new Timer(Clear, _timer, new TimeSpan(0, 0, 0), clearPeriod);
            _timerMetric = new Timer(CollectMetrics, _timerMetric, new TimeSpan(0, 0, 0), TimeSpan.FromMilliseconds(500));
        }

        public bool GetValue<TKey, TValue>(TKey key, out TValue value, bool isTouched = false)
        {
            var cache = GetCache<TKey, TValue>();
            var res = cache.TryGetValue(key, out CacheValue<TValue> innerValue);

            if (!res || innerValue.IsExpire)
            {
                value = default(TValue);
                return false;
            }

            if (isTouched)
            {
                innerValue.Refresh(_expireTime);
            }

            value = innerValue.Value;
            return res;
        }

        public IEnumerable<TValue> GetValues<TKey, TValue>()
        {
            var cache = GetCache<TKey, TValue>();
            return cache.Select(x => x.Value.Value);
        }

        public TValue AddOrUpdate<TKey, TValue>(TKey key, TValue value)
        {
            return AddOrUpdate(key, value, _expireTime, (k, v) => value);
        }

        public TValue AddOrUpdate<TKey, TValue>(TKey key, TValue value, TimeSpan timeToExpire)
        {
            return AddOrUpdate(key, value, timeToExpire, (k, v) => value);
        }

        public TValue AddOrUpdate<TKey, TValue>(TKey key, TValue value, Func<TKey, TValue, TValue> updateValue)
        {
            return AddOrUpdate(key, value, _expireTime, updateValue);
        }

        public TValue AddOrUpdate<TKey, TValue>(TKey key, TValue value, TimeSpan timeToExpire, Func<TKey, TValue, TValue> updateValue)
        {
            var cache = GetCache<TKey, TValue>();
            return cache.AddOrUpdate(
                key,
                new CacheValue<TValue>(value, timeToExpire),
                (k, v) => new CacheValue<TValue>(updateValue(k, v.Value), timeToExpire)).Value;
        }

        public TValue AddOrUpdateNoTouch<TKey, TValue>(TKey key, TValue value, TimeSpan timeToExpire, Func<TKey, TValue, TValue> updateValue)
        {
            var cache = GetCache<TKey, TValue>();
            return cache.AddOrUpdate(
                key,
                new CacheValue<TValue>(value, timeToExpire),
                (k, v) =>
                {
                    v.Value = updateValue(k, v.Value);
                    return v;
                }).Value;
        }

        public bool TryAdd<TKey, TValue>(TKey key, TValue value)
        {
            return TryAdd(key, value, _expireTime);
        }

        public bool TryAdd<TKey, TValue>(TKey key, TValue value, TimeSpan timeToExpire)
        {
            var cache = GetCache<TKey, TValue>();
            return cache.TryAdd(key, new CacheValue<TValue>(value, timeToExpire));
        }

        public bool Remove<TKey, TValue>(TKey key, out TValue value)
        {
            var cache = GetCache<TKey, TValue>();

            var result = cache.TryRemove(key, out CacheValue<TValue> cacheValue);
            if (result)
            {
                value = cacheValue.Value;
                return result;
            }

            value = default;
            return false;
        }

        public int Count<TKey, TValue>()
        {
            return GetCache<TKey, TValue>().Count;
        }

        public long CountAll()
        {
            long count = 0L;

            foreach (var dictionary in _container.Select(x => x.Value))
            {
                count += dictionary.Count;
            }

            return count;
        }

        private ConcurrentDictionary<TKey, CacheValue<TValue>> GetCache<TKey, TValue>()
        {
            var keyType = typeof(TKey);
            var valueType = typeof(TValue);
            var hash = HashCode.Combine(keyType.FullName, valueType.FullName);

            if (_container.TryGetValue(hash, out IDictionary cache))
            {
                return (ConcurrentDictionary<TKey, CacheValue<TValue>>)cache;
            }

            _keys.TryAdd(hash, (keyType, valueType));

            return (ConcurrentDictionary<TKey, CacheValue<TValue>>)_container.GetOrAdd(hash, new ConcurrentDictionary<TKey, CacheValue<TValue>>());
        }

        private void Clear(object state)
        {
            foreach (var dictionary in _container.Select(x => x.Value))
            {
                foreach (DictionaryEntry item in dictionary)
                {
                    if (((ICacheValue)item.Value).IsExpire)
                    {
                        dictionary.Remove(item.Key);
                    }
                }
            }
        }

        private void CollectMetrics(object state)
        {
            foreach (var item in _container)
            {
                var count = item.Value.Count;
                var key = _keys[item.Key].Item1.Name;
                var val = _keys[item.Key].Item2.Name;

                // _gauge.WithLabels(key, val).Set(count);
            }
        }
    }
}
