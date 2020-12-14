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
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using LibVLCSharp.Shared;

    using LightController.API;
    using LightController.API.FrameData;
    using LightController.API.Model;

    using NAudio.CoreAudioApi;

    public class Program
    {
        private static MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        private static MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

        // Config
        private static string baseUrl = "http://10.0.100.177";
        private static bool isRGBW = true;
        private static int ledCount = 250;

        // State
        private static int level;
        private static TimeSpan time;
        private static Dictionary<int, int> timeCodes = new Dictionary<int, int>();

        public static async Task Main(string[] args)
        {
            Console.WriteLine("# SMART Led Lights Controller");
            Console.WriteLine("  - Base URL: " + baseUrl);
            Console.WriteLine("  - Using Audio Device: " + device.DeviceFriendlyName);
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
            Carol carol = new Carol(ledCount, isRGBW);

            Thread.Sleep(10000);

            string[] options = new List<string>().ToArray();
            ParseProfile();

            LibVLCSharp.Shared.Core.Initialize();
            using (var libvlc = new LibVLC(false, options))
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "carol.m4a");

                using (var media = new Media(libvlc, new Uri(path)))
                {
                    using (var mediaplayer = new MediaPlayer(media))
                    {
                        mediaplayer.Play();
                        mediaplayer.TimeChanged += Mediaplayer_TimeChanged;    
                        await Task.Delay(500); 

                        while (mediaplayer.IsPlaying)
                        {
                            List<Led> letSet = carol.GetNextFrame(level);
                            realTimeMode.SendFrame(authResponse, letSet);

                            foreach (var record in timeCodes.OrderByDescending(o => o.Key))
                            {
                                if (time.TotalSeconds > record.Key)
                                {
                                    carol.SetPattern(record.Value);
                                    break;
                                }
                            }

                            Thread.Sleep(150);
                        }
                    }
                }
            }

            // Reset when done.
            modeService.SetMode(authResponse, "movie");
            brightness.SetBrightness(authResponse, 90);

            Console.WriteLine(Environment.NewLine + "Press Any Key to exit!");

            Console.Read();
        }

        private static void Mediaplayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            time = TimeSpan.FromMilliseconds(e.Time);
            level = (int)Math.Round(device.AudioMeterInformation.MasterPeakValue * 100, 0);

            Console.WriteLine($"  [{time}]       -  Level: {level}");
        }

        private static void ParseProfile()
        {
            using (StreamReader reader = new StreamReader("carol.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("#"))
                    {
                        continue;
                    }

                    string[] items = line.Split(",");
                    int seconds = int.Parse(items[0].Trim());
                    int level = int.Parse(items[1].Trim());
                    timeCodes.Add(seconds, level);
                }
            }
        }
    }
}
