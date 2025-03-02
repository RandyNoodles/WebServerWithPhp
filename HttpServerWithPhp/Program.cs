

using HttpServerWithPhp;
using HttpServerWithPhp.RequestHandling;
using HttpServerWithPhp.Config;
using System;
using System.Runtime.CompilerServices;

class Program
{
    private static bool _keepRunning = true;

    static void Main(string[] args)
    {
        
        //Soultion-level folder
        string solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;

        string configPath = Path.Combine(solutionDir, "HttpServerWithPhp\\config.json");

        Config config = null;

        //Load config
        try
        {
            config = Config.LoadConfig(configPath);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        //Convert document root to absolute
        string absoluteDocumentRoot = Path.Combine(solutionDir, config.PhpSettings.DocumentRoot);


        var phpHandler = new PhpCgiHandler(config.PhpSettings.PhpCgiPath);
        var clientHandler = new BasicRequestHandler(absoluteDocumentRoot, phpHandler, config.Routing.RouteDictionary, config.Routing.Enabled);

        var server = new HttpServer(config.ServerSettings.Port, config.ServerSettings.Host, clientHandler);


        //Intercept ctrl+c -> use it to shutdown server & running tasks gracefully.
        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Program._keepRunning = false;
        };

        server.Start();
        
        while (Program._keepRunning) { }

        server.Stop();

        Console.WriteLine("Server shutdown complete.");

    }
}