// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Led.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Led type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API.FrameData
{
    public class Led
    {
        public Led(int red, int green, int blue, int white)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.White = white;
        }

        public int Red { get; set; }

        public int Green { get; set; }

        public int Blue { get; set; }

        public int White { get; set; }
    }
}
