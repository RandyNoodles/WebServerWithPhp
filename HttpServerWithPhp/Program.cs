

using HttpServerWithPhp;
using System;
using System.Runtime.CompilerServices;

class Program
{
    private static bool _keepRunning = true;

    static void Main(string[] args)
    {

        //Soultion-level folder
        string solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;

        //Document root
        string relativeDocumentRoot = Path.Combine(solutionDir, "htdocs");
        
        //Php exe
        string phpCgiExe = "C:\\php\\php-cgi.exe";


        //Intercept ctrl+c
        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Program._keepRunning = false;
        };


        PhpCgiHandler phpHandler = new PhpCgiHandler(phpCgiExe);
        BasicRequestHandler handler = new BasicRequestHandler(relativeDocumentRoot, phpHandler);

        var server = new HttpServer(13005, "http://localhost:", "127.0.0.1", handler);

        server.Start();
        
        while (Program._keepRunning) { }

        server.Stop();

        Console.WriteLine("Server shutdown complete.");

    }
}