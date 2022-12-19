// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UDPHelper.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the UDPHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net.Sockets;
using LightController.API.Model;

namespace LightController.Helpers
{
    public class UDPHelper
    {
        public static void SendFrame(LoginResponse auth, byte[] frame, string ipAddress, int port, bool isDebugMode)
        {
            UdpClient udpClient = new UdpClient(ipAddress, port);

            if (isDebugMode)
            {
                ConsoleOutput.WriteLine(HexDump.Write(frame), ConsoleColor.White);
            }

            try
            {
                udpClient.Send(frame, frame.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
