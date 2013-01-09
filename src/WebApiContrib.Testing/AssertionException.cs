// http://mvccontrib.codeplex.com
// Copyright 2007-2010 Eric Hexter, Jeffrey Palermo

using System;
using System.Linq;

namespace WebApiContrib.Testing
{
    ///<summary>
    /// This exception is thrown by the Testing extension methods.  This allows this project to be unit test framework agnostic.
    ///</summary>
    public class AssertionException : Exception
    {
        public AssertionException(string message)
            : base(message)
        {
        }

        public override string StackTrace
        {
            get
            {
                string Namespace = GetType().Namespace;
                var stacktracestring = SplitTheStackTraceByEachNewLine()
                    .Where(s => !s.TrimStart(' ').StartsWith("at " + Namespace))
                    .ToArray();
                return JoinArrayWithNewLineCharacters(stacktracestring);
            }
        }

        private string[] SplitTheStackTraceByEachNewLine()
        {
            return base.StackTrace.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string JoinArrayWithNewLineCharacters(string[] stacktracestring)
        {
            return string.Join(Environment.NewLine, stacktracestring);
        }
    }
}