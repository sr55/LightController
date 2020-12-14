// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealTime.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the RealTime type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using LightController.API.FrameData;
    using LightController.API.Model;
    using LightController.Helpers;
    using LightController.UDP;

    public class RealTime : UDPHelper
    {
        private readonly int port;

        private readonly int ledCount;

        private readonly bool isRgbw;

        private readonly string ipAddress;

        public RealTime(string baseUrl, int port, int ledCount, bool isRgbw)
        {
            this.port = port;
            this.ledCount = ledCount;
            this.isRgbw = isRgbw;
            this.ipAddress = baseUrl.Replace("http://", string.Empty, StringComparison.InvariantCultureIgnoreCase);
        }

        public void SendFrame(LoginResponse auth, List<Led> ledSet)
        {
            ConsoleOutput.WriteLine("  - Sending Frame", ConsoleColor.Yellow);
            
            byte[] frame = GenerateFrame(auth, ledSet);
            UDPSendFrame(auth, frame, this.ipAddress, this.port);

            Thread.Sleep(250);
        }

        public List<Led> GenerateSampleLedSet()
        {
            Random rnd = new Random();

            List<Led> leds = new List<Led>();

            for (int i = 0; i < this.ledCount; i++)
            {
                int randomNumber = rnd.Next(1, 4);

                Led led;
                switch (randomNumber)
                {
                    case 1:
                        led = new Led(255, 0, 0, 0);
                        break;
                    case 2:
                        led = new Led(0, 255, 0, 0);
                        break;
                    case 3:
                        led = new Led(0, 0, 255, 0);
                        break;
                    default:
                        led = new Led(0, 0, 0, 255);
                        break;
                }

                leds.Add(led);
            }
           
            return leds;
        }


        private byte[] GenerateFrame(LoginResponse response, List<Led> ledSet)
        {
            int byteSize = (this.ledCount * 4) + 10;
            byte[] frame = new byte[byteSize];

            frame[0] = 0x01;

            byte[] token = Convert.FromBase64String(response.Authentication_token);

            Buffer.BlockCopy(token, 0, frame, 1, token.Length);

            frame[9] = Convert.ToByte(this.ledCount);

            int bytePosition = 10;
            foreach (Led led in ledSet)
            {
                byte[] ledColour = this.GenerateLedColourPacket(led.White, led.Red, led.Green, led.Blue);

                Buffer.BlockCopy(ledColour, 0, frame, bytePosition, ledColour.Length);
                bytePosition = bytePosition + 4;
            }

            return frame;
        }

        private byte[] GenerateLedColourPacket(int w, int r, int g, int b)
        {
            if (this.isRgbw)
            {
                byte[] led = new byte[4];
                led[0] = Convert.ToByte(w);
                led[1] = Convert.ToByte(r);
                led[2] = Convert.ToByte(g);
                led[3] = Convert.ToByte(b);
                return led;
            }
            else
            {
                byte[] led = new byte[3];
                led[0] = Convert.ToByte(r);
                led[1] = Convert.ToByte(g);
                led[2] = Convert.ToByte(b);
                return led;
            }
        }
    }
}
