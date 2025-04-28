using System.Net;

namespace Ecommerce.Frontend.Models.Common
{
    public class Response<T>
    {
        public Response()
        {
        }

        public Response(T data, string message = null)
        {
            StatusCode = System.Net.HttpStatusCode.OK;
            Succeeded = true;
            Message = message ?? "Success";
            Data = data;
        }

        public Response(string message, bool succeeded = false, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            StatusCode = statusCode;
            Succeeded = succeeded;
            Message = message;
        }

        public HttpStatusCode StatusCode { get; set; }
        public object Meta { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }
    }
}