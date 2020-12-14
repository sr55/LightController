// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Carol.cs" company="LightController Project (http://github.com/sr55/LightController)">
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

    using LightController.API.FrameData;

    public class Carol
    {
        private static Led warmWhite = new Led(0, 0, 0, 255);
        private static Led coolWhite = new Led(255, 255, 255, 0);
        private static Led red = new Led(255, 0, 0, 0);
        private static Led green = new Led(0, 255, 0, 0);
        private static Led blue = new Led(0, 0, 255, 0);
        private static Led yellow = new Led(255, 255, 0, 0);
        private static Led off = new Led(0, 0, 0, 0);

        private readonly int ledCount;
        private readonly bool isRgbw;

        private int colourPattern = 3;

        public Carol(int ledCount, bool isRGBW)
        {
            this.ledCount = ledCount;
            this.isRgbw = isRGBW;
        }

        public List<Led> GetNextFrame(int ledCount)
        {
            List<Led> frame = this.GenerateFullFrame(ledCount);
            return frame;
        }

        public void SetPattern(int i)
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

        public Led PickColourPattern()
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

        public Led PickWhite()
        {
            return coolWhite;
        }

        public Led PickWarmWhite()
        {
            return warmWhite;
        }

        public Led PickBlue()
        {
            return blue;
        }

        public Led PickBlueWhite()
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
        
        public Led PickRandomColour()
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
