using System;

namespace GameBox.Console.Vendor.LitJson
{
    internal class JsonException :
#if NETSTANDARD1_5
        Exception
#else
        ApplicationException
#endif
    {
        public JsonException()
            : base()
        {
        }

        internal JsonException(ParserToken token)
            : base(string.Format(
                    "Invalid token '{0}' in input string", token))
        {
        }

        internal JsonException(ParserToken token,
                                System.Exception inner_exception)
            : base(string.Format(
                    "Invalid token '{0}' in input string", token),
                inner_exception)
        {
        }

        internal JsonException(int c)
            : base(String.Format(
                    "Invalid character '{0}' in input string", (char)c))
        {
        }

        internal JsonException(int c, System.Exception inner_exception)
            : base(String.Format(
                    "Invalid character '{0}' in input string", (char)c),
                inner_exception)
        {
        }

        public JsonException(string message)
            : base(message)
        {
        }

        public JsonException(string message, System.Exception inner_exception)
            : base(message, inner_exception)
        {
        }
    }
}
