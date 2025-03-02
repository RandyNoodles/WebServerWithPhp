using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerWithPhp
{
    internal class BasicRequestHandler : IRequestHandler
    {

        private string _documentRoot;
        private PhpCgiHandler _phpHandler;
        private bool _routingEnabled;
        private Dictionary<string, string> _routes;

        //Without routing
        public BasicRequestHandler(string rootDir, PhpCgiHandler phpHandler)
        {
            _documentRoot = rootDir;
            _phpHandler = phpHandler;
            _routingEnabled = false;
            _routes = new Dictionary<string, string>();
        }

        //With routing
        public BasicRequestHandler(string rootDir, PhpCgiHandler phpHandler, Dictionary<string, string> routes)
        {
            _documentRoot = rootDir;
            _phpHandler = phpHandler;
            _routingEnabled = true;
            _routes = routes;
        }

        public void HandleRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            var response = context.Response;
            var request = context.Request;

            //Not sure if request.RawUrl is correct..
            string requestedResource = _routingEnabled ? GetResourcePath(request.Url.AbsolutePath) : request.Url.AbsolutePath;

            string fullPath = Path.Combine(_documentRoot, requestedResource);

            //Check if resource exists in rootDir
            if ( requestedResource == string.Empty || !File.Exists(fullPath)){
                response.StatusCode = 404;
                response.StatusDescription = "Not found";
                SendResponse(response, Encoding.UTF8.GetBytes("404 - Not Found"), "txt/plain");
                return;
            }

            //Check what kind of resource they want - php, image, etc..
            string fileExtension = Path.GetExtension(fullPath).ToLowerInvariant();
            string mimeType = "text/html";


            //If Php, send to _phpHandler
            if (fileExtension == ".php")
            {
                var (headers, body) = _phpHandler.ProcessPHP(context.Request, "cgi_debug.php", _documentRoot);

                foreach (var item in headers)
                {
                    if(item.Key == "Content-type")
                    {
                        mimeType = item.Key;//In case of non-standard Content-type
                    }
                    response.AddHeader(item.Key, item.Value);
                }


                //Send response
                byte[] contentBytes = Encoding.UTF8.GetBytes(body);
                SendResponse(response, contentBytes, mimeType);
                return;
            }
            
            //Only GET is allowed for static resources.
            if(request.HttpMethod != "GET")
            {
                response.StatusCode = 405;//Method not allowed
                response.StatusDescription = $"{request.HttpMethod} is not supported for static resources";
                SendResponse(response, Encoding.UTF8.GetBytes("405 - Method Not Allowed"), "text/plain");
                return;
            }

            //Serve static files (GET only)
            byte[] content = LoadRequestedResource(fullPath);
            mimeType = GetMimeType(fileExtension);
            SendResponse(response, content, mimeType);


        }

        public string GetResourcePath(string route)
        {
            //See if the key exists
            //If it exists, return the value
            return _routes.TryGetValue(route, out string mappedPath) ? mappedPath : "";
        }

        private byte[] LoadRequestedResource(string requestedResource)
        {
            return null;
        }

        private string GetMimeType(string fileExtension)
        {
            return fileExtension switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".ico" => "image/x-icon",
                ".txt" => "text/plain",
                ".json" => "application/json",
                ".xml" => "application/xml",
                _ => "application/octet-stream",
            };
        }


        private void SendResponse(HttpListenerResponse response, byte[] content, string contentType)
        {
            response.ContentType = contentType;
            response.ContentLength64 = content.Length;

            try
            {
                response.OutputStream.Write(content, 0, content.Length);
            }
            catch (ObjectDisposedException)
            {
                // Server shutdown, nothing to do
            }
            finally
            {
                response.OutputStream.Close();
            }
        }
    }
}
