// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using LightController.API;
using LightController.API.Model;
using LightController.Services.Config;
using LightController.Services.Config.Model;
using LightController.Win.Demo.LightModes;

namespace LightController.Win.Demo
{
    public class Program
    {
        // Config
        private static ConfigService configService = new ConfigService();

        public static async Task Main(string[] args)
        {
            configService.Init();
            ConfigOptions options = configService.GetConfig();

            Console.WriteLine("# SMART Led Lights Controller");
            Console.WriteLine("  - Base URL: " + options.LightsUrl);
            Console.WriteLine();

            // Login
            Authentication auth = new Authentication(options.LightsUrl);
            LoginResponse authResponse = auth.Login();

            if (!auth.Verify(authResponse))
            {
                Console.WriteLine(Environment.NewLine + "Press Any Key to exit!");
                Console.Read();
            }

            // Setup Mode
            Mode modeService = new Mode(options.LightsUrl);
            modeService.GetMode(authResponse);
            modeService.SetMode(authResponse, "rt");
            RealTime realTimeMode = new RealTime(options.LightsUrl, 7777, options.LedCount, options.IsRGBWSet);

            // Setup Brightness
            Brightness brightness = new Brightness(options.LightsUrl);
            brightness.GetBrightness(authResponse);
            brightness.SetBrightness(authResponse, 100);

            switch (options.Mode)
            {
                case 1: // A Demo Mode
                    CarolDemoMode carolDemoMode = new CarolDemoMode(options.LedCount, options.IsRGBWSet, realTimeMode);
                    Console.Read();
                    Task t = carolDemoMode.Run(authResponse);
                    t.Wait();
                    break;

                case 2: // Flash each LED to allow mapping
                    MapLinesMode mapTreeLightsMode = new MapLinesMode(options.LedCount, realTimeMode);
                    Console.Read();
                    Task t2 = mapTreeLightsMode.Run(authResponse);
                    t2.Wait();
                    break;
            }

            // Reset when done.
            modeService.SetMode(authResponse, "movie");
            brightness.SetBrightness(authResponse, 80);

            Console.WriteLine(Environment.NewLine + "Press Any Key to exit!");

            Console.Read();
        }
    }
}
