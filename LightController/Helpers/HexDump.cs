// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HexDump.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the HexDump type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.Helpers
{
    using System;
    using System.Text;

    public class HexDump
    {
        public static string Write(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null)
            {
                return "<null>";
            }

            int bytesLength = bytes.Length;

            char[] hexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn = 11;
            int firstCharColumn = firstHexColumn + bytesPerLine * 3 + (bytesPerLine - 1) / 8 + 2;

            int lineLength = firstCharColumn + bytesPerLine + Environment.NewLine.Length;

            char[] line = (new string(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = hexChars[(i >> 28) & 0xF];
                line[1] = hexChars[(i >> 24) & 0xF];
                line[2] = hexChars[(i >> 20) & 0xF];
                line[3] = hexChars[(i >> 16) & 0xF];
                line[4] = hexChars[(i >> 12) & 0xF];
                line[5] = hexChars[(i >> 8) & 0xF];
                line[6] = hexChars[(i >> 4) & 0xF];
                line[7] = hexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0)
                    {
                        hexColumn++;
                    }

                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = hexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = hexChars[b & 0xF];
                        line[charColumn] = AsciiSymbol(b);
                    }

                    hexColumn += 3;
                    charColumn++;
                }

                result.Append(line);
            }

            return result.ToString();
        }

        private static char AsciiSymbol(byte val)
        {
            if (val < 32)
            {
                return '.';
            }

            if (val < 127)
            {
                return (char)val;
            } 

            // Handle the hole in Latin-1
            if (val == 127)
            {
                return '.';
            }

            if (val < 0x90)
            {
                return "€.‚ƒ„…†‡ˆ‰Š‹Œ.Ž."[val & 0xF];
            }

            if (val < 0xA0)
            {
                return ".‘’“”•–—˜™š›œ.žŸ"[val & 0xF];
            }

            if (val == 0xAD)
            {
                return '.';
            }   

            return (char)val;
        }
    }
}
