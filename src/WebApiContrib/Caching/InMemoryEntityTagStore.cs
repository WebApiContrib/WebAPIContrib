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

		public bool TryGet(EntityTagKey key, out TimedEntityTagHeaderValue eTag)
		{
			return _eTagCache.TryGetValue(key, out eTag);
		}

		public void AddOrUpdate(EntityTagKey key, TimedEntityTagHeaderValue eTag)
		{
			_eTagCache.AddOrUpdate(key, eTag, (theKey, oldValue) => eTag);
		}

		public bool TryRemove(EntityTagKey key)
		{
			TimedEntityTagHeaderValue entityTagHeaderValue;
			return _eTagCache.TryRemove(key, out entityTagHeaderValue);
		}

		public void Clear()
		{
			_eTagCache.Clear();
		}
	}
}
