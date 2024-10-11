using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedAPI
{
    public class ResponseDto<T>
    {
        public bool isSuccessful {  get; set; }

        public int statusCode { get; set; }

        public T data { get; set; }

        public IEnumerable<string>? Errors { get; set; }

        public static ResponseDto<T> Success(int StatusCode, T Data) => new ResponseDto<T> { isSuccessful = true, data = Data, statusCode = StatusCode };

        public static ResponseDto<T> Error(int StatusCode, string? Error = null) => new ResponseDto<T> { statusCode = StatusCode, Errors = new List<string> { Error } };

        public static ResponseDto<T> Error(int StatusCode, IEnumerable<string>? Errors = null) => new ResponseDto<T> { statusCode = StatusCode, Errors = Errors };

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
