// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarolDemoMode.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Carol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightController.API;
using LightController.API.FrameData;
using LightController.API.Model;
using LightController.Helpers;
using LightController.Services.Config.Model;
using LightController.Win.Demo.LightModes.Model;
using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.WaveFormRenderer;

namespace LightController.Win.Demo.LightModes
{
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
        private static Led Purple = new Led(128, 0, 128, 0);
        private static Led off = new Led(0, 0, 0, 0);

        private readonly int ledCount;
        private readonly bool isRgbw;

        private readonly RealTime realTime;

        // State
        private Dictionary<double, CarolModel> timeCodes = new Dictionary<double, CarolModel>();

        private int colourPattern = 3;

        public CarolDemoMode(int ledCount, bool isRGBW, RealTime realTime)
        {
            this.ledCount = ledCount;
            this.isRgbw = isRGBW;
            this.realTime = realTime;
        }

        private void RenderWaveForm(string path)
        {
            var settings = new StandardWaveFormRendererSettings();
            settings.Width = 840;
            settings.TopHeight = 96;
            settings.BottomHeight = 96;

            var averagePeakProvider = new AveragePeakProvider(4);

            WaveFormRenderer renderer = new WaveFormRenderer();
            Image image = renderer.Render(new WaveFileReader(path), averagePeakProvider, settings);

            string filename = Path.GetFileNameWithoutExtension(path);

            image.Save(string.Format("{0}.png", filename), ImageFormat.Png);
        }

        public async Task Run(LoginResponse authResponse, ConfigOptions config)
        {
            Console.WriteLine("  - Using Audio Device: " + device.DeviceFriendlyName);

            this.ParseProfile();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "carol.wav");

            RenderWaveForm(path);

            using (var audioFile = new AudioFileReader(path))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    TimeSpan totalSeconds = outputDevice.GetPositionTimeSpan();
                    int level = (int)Math.Round(device.AudioMeterInformation.MasterPeakValue * 100, 0);
                    level = Math.Min(this.GetMax(level), level);

                    ConsoleOutput.WriteLine(string.Format("  - {0} with Level {1}", totalSeconds, level), ConsoleColor.White);

                    foreach (var record in this.timeCodes.OrderByDescending(o => o.Key))
                    {
                        if (totalSeconds.TotalSeconds > record.Key)
                        {
                            if (record.Value.IntensityOverride.HasValue)
                            {
                                level = record.Value.IntensityOverride.Value * 2;
                            }

                            this.SetPattern(record.Value.Mode);
                            break;
                        }
                    }

                    List<Led> letSet = this.GetNextFrame(level);

                    if (config.SendHttpFrames)
                    {
                        this.realTime.SendFrameHttp(authResponse, letSet);
                    }
                    else
                    {
                        this.realTime.SendFrame(authResponse, letSet);
                    }

                    Thread.Sleep(70);
                }
            }

        }

        private void ParseProfile()
        {
            using (StreamReader reader = new StreamReader("carol2.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("#"))
                    {
                        continue;
                    }

                    string[] items = line.Split(",");
                    double seconds = double.Parse(items[0].Trim());
                    int mode = int.Parse(items[1].Trim());
                    int levelOveride = int.Parse(items[2].Trim());

                    CarolModel carolMode = new CarolModel();
                    carolMode.TimeCode = seconds;
                    carolMode.Mode = mode;
                    carolMode.IntensityOverride = levelOveride > 0 ? levelOveride : null;

                    this.timeCodes.Add(seconds, carolMode);
                }
            }
        }

        private int GetMax(int level)
        {
            switch (level)
            {
                case 1:
                    return 10;
                case 2:
                    return 20;

                case 3:
                    return 40;

                case 4:
                    return 60;

                case 5:
                    return 75;

                case 6:
                    return 100;

                default:
                    return 100;
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
                leds.Add(this.IsLedEnabled(randomLedIndexes, i) ? this.PickColourPattern() : off);
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
                    return this.PickPurpleWhite();
                case 5:
                    return this.PickRedYellow();
                case 6:
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

        private Led PickPurpleWhite()
        {
            Random rnd = new Random();

            int randomNumber = rnd.Next(1, 3);

            switch (randomNumber)
            {
                case 1:
                    return Purple;
                case 2:
                    return coolWhite;
            }

            return off;
        }


        private Led PickRedYellow()
        {
            Random rnd = new Random();

            int randomNumber = rnd.Next(1, 3);

            switch (randomNumber)
            {
                case 1:
                    return red;
                case 2:
                    return yellow;
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
