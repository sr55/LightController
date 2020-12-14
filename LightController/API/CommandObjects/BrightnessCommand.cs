// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrightnessCommand.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the BrightnessCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API.CommandObjects
{
    public class BrightnessCommand
    {
        public BrightnessCommand(string mode, string type, int value)
        {
            this.Mode = mode;
            this.Type = type;
            this.Value = value;
        }

        public string Mode { get; set; }

        public string Type { get; set; }

        public int Value { get; set; }
    }
}
