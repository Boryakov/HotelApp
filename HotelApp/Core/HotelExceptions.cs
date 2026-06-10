using System;
using System.Collections.Generic;
using System.Text;


namespace HotelApp.Core
{
    // Custom exception class designed to handle application-specific hotel management runtime errors
    internal class HotelException : Exception
    {
        public HotelException() : base() { }

        public HotelException(string message) : base(message) { }

        public HotelException(string message, Exception innerException) : base(message, innerException) { }
    }
}