using HttpServerWithPhp.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace HttpServerWithPhp.Config
{
    public class Config
    {
        public ServerConfig ServerSettings { get; set; } = new();
        public PhpConfig PhpSettings { get; set; } = new();
        public RoutingConfig Routing { get; set; } = new();

        public static Config LoadConfig(string path) 
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} was not found");
            }

            string json = File.ReadAllText(path);
            var config = JsonSerializer.Deserialize<Config>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
            });

            if (config == null)
            {
                throw new FileLoadException($"{path} contents were empty or invalid.");
            }

            Dictionary<string, string> routeMappings = new Dictionary<string, string>();
            if (config.Routing.Enabled)
            {
                foreach(var route in config.Routing.Routes)
                {
                    config.Routing.RouteDictionary[route.Route] = route.Resource;
                }
            }

            return config;
        }
    }

    public class ServerConfig
    {
        public string Host { get; set; } = "http://localhost";
        public int Port { get; set; } = 8080;
    }

    public class PhpConfig
    {
        public string PhpCgiPath { get; set; } = "";
        public string DocumentRoot { get; set; } = "";
    }

    public class RoutingConfig
    {
        public bool Enabled { get; set; } = false;
        public List<RouteMapping> Routes { get; set; } = new();
        public Dictionary<string, string> RouteDictionary = new();

    }

    public class RouteMapping
    {
        public string Route { get; set; } = "";
        public string Resource { get; set; } = "";
    }
}
