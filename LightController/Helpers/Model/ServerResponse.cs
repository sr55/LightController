// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerResponse.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the ServerResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.Helpers.Model
{
    public class ServerResponse
    {
        public ServerResponse(bool wasSuccessful, string jsonResponse)
        {
            this.WasSuccessful = wasSuccessful;
            this.JsonResponse = jsonResponse;
        }

        public bool WasSuccessful { get; private set; }

        public string JsonResponse { get; private set; }
    }
}