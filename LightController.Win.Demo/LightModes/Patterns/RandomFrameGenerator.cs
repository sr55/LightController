// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatternHelper.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LightController.API.FrameData;
using System;
using System.Collections.Generic;

namespace LightController.Win.Demo.LightModes.Patterns
{
    public class RandomFrameGenerator
    {
        private static Led warmWhite = new Led(0, 0, 0, 255);
        private static Led coolWhite = new Led(255, 255, 255, 0);
        private static Led red = new Led(255, 0, 0, 0);
        private static Led green = new Led(0, 255, 0, 0);
        private static Led blue = new Led(0, 0, 255, 0);
        private static Led yellow = new Led(255, 255, 0, 0);
        private static Led Purple = new Led(128, 0, 128, 0);
        private static Led off = new Led(0, 0, 0, 0);


        public static List<Led> GenerateFullFrame(int ledsToLight, int ledCount, int pattern)
        {
            List<Led> leds = new List<Led>();

            HashSet<int> randomLedIndexes = RandomLeds(ledsToLight, ledCount);

            for (int i = 0; i < ledCount; i++)
            {
                leds.Add(IsLedEnabled(randomLedIndexes, i) ? PickColourPattern(pattern) : off);
            }

            return leds;
        }

        
        public static bool IsLedEnabled(HashSet<int> ledsOn, int index)
        {
            if (ledsOn.Contains(index))
            {
                return true;
            }

            return false;
        }

        public static HashSet<int> RandomLeds(int ledsToTurnOn, int ledCount)
        {
            Random rnd = new Random();
            HashSet<int> ledIndexes = new HashSet<int>();

            while (ledIndexes.Count != ledsToTurnOn)
            {
                int randomNumber = rnd.Next(1, ledCount + 1);
                ledIndexes.Add(randomNumber);
            }

            return ledIndexes;
        }

        public static Led PickColourPattern(int colourPattern)
        {
            switch (colourPattern)
            {
                case 1:
                    return PickWhite();
                case 2:
                    return PickBlue();
                case 3:
                    return PickBlueWhite();
                case 4:
                    return PickPurpleWhite();
                case 5:
                    return PickRedYellow();
                case 6:
                    return PickWarmWhite();
            }

            return off;
        }

        private static Led PickWhite()
        {
            return coolWhite;
        }

        private static Led PickWarmWhite()
        {
            return warmWhite;
        }

        private static Led PickBlue()
        {
            return blue;
        }

        private static Led PickBlueWhite()
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

        private static Led PickPurpleWhite()
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


        private static Led PickRedYellow()
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

        private static Led PickRandomColour()
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
    }
}
