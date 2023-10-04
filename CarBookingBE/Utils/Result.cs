using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.Utils
{
    public class Result<T>
    {
        public Result(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }
        public Result(bool Success, string Message, T Data)
        {
            this.Success = Success;
            this.Message = Message;
            this.Data = Data;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}