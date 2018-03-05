using System;


namespace gwn.common.exceptionhandling
{
    // custom exception for input validation ?
    public class ApiException : System.Exception
    {
        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception ex) : base(message, ex)  { }
    }
}
