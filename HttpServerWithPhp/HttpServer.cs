using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace HttpServerWithPhp
{
    internal class HttpServer
    {
        public int Port { get; set; }
        public string UrlPrefix { get; set; }
        private HttpListener _listener;
        private IRequestHandler _requestHandler;

        private ConcurrentDictionary<Task, byte> _runningTasks;

        private CancellationTokenSource _cts;

        public HttpServer(int port, string urlPrefix, string iPAddress, IRequestHandler requestHandler)
        {
            Port = port;
            UrlPrefix = urlPrefix;
            _listener = new HttpListener();
            _cts = new CancellationTokenSource();
            _requestHandler = requestHandler;
            _runningTasks = new ConcurrentDictionary<Task, byte>();
        }

        public async Task Start()
        {
            
            
            try
            {
                _listener.Prefixes.Add(UrlPrefix + Port.ToString() + "/");
                _listener.Start();


                while (!_cts.IsCancellationRequested)
                {
                    var context = await _listener.GetContextAsync();

                    Console.WriteLine("Request received");

                    var task = Task.Run(() => _requestHandler.HandleRequest(context, _cts.Token));
                    
                    //Log task in dictionary
                    _runningTasks[task] = 0;
                    //Remove once complete
                    task.ContinueWith(t => _runningTasks.TryRemove(t, out _));
                }
            }
            catch (Exception ex) when (!_cts.IsCancellationRequested)
            {
                Console.WriteLine(ex.Message);
                _cts.Cancel();
                _listener.Stop();
                return;
            }
            
            


        }
        public void Stop()
        {
            _cts.Cancel();
            _listener.Stop();

            Task.WaitAll(_runningTasks.Keys.ToArray<Task>(), 5000);
            Console.WriteLine("All running tasks completed.");
        }

    }
}
