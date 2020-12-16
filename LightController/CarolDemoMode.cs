// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarolDemoMode.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Carol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;

    using LibVLCSharp.Shared;

    using LightController.API;
    using LightController.API.FrameData;
    using LightController.API.Model;
    using LightController.Helpers;

    using NAudio.CoreAudioApi;

    using Timer = System.Timers.Timer;

    public class CarolDemoMode
    {
        private static MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        private static MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

        private static Led warmWhite = new Led(0, 0, 0, 255);
        private static Led coolWhite = new Led(255, 255, 255, 0);
        private static Led red = new Led(255, 0, 0, 0);
        private static Led green = new Led(0, 255, 0, 0);
        private static Led blue = new Led(0, 0, 255, 0);
        private static Led yellow = new Led(255, 255, 0, 0);
        private static Led off = new Led(0, 0, 0, 0);

        private readonly int ledCount;
        private readonly bool isRgbw;

        private readonly RealTime realTime;

        // State
        private Dictionary<int, int> timeCodes = new Dictionary<int, int>();

        private int colourPattern = 3;

        public CarolDemoMode(int ledCount, bool isRGBW, RealTime realTime)
        {
            this.ledCount = ledCount;
            this.isRgbw = isRGBW;
            this.realTime = realTime;
        }

        public async Task Run(LoginResponse authResponse)
        {
            Console.WriteLine("  - Using Audio Device: " + device.DeviceFriendlyName);

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
                        await Task.Delay(500);

                        while (mediaplayer.IsPlaying)
                        {
                            TimeSpan totalSeconds = TimeSpan.FromMilliseconds(mediaplayer.Time);
                            int level = (int)Math.Round(device.AudioMeterInformation.MasterPeakValue * 50, 0);

                            ConsoleOutput.WriteLine(string.Format("  - {0} with Level {1}", totalSeconds, level), ConsoleColor.White);

                            foreach (var record in this.timeCodes.OrderByDescending(o => o.Key))
                            {
                                if (totalSeconds.TotalSeconds > record.Key)
                                {
                                    this.SetPattern(record.Value);
                                    break;
                                }
                            }

                            List<Led> letSet = this.GetNextFrame(level);
                            this.realTime.SendFrame(authResponse, letSet);

                            Thread.Sleep(150);
                        }
                    }
                }
            }
        }

        private void ParseProfile()
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
        
        private List<Led> GetNextFrame(int ledCount)
        {
            List<Led> frame = this.GenerateFullFrame(ledCount);
            return frame;
        }

        private void SetPattern(int i)
        {
            this.colourPattern = i;
        }

        private List<Led> GenerateFullFrame(int ledsToLight)
        {
            List<Led> leds = new List<Led>();

            HashSet<int> randomLedIndexes = this.RandomLeds(ledsToLight);

            for (int i = 0; i < this.ledCount; i++)
            {
                if (this.IsLedEnabled(randomLedIndexes, i))
                {
                    leds.Add(this.PickColourPattern());
                }
                else
                {
                    leds.Add(off);
                }
            }

            return leds;
        }

        private Led PickColourPattern()
        {
            switch (this.colourPattern)
            {
                case 1:
                    return this.PickWhite();
                case 2:
                    return this.PickBlue();
                case 3:
                    return this.PickBlueWhite();
                case 4:
                    return this.PickRandomColour();
                case 5:
                    return this.PickWarmWhite();
            }

            return off;
        }

        private Led PickWhite()
        {
            return coolWhite;
        }

        private Led PickWarmWhite()
        {
            return warmWhite;
        }

        private Led PickBlue()
        {
            return blue;
        }

        private Led PickBlueWhite()
        {
            Random rnd = new Random();

            int randomNumber = rnd.Next(1, 3);

            switch (randomNumber)
            {
                case 1:
                    return blue;
                case 2:
                    return coolWhite;
            }

            return off;
        }

        private Led PickRandomColour()
        {
            Random rnd = new Random();

            int randomNumber = rnd.Next(1, 5);

            switch (randomNumber)
            {
                case 1:
                    return red;
                case 2:
                    return green;
                case 3:
                    return blue;
                case 4:
                    return yellow;
            }

            return off;
        }

        private bool IsLedEnabled(HashSet<int> ledsOn, int index)
        {
            if (ledsOn.Contains(index))
            {
                return true;
            }

            return false;
        }

        private HashSet<int> RandomLeds(int ledsToTurnOn)
        {
            Random rnd = new Random();
            HashSet<int> ledIndexes = new HashSet<int>();

            while (ledIndexes.Count != ledsToTurnOn)
            {
                int randomNumber = rnd.Next(1, this.ledCount + 1);
                ledIndexes.Add(randomNumber);
            }

            return ledIndexes;
        }
    }
}
