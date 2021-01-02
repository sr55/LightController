// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the UDPHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Threading.Tasks;
using LightController.API;
using LightController.API.Model;

namespace LightController.Win.Demo
{
    public class Program
    {
        // Config
        private static string baseUrl = "http://10.0.100.177";
        private static bool isRGBW = true;
        private static int ledCount = 250;
        
        public static async Task Main(string[] args)
        {
            Console.WriteLine("# SMART Led Lights Controller");
            Console.WriteLine("  - Base URL: " + baseUrl);
            Console.WriteLine();

            // Login
            Authentication auth = new Authentication(baseUrl);
            LoginResponse authResponse = auth.Login();

            if (!auth.Verify(authResponse))
            {
                Console.WriteLine(Environment.NewLine + "Press Any Key to exit!");
                Console.Read();
            }

            // Setup Mode
            Mode modeService = new Mode(baseUrl);
            modeService.GetMode(authResponse);
            modeService.SetMode(authResponse, "rt");

            // Setup Brightness
            Brightness brightness = new Brightness(baseUrl);
            brightness.GetBrightness(authResponse);
            brightness.SetBrightness(authResponse, 50);

            // Demo Real Time Mode
            RealTime realTimeMode = new RealTime(baseUrl, 7777, ledCount, isRGBW);
            CarolDemoMode carol = new CarolDemoMode(ledCount, isRGBW, realTimeMode);

            Console.Read();

            Task t = carol.Run(authResponse);
            t.Wait();

            // Reset when done.
            modeService.SetMode(authResponse, "movie");
            brightness.SetBrightness(authResponse, 90);

            Console.WriteLine(Environment.NewLine + "Press Any Key to exit!");

            Console.Read();
        }
    }
}
