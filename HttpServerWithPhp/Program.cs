

using HttpServerWithPhp;
using System;
using System.Runtime.CompilerServices;

class Program
{
    private static bool _keepRunning = true;

    static void Main(string[] args)
    {

        string rootDir = $"C:\\Users\\joshr\\source\\repos\\CSHARP\\WebServerWithPhp\\htdocs";
        string phpCgiExe = "C:\\php\\php-cgi.exe";

        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Program._keepRunning = false;
        };


        PhpCgiHandler phpHandler = new PhpCgiHandler(phpCgiExe);
        BasicRequestHandler handler = new BasicRequestHandler(rootDir, phpHandler);

        var server = new HttpServer(13005, "http://localhost:", "127.0.0.1", handler);

        server.Start();
        
        while (Program._keepRunning) { }

        server.Stop();

        Console.WriteLine("Server shutdown complete.");

    }
}