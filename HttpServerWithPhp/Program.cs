

using HttpServerWithPhp;
using System.Runtime.CompilerServices;

class Program
{
    private static bool _keepRunning = true;

    static void Main(string[] args)
    {


        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Program._keepRunning = false;
        };

        BasicRequestHandler handler = new BasicRequestHandler();

        var server = new HttpServer(13005, "http://localhost:", "127.0.0.1", handler);

        server.Start();
        
        while (Program._keepRunning) { }

        server.Stop();

        Console.WriteLine("Server shutdown complete.");

    }
}