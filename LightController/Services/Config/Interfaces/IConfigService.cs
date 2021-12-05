// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigService.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LightController.Services.Config.Model;

namespace LightController.Services.Config.Interfaces
{
    public interface IConfigService
    {
        void Init();
        ConfigOptions GetConfig();
    }
}
