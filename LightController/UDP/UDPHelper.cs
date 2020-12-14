// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UDPHelper.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the UDPHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.UDP
{
    using System;
    using System.Net.Sockets;

    using LightController.API.Model;

    public class UDPHelper
    {
        internal void UDPSendFrame(LoginResponse auth, byte[] frame, string ipAddress, int port)
        {
            UdpClient udpClient = new UdpClient(ipAddress, port);

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
