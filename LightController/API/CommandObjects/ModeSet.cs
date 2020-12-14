// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModeSet.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the ModeSet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API.CommandObjects
{
    public class ModeSet
    {
        public ModeSet(string mode)
        {
            this.Mode = mode;
        }

        public string Mode { get; set; }
    }
}
