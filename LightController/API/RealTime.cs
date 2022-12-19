// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealTime.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the RealTime type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using System.Threading.Tasks;

namespace LightController.API
{
    using System;
    using System.Collections.Generic;
    using LightController.API.FrameData;
    using LightController.API.Model;
    using LightController.Helpers;

    public class RealTime
    {
        private readonly string baseUrl;
        private readonly int port;
        private readonly int ledCount;
        private readonly bool isRgbw;
        private readonly string ipAddress;

        private string httpRealtimeFrameUrl = "/xled/v1/led/rt/frame";

        public RealTime(string baseUrl, int port, int ledCount, bool isRgbw)
        {
            this.baseUrl = baseUrl;
            this.port = port;
            this.ledCount = ledCount;
            this.isRgbw = isRgbw;
            this.ipAddress = baseUrl.Replace("http://", string.Empty, StringComparison.InvariantCultureIgnoreCase);
        }

        public void SendFrame(LoginResponse auth, List<Led> ledSet, bool isDebugMode, int frameNumber)
        {
            ConsoleOutput.WriteLine(string.Format("  - Sending Frame {0} (UDP)", frameNumber), ConsoleColor.Yellow);

            byte[] frame = GenerateFrame(auth, ledSet, false);
            UDPHelper.SendFrame(auth, frame, this.ipAddress, this.port, isDebugMode);
        }

        public void SendFrameHttp(LoginResponse token, List<Led> ledSet, bool isDebugMode, int frameNumber)
        {
            ConsoleOutput.WriteLine(string.Format("  - Sending Frame {0} (HTTP)", frameNumber), ConsoleColor.Yellow);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", token.Authentication_token);

            byte[] frame = GenerateFrame(token, ledSet, true);

            var task = Task.Run(async () => await HttpHelper.MakePOSTRequest(this.baseUrl + this.httpRealtimeFrameUrl, frame, headers, "application/octet-stream"));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                VerifyResponse response = JsonSerializer.Deserialize<VerifyResponse>(task.Result.JsonResponse, HttpHelper.JsonOptions);
                if (response != null)
                {
                    ConsoleOutput.WriteLine(string.Format("    Frame Reply: {0}", response.Code), ConsoleColor.Cyan);
                }
            }
            else
            {
                ConsoleOutput.WriteLine(string.Format("    Failed to send frame ({0})", task.Result?.JsonResponse), ConsoleColor.Red);
            }
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
        
        private byte[] GenerateFrame(LoginResponse response, List<Led> ledSet, bool skipHeader)
        {
            int byteSize = skipHeader ? (this.ledCount * 4)  :  (this.ledCount * 4) + 10;
            byte[] frame = new byte[byteSize];
            int bytePosition = 0;

            if (!skipHeader)
            {
                frame[0] = 0x01;

                byte[] token = Convert.FromBase64String(response.Authentication_token);

                Buffer.BlockCopy(token, 0, frame, 1, token.Length);

                frame[9] = Convert.ToByte(this.ledCount);

                bytePosition = 10;
            }

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
