using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerWithPhp
{
    internal class BasicRequestHandler : IRequestHandler
    {

        public BasicRequestHandler()
        {

        }

        public void HandleRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {

            var response = context.Response;

            string content = "<!DOCTYPE html>\n<html>\n<head>\n<title>Test12</title></head><body>Mic check 1-2</body></html>";

            byte[] contentBytes = Encoding.UTF8.GetBytes(content);


            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "html";
            response.OutputStream.Write(contentBytes, 0, contentBytes.Length);
            response.OutputStream.Close();

        }
    }
}
