using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerWithPhp
{

    internal class PhpCgiHandler
    {
        private string _exePath;

        public PhpCgiHandler(string phpCgiExePath)
        {
            _exePath = phpCgiExePath;
        }

        /*Summary: Calls php script via CGI, returns output as header dictionary, body
        Params:
                HttpListenerRequest request : Request found via HttpListenerContext in the listener
                string scriptPath           : Relative path of the script in relation to document root
                string documentRoot         : Document root for the server

        Returns:
                headers     : All php headers. E.g: Content-type, Set-Cookie, etc..
                body        : Body of the php output
        */
        public (Dictionary<string, string> headers, string body) ProcessPHP(HttpListenerRequest request, string scriptPath, string documentRoot)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _exePath,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            //NOTE: Don't 100% understand this - but it spits out an error if I don't include it.
            //Something about security? Not sure how secure it is if I can turn it off like this..
            startInfo.EnvironmentVariables["REDIRECT_STATUS"] = "1";

            // Set CGI environment variables
            startInfo.EnvironmentVariables["DOCUMENT_ROOT"] = documentRoot;
            startInfo.EnvironmentVariables["SCRIPT_FILENAME"] = documentRoot + "\\" + scriptPath;
            startInfo.EnvironmentVariables["SCRIPT_NAME"] = scriptPath;
            startInfo.EnvironmentVariables["REQUEST_METHOD"] = request.HttpMethod;
            startInfo.EnvironmentVariables["QUERY_STRING"] = request.Url.Query.TrimStart('?');
            startInfo.EnvironmentVariables["CONTENT_TYPE"] = request.ContentType ?? string.Empty;
            startInfo.EnvironmentVariables["CONTENT_LENGTH"] = request.ContentLength64 > 0 ? request.ContentLength64.ToString() : string.Empty;
            startInfo.EnvironmentVariables["HTTPS"] = request.IsSecureConnection ? "on" : "off";
            startInfo.EnvironmentVariables["HTTP_USER_AGENT"] = request.UserAgent;
            startInfo.EnvironmentVariables["REQUEST_URI"] = request.RawUrl ?? string.Empty;
            startInfo.EnvironmentVariables["SERVER_PROTOCOL"] = request.ProtocolVersion.ToString();
            startInfo.EnvironmentVariables["SERVER_NAME"] = request.UserHostName ?? "localhost";
            startInfo.EnvironmentVariables["SERVER_PORT"] = request.LocalEndPoint.Port.ToString();
            startInfo.EnvironmentVariables["REMOTE_ADDR"] = request.RemoteEndPoint.Address.ToString();
            startInfo.EnvironmentVariables["REMOTE_PORT"] = request.RemoteEndPoint.Port.ToString();
            startInfo.EnvironmentVariables["GATEWAY_INTERFACE"] = "CGI/1.1";

            using (Process cgi = new Process { StartInfo = startInfo })
            {
                cgi.Start();

                //If POST, write the request body to InputStream
                if (request.HttpMethod == "POST" && request.ContentLength64 > 0)
                {
                    using (StreamWriter writer = cgi.StandardInput)
                    {
                        request.InputStream.CopyTo(writer.BaseStream);
                    }
                }

                string output = cgi.StandardOutput.ReadToEnd();
                string error = cgi.StandardError.ReadToEnd();

                cgi.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"PHP CGI ERROR: {error}");
                }

                Console.WriteLine(output);

                return ParsePhpOutput(output);
            }
        }

        //Splits the raw PhP output string into headers & body
        public (Dictionary<string, string> headers, string body) ParsePhpOutput(string output)
        {
            var headers = new Dictionary<string, string>();
            var bodyBuilder = new StringBuilder();

            using (StringReader reader = new StringReader(output))
            {
                string? line;
                bool isBody = false;

                while ((line = reader.ReadLine()) != null)
                {
                    //Should be 2 newlines delineating headers/body
                    if (string.IsNullOrEmpty(line))
                    {
                        isBody = true;
                        continue;
                    }

                    //If header, split into Key/value
                    if (!isBody)
                    {
                        var headerParts = line.Split(':', 2, StringSplitOptions.TrimEntries);
                        if (headerParts.Length == 2)
                        {
                            headers[headerParts[0]] = headerParts[1];
                        }
                    }
                    //Else, append to body
                    else
                    {
                        bodyBuilder.AppendLine(line);
                    }
                }
            }
            return (headers, bodyBuilder.ToString());
        }
    }
}


