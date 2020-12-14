// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController
{
    using System;
    using System.Threading;

    using LightController.API;
    using LightController.API.Model;

    public class Program
    {
        private static string baseUrl = "http://10.0.100.177";

        static void Main(string[] args)
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
            modeService.SetMode(authResponse, "off");

            modeService.SetMode(authResponse, "rt");


            // Setup Brightness
            Brightness brightness = new Brightness(baseUrl);
            brightness.GetBrightness(authResponse);
            brightness.SetBrightness(authResponse, 95);
            Thread.Sleep(1000);

            RealTime realTimeMode = new RealTime(baseUrl, 7777, 250);

            while (true)
            {
                realTimeMode.SendFrame(authResponse);
            }

            Console.WriteLine(Environment.NewLine + "Press Any Key to exit!");

            Console.Read();
        }
    }
}
