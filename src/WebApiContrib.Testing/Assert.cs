using System;
using System.Text;

namespace WebApiContrib.Testing
{
    /// <summary>
    /// Test framework agnostic assertions
    /// </summary>
    internal static class Assert
    {
        /// <summary>
        /// Assert that the value is true
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <param name="message">Message to display when assertion fails</param>
        /// <exception cref="WebApiContrib.Testing.AssertionException"></exception>
        public static void IsTrue(bool actual, string message = null)
        {
            if(actual)
                return;

            string errorMessage = BuildErrorMessage("Expected the value to be true", message);
            throw new AssertionException(errorMessage);
        }

        /// <summary>
        /// Asserts that the object is not null
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <param name="message">Message to display when assertion fails</param>
        /// <exception cref="WebApiContrib.Testing.AssertionException"></exception>
        public static void IsNotNull(object actual, string message = null)
        {
            if(actual != null)
                return;

            string errorMessage = BuildErrorMessage("Expected the value to not be null", message);
            throw new AssertionException(errorMessage);
        }

        /// <summary>
        /// Asserts that the object is of type T
        /// </summary>
        /// <typeparam name="T">The expected type</typeparam>
        /// <param name="actual">The actual instance</param>
        /// <param name="message">Message to display when assertion fails</param>
        /// <exception cref="WebApiContrib.Testing.AssertionException"></exception>
        public static T InstanceOf<T>(object actual, string message = null)
            where T : class
        {
            var actualT = actual as T;
            if(actualT != null)
                return actualT;

            string expectedType = typeof(T).FullName;
            string actualType = actual == null ? "null" : actual.GetType().FullName;
            string errorMessage = BuildErrorMessage(expectedType, actualType, message);
            throw new AssertionException(errorMessage);
        }

        /// <summary>
        /// Asserts that the object is the expected value
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="message">Message to display when assertion fails</param>
        /// <exception cref="WebApiContrib.Testing.AssertionException"></exception>
        public static void AreEqual(object expected, object actual, string message = null)
        {
            if(Equals(actual, expected))
                return;

            string errorMessage = BuildErrorMessage(expected, actual, message);
            throw new AssertionException(errorMessage);
        }

        /// <summary>
        /// Compares the two strings (culture and case-insensitive).
        /// </summary>
        /// <param name="expected">The expected string.</param>
        /// <param name="actual">The actual string.</param>
        /// <param name="message">Message to display when assertion fails</param>
        /// <exception cref="WebApiContrib.Testing.AssertionException"></exception>
        public static void AreSameString(string expected, string actual, string message = null)
        {
            if(string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase))
                return;

            string errorMessage = BuildErrorMessage(expected, actual, message);
            throw new AssertionException(errorMessage);
        }

        private static string BuildErrorMessage(string assertionMessage, string message)
        {
            var exceptionMessage = new StringBuilder();
            exceptionMessage.AppendLine(assertionMessage);
            if(message != null)
            {
                exceptionMessage.AppendLine(message);
            }

            return exceptionMessage.ToString();
        }

        private static string BuildErrorMessage(object expected, object actual, string message)
        {
            string actualValue = actual != null ? actual.ToString() : "null";
            string expectedValue = expected != null ? expected.ToString() : "null";

            var exceptionMessage = new StringBuilder();
            exceptionMessage.AppendFormat("was {0} but expected {1}", actualValue, expectedValue);
            if(message != null)
            {
                exceptionMessage.AppendLine();
                exceptionMessage.AppendLine(message);
            }

            return exceptionMessage.ToString();
        }
    }
}