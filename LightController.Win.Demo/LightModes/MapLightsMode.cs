// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarolDemoMode.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Blink each LED on both strands to allow mapping of their position.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightController.API;
using LightController.API.FrameData;
using LightController.API.Model;

namespace LightController.Win.Demo.LightModes
{
    public class MapLinesMode
    {
        private static Led coolWhite = new Led(0, 0, 0, 255);
        private static Led off = new Led(0, 0, 0, 0);

        private readonly int ledCount;

        private readonly RealTime realTime;


        public MapLinesMode(int ledCount, RealTime realTime)
        {
            this.ledCount = ledCount;
            this.realTime = realTime;
        }

        public async Task RunManual(LoginResponse authResponse, bool isDebugMode)
        {

            for (int i = 0; i < this.ledCount; i++)
            {
                List<Led> letSet = this.BuildFrame(i);
                this.realTime.SendFrame(authResponse, letSet, isDebugMode, i);
                Console.Read();
            }
        }

        public async Task Run(LoginResponse authResponse, bool isDebugMode)
        {

            for (int i = 0; i < this.ledCount; i++)
            {
                List<Led> letSet = this.BuildFrame(i);
                this.realTime.SendFrame(authResponse, letSet, isDebugMode, i);
                Thread.Sleep(20);
            }
        }

        private List<Led> BuildFrame(int indexToLight)
        {
            List<Led> ledList = new List<Led>();

            for (int i = 0; i < this.ledCount; i++)
            {
                ledList.Add(i == indexToLight ? coolWhite : off);
            }

            return ledList;
        }
    }
}
