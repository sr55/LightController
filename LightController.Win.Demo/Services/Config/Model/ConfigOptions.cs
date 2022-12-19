// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigOptions.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.Win.Demo.Services.Config.Model
{
    public class ConfigOptions
    {
        public ConfigOptions()
        {
        }

        public ConfigOptions(string lightsUrl, bool isRgbwSet, int setLedCount)
        {
            this.LightsUrl = lightsUrl;
            this.IsRGBWSet = isRgbwSet;
            this.LedCount = setLedCount;
            this.Mode = 1;
        }

        public string LightsUrl { get; set; }
        public bool IsRGBWSet { get; set; }
        public int LedCount { get; set; }

        public int Mode { get; set; }

        public bool SendHttpFrames { get; set; }

        public bool DebugMode { get; set; }
        public string MusicFile { get; set; }
        public string CarolPatternFile { get; set; }
    }
}
