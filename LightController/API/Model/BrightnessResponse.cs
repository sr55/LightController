// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrightnessResponse.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the BrightnessResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API.Model
{
    using System;

    public class BrightnessResponse
    {
        public int Value { get; set; }
        public string Mode { get; set; }
        public int Code { get; set; }
    }
}
