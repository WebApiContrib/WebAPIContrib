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

		private const string EntityTagKeyFormat = "{0}-{1}";

		public EntityTagKey(string resourceUri, IEnumerable<string> headerValues)
		{
			_headerValues = headerValues.ToList();
			_resourceUri = resourceUri;
			_toString = string.Format(EntityTagKeyFormat, resourceUri, string.Join("-", headerValues));
		}

		public string ResourceUri
		{
			get { return _resourceUri; }
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
