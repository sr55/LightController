// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleOutput.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the ConsoleOutput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.Helpers
{
    using System;

    public class ConsoleOutput
    {
        private static object lockObj = new object();

        public static void WriteLine(string message, ConsoleColor colour)
        {
            lock (lockObj)
            {
                Console.ForegroundColor = colour;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
