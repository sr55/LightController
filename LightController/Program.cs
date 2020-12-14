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
    using System.Collections.Generic;

    using LightController.API;
    using LightController.API.FrameData;
    using LightController.API.Model;


    public class Program
    {
        private static string baseUrl = "http://10.0.100.177";

        private static bool isRGBW = true;

        private static int ledCount = 250;

        public static void Main(string[] args)
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
            brightness.SetBrightness(authResponse, 95);

            // Demo Real Time Mode
            RealTime realTimeMode = new RealTime(baseUrl, 7777, ledCount, isRGBW);

            int i = 0;
            while (i != 100)
            {
                List<Led> letSet = realTimeMode.GenerateSampleLedSet();
                realTimeMode.SendFrame(authResponse, letSet);
                i = i + 1;
            }

            // Reset when done.
            modeService.SetMode(authResponse, "movie");

            Console.WriteLine(Environment.NewLine + "Press Any Key to exit!");

            Console.Read();
        }
    }
}
