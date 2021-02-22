using System;

namespace CsgoHoldem.Api.Util
{
    public class CheckBuilder
    {
        private string _message;
        public CheckBuilder(string message)
        {
            _message = message;
        }
        
        public CheckBuilder Ref(string name, object value)
        {
            if (value == null) {
                throw new ArgumentNullException(name, _message);
            }

            return this;
        }
    }
}