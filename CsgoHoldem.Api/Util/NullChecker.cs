using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CsgoHoldem.Api.Util
{
    public class NullChecker
    {
        public static void HasNoNulls<T>([NotNull] string parameterName, IReadOnlyList<T> value, [CallerMemberName] string caller = null, [CallerLineNumber] int lineNumber = 0)
            where T : class
        {
            Ref(parameterName, value, caller, lineNumber);

            if (value.Any(e => e == null))
            {
                throw new ArgumentException($"Should not have any null values At {caller} on Line: {lineNumber}", parameterName);
            }
        }
        public static CheckBuilder Ref(string name, object value, [CallerMemberName] string caller = null, [CallerLineNumber] int lineNumber = 0)
        {
            var message = $"{caller} on Line: {lineNumber}";
            if (value == null) {
                throw new ArgumentNullException(name, message);
            }
            return new CheckBuilder(message);
        }
        
        public static CheckBuilder Var(string name, object value, string message)
        {
            if (value == null) {
                throw new ArgumentNullException(name, message);
            }
            return new CheckBuilder(message);
        }
    }
}