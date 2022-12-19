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
using LightController.Win.Demo.LightModes.Model;
using LightController.Win.Demo.LightModes.Patterns;
using LightController.Win.Demo.Services.Config.Model;
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

        private readonly int ledCount;
        private readonly bool isRgbw;

        private readonly RealTime realTime;

        // State
        private Dictionary<double, CarolModel> timeCodes = new Dictionary<double, CarolModel>();

        public CarolDemoMode(int ledCount, bool isRGBW, RealTime realTime)
        {
            this.ledCount = ledCount;
            this.isRgbw = isRGBW;
            this.realTime = realTime;
        }

        private void RenderWaveForm(string path)
        {
            var settings = new StandardWaveFormRendererSettings();
            settings.Width = 768;
            settings.TopHeight = 64;
            settings.BottomHeight = 64;

            var averagePeakProvider = new AveragePeakProvider(4);

            WaveFormRenderer renderer = new WaveFormRenderer();
            string filename = Path.GetFileNameWithoutExtension(path);

            try
            {
                Image image = renderer.Render(new WaveFileReader(path), averagePeakProvider, settings);
                image.Save($"{filename}.png", ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to generate waveform. " + ex);
            }
        }

        public async Task Run(LoginResponse authResponse, ConfigOptions config)
        {
            Console.WriteLine("  - Using Audio Device: " + device.DeviceFriendlyName);

            this.ParseProfile(config);

            string directory = Path.GetDirectoryName(config.MusicFile) ?? Directory.GetCurrentDirectory();
            string path = Path.Combine(directory, config.MusicFile);

            RenderWaveForm(path);

            using (var audioFile = new AudioFileReader(path))
            {
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    int frameCounter = 0;

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        TimeSpan totalSeconds = outputDevice.GetPositionTimeSpan();
                        int level = (int)Math.Round(device.AudioMeterInformation.MasterPeakValue * 100, 0);
                        level = Math.Min(this.GetMax(level), level);

                        ConsoleOutput.WriteLine(string.Format("  - {0} with Level {1}", totalSeconds, level), ConsoleColor.White);

                        int pattern = 1;
                        foreach (var record in this.timeCodes.OrderByDescending(o => o.Key))
                        {
                            if (totalSeconds.TotalSeconds > record.Key)
                            {
                                if (record.Value.IntensityOverride.HasValue)
                                {
                                    level = record.Value.IntensityOverride.Value * 2;
                                }

                                pattern = record.Value.Mode;
                                break;
                            }
                        }

                        List<Led> letSet = RandomFrameGenerator.GenerateFullFrame(level, this.ledCount, pattern);

                        Thread.Sleep(8);
                        frameCounter += 1;
                        if (config.SendHttpFrames)
                        {
                            this.realTime.SendFrameHttp(authResponse, letSet, config.DebugMode, frameCounter);
                        }
                        else
                        {
                            this.realTime.SendFrame(authResponse, letSet, config.DebugMode, frameCounter);
                        }
                    }
                }
            }
        }

        private void ParseProfile(ConfigOptions config)
        {
            using (StreamReader reader = new StreamReader(config.CarolPatternFile))
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
    }
}
