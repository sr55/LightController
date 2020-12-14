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
    using System.Threading;

    using LightController.API.Model;
    using LightController.Helpers;
    using LightController.UDP;

    public class RealTime : UDPHelper
    {
        private readonly int port;

        private readonly int ledCount;

        private readonly string ipAddress;

        public RealTime(string baseUrl, int port, int ledCount)
        {
            this.port = port;
            this.ledCount = ledCount;
            this.ipAddress = baseUrl.Replace("http://", string.Empty, StringComparison.InvariantCultureIgnoreCase);
        }

        private int bitflip;

        public void SendFrame(LoginResponse auth)
        {
            ConsoleOutput.WriteLine("  - Sending Frame", ConsoleColor.Yellow);

            byte[] ledColour = null;

            if (this.bitflip == 0)
            {
                ledColour = this.GenerateLedColour(50, 0, 0);
                this.bitflip = 1;
            }
            else if (this.bitflip == 1)
            {
                ledColour = this.GenerateLedColour(0, 50, 0);
                this.bitflip = 2;
            }
            else
            {
                ledColour = this.GenerateLedColour(0, 0, 50);
                this.bitflip = 0;
            }
            
            byte[] frame = GenerateFrame(auth, ledColour);
            UDPSendFrame(auth, frame, this.ipAddress, this.port);

            Thread.Sleep(1000);
        }
        
        public byte[] GenerateFrame(LoginResponse response, byte[] ledColour)
        {
            int byteSize = (this.ledCount * 3) + 10;
            byte[] frame = new byte[byteSize];

            frame[0] = 0x01;

            byte[] token = Convert.FromBase64String(response.Authentication_token);

            Buffer.BlockCopy(token, 0, frame, 1, token.Length);

            frame[9] = Convert.ToByte(this.ledCount);

            int bytePosition = 10;
            for (int i = 0; i < this.ledCount; i++)
            {
                Buffer.BlockCopy(ledColour, 0, frame, bytePosition, ledColour.Length);
                bytePosition = bytePosition + 3;
            }

            return frame;
        }

        public byte[] GenerateLedColour(int r, int g, int b)
        {
            byte[] led = new byte[3];
            led[0] = Convert.ToByte(r);
            led[1] = Convert.ToByte(g);
            led[2] = Convert.ToByte(b);

            return led;
        }
    }
}
