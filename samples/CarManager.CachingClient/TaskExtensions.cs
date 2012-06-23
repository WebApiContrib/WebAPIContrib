using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarManager.CachingClient
{
	static class TaskExtensions
	{
		public static string WrapException<T>(this Task<T> task) where T: class 
		{
			string result = null;
			try
			{
				T t = task.Result;
				return t == null ? string.Empty : t.ToString();
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
	}
}
