using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }

        static ConfigurationManager()
        {                    
            Configuration = new ConfigurationBuilder().Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true }).Build();
        }
    }
}
