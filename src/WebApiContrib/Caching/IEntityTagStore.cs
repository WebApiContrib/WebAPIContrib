using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace WebApiContrib.Caching
{
	/// <summary>
	/// This is an interface representing an ETag store acting similar to a dictionary. 
	/// storing and retriving ETags.
	///  
	/// In a single-server scenario, this could be an in-memory disctionary implementation
	/// while in a server farm, this will be a persistent store.
	/// </summary>
	public interface IEntityTagStore
	{
		bool TryGet(EntityTagKey key, out TimedEntityTagHeaderValue eTag);
		void AddOrUpdate(EntityTagKey key, TimedEntityTagHeaderValue eTag);
		bool TryRemove(EntityTagKey key);
		void Clear();
	}
}
