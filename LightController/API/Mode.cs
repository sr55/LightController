// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mode.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Mode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using LightController.API.CommandObjects;
    using LightController.API.Model;
    using LightController.Helpers;
    using LightController.HTTP;

    public class Mode : HttpRequestBase
    {
        private readonly string baseUrl;

        private string modeUrl = "/xled/v1/led/mode";

        public Mode(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        
        public ModeResponse GetMode(LoginResponse token)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", token.Authentication_token);

            var task = Task.Run(async () => await this.MakeGETRequest(this.baseUrl + this.modeUrl, headers));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                ModeResponse response = JsonSerializer.Deserialize<ModeResponse>(task.Result.JsonResponse, JsonOptions);
                if (response != null)
                {
                    ConsoleOutput.WriteLine(string.Format("  Lights Mode: ({0})", response.Mode), ConsoleColor.Cyan);
                    return response;
                }
            }
            else
            {
                ConsoleOutput.WriteLine(string.Format("  Get Mode Failed ({0})", task.Result?.JsonResponse), ConsoleColor.Red);
            }

            return null;
        }

        public ModeResponse SetMode(LoginResponse token, string mode)
        {
            ConsoleOutput.WriteLine(string.Format("- Set Mode: {0}", mode), ConsoleColor.Yellow);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", token.Authentication_token);

            ModeSet setMode = new ModeSet(mode);
            string jsonString = JsonSerializer.Serialize(setMode, JsonOptions);

            var task = Task.Run(async () => await this.MakePOSTRequest(this.baseUrl + this.modeUrl, jsonString, headers));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                ModeResponse response = JsonSerializer.Deserialize<ModeResponse>(task.Result.JsonResponse, JsonOptions);
                if (response != null)
                {
                    response.Mode = mode;
                    ConsoleOutput.WriteLine(string.Format("  Mode Set: {0}", mode), ConsoleColor.Cyan);
                    return response;
                }
            }
            else
            {
                ConsoleOutput.WriteLine(string.Format("  Set Mode Failed ({0})", task.Result?.JsonResponse), ConsoleColor.Red);
            }

            return null;
        }
    }
}
