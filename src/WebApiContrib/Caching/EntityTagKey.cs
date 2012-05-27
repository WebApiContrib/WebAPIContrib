using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiContrib.Caching
{
	public class EntityTagKey
	{
		private readonly string _resourceUri;
		private readonly IEnumerable<string> _headerValues;
		private readonly string _toString;
		private readonly string _routePattern;

		private const string EntityTagKeyFormat = "{0}-{1}";

		public EntityTagKey(string resourceUri, IEnumerable<string> headerValues)
			: this(resourceUri, headerValues, resourceUri)
		{
					
		}

		/// <summary>
		/// constructor for EntityTagKey
		/// </summary>
		/// <param name="resourceUri">URI of the resource</param>
		/// <param name="headerValues">value of the headers as in the request. Only those values whose named defined in VaryByHeader
		/// must be passed
		/// </param>
		/// <param name="routePattern">route pattern for the URI. by default it is the same
		/// but in some cases it could be different.
		/// For example /api/cars/fastest and /api/cars/mostExpensive can share tha pattern /api/cars/*
		/// This will be used at the time of cache invalidation. 
		/// </param>
		public EntityTagKey(string resourceUri, IEnumerable<string> headerValues, string routePattern)
		{
			_routePattern = routePattern;
			_headerValues = headerValues.ToList();
			_resourceUri = resourceUri;
			_toString = string.Format(EntityTagKeyFormat, resourceUri, string.Join("-", headerValues));
		}

		public string ResourceUri
		{
			get { return _resourceUri; }
		}

		public string RoutePattern
		{
			get { return _routePattern; }
		}

		public override string ToString()
		{
			return _toString;
		}


		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			var eTagKey = obj as EntityTagKey;
			if (eTagKey == null)
				return false;
			return ToString() == eTagKey.ToString();
		}

		public override int GetHashCode()
		{
			return _toString.GetHashCode();
		}

	}
}
