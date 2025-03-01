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

        private string _documentRoot;
        private PhpCgiHandler _phpHandler;


        public BasicRequestHandler(string rootDir, PhpCgiHandler phpHandler)
        {
            _documentRoot = rootDir;
            _phpHandler = phpHandler;
        }

        public void HandleRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {

            var response = context.Response;

            var (headers, body) = _phpHandler.ProcessPHP(context.Request, "cgi_debug.php", _documentRoot);

            foreach (var item in headers)
            {
                response.AddHeader(item.Key, item.Value);
            }


            byte[] contentBytes = Encoding.UTF8.GetBytes(body);

            try
            {
                response.OutputStream.Write(contentBytes, 0, contentBytes.Length);
                response.OutputStream.Close();
            }
            catch (ObjectDisposedException e)
            {
                //Server was shutdown
                return;
            }
        }
    }
}
