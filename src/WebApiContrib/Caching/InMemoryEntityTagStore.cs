using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace WebApiContrib.Caching
{
	public class InMemoryEntityTagStore : IEntityTagStore
	{
		private readonly ConcurrentDictionary<EntityTagKey, TimedEntityTagHeaderValue> _eTagCache = new ConcurrentDictionary<EntityTagKey, TimedEntityTagHeaderValue>();
		private readonly ConcurrentDictionary<string, HashSet<EntityTagKey>> _routePatternCache = new ConcurrentDictionary<string, HashSet<EntityTagKey>>();

		public bool TryGetValue(EntityTagKey key, out TimedEntityTagHeaderValue eTag)
		{
			return _eTagCache.TryGetValue(key, out eTag);
		}

		public void AddOrUpdate(EntityTagKey key, TimedEntityTagHeaderValue eTag)
		{
			_eTagCache.AddOrUpdate(key, eTag, (theKey, oldValue) => eTag);
			_routePatternCache.AddOrUpdate(key.RoutePattern, new HashSet<EntityTagKey>(), 
				(routePattern, hashSet) =>
					{
						hashSet.Add(key);
						return hashSet;
					});
		}

		public bool TryRemove(EntityTagKey key)
		{
			TimedEntityTagHeaderValue entityTagHeaderValue;
			return _eTagCache.TryRemove(key, out entityTagHeaderValue);
		}

		public int RemoveAllByRoutePattern(string routePattern)
		{
			int count = 0;
			HashSet<EntityTagKey> keys;
			if(_routePatternCache.TryGetValue(routePattern, out keys))
			{
				count = keys.Count;
				keys.Select(x => this.TryRemove(x));
				_routePatternCache.TryRemove(routePattern, out keys);
			}
			return count;
		}

		public void Clear()
		{
			_eTagCache.Clear();
		}
	}
}
