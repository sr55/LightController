// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigService.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Text.Json;
using LightController.Win.Demo.Services.Config.Interfaces;
using LightController.Win.Demo.Services.Config.Model;

namespace LightController.Win.Demo.Services.Config
{
    public class ConfigService : IConfigService
    {
        private ConfigOptions options = null;
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public void Init()
        {
            string configFile;
            using (StreamReader reader = new StreamReader("config.json"))
            {
                configFile = reader.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(configFile))
            {
                this.options = JsonSerializer.Deserialize<ConfigOptions>(configFile, jsonOptions);
            }
            else
            {
                this.options = new ConfigOptions("https://10.0.0.2", true, 250);  // Build default settings
            }
        }

        public ConfigOptions GetConfig()
        {
            return this.options;
        }
    }
}
