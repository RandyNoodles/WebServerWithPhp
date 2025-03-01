using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace HttpServerWithPhp
{
    internal class HttpServer
    {
        private ConcurrentDictionary<Task, byte> _runningTasks;

        public int Port { get; set; }
        public string UriPrefix { get; set; }
        private HttpListener _listener;

        private CancellationTokenSource _cts;
        private bool _isRunning;

        public HttpServer(int port, string urlPrefix, string iPAddress, CancellationToken token)
        {
            Port = port;
            UriPrefix = urlPrefix;
            _listener = new HttpListener();
            _runningTasks = new ConcurrentDictionary<Task, byte>();
            _cts = new CancellationTokenSource();
            _isRunning = false;
        }

        public async void Start()
        {
            if (_isRunning) { return; }
            
            while (_isRunning)
            {
                try
                {
                    _listener.Prefixes.Add(UriPrefix + Port.ToString() + "/");
                    _listener.Start();
                    _isRunning = true;
                    var context = await _listener.GetContextAsync();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _isRunning = false;
                    _listener.Stop();
                    return;
                }
            }
            


        }
        public void Stop()
        {
            _cts.Cancel();
            _listener.Stop();
        }
    }
}
