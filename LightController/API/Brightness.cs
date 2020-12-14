// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Brightness.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Brightness type.
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

    public class Brightness : HttpRequestBase
    {
        private readonly string baseUrl;

        private string modeUrl = "/xled/v1/led/out/brightness";

        public Brightness(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        
        public BrightnessResponse GetBrightness(LoginResponse token)
        {
            ConsoleOutput.WriteLine("- Fetching Brightness", ConsoleColor.Yellow);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", token.Authentication_token);

            var task = Task.Run(async () => await this.MakeGETRequest(this.baseUrl + this.modeUrl, headers));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                BrightnessResponse response = JsonSerializer.Deserialize<BrightnessResponse>(task.Result.JsonResponse, JsonOptions);
                if (response != null)
                {
                    ConsoleOutput.WriteLine(string.Format("  Brightness Level: ({0})", response.Value), ConsoleColor.Cyan);
                    return response;
                }
            }
            else
            {
                ConsoleOutput.WriteLine(string.Format("  Get Brightness Failed ({0})", task.Result?.JsonResponse), ConsoleColor.Red);
            }

            return null;
        }

        public BrightnessResponse SetBrightness(LoginResponse token, int value)
        {
            ConsoleOutput.WriteLine(string.Format("- Set Brightness to: {0}", value), ConsoleColor.Yellow);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", token.Authentication_token);

            BrightnessCommand setMode = new BrightnessCommand("enabled", "A", value);
            string jsonString = JsonSerializer.Serialize(setMode, JsonOptions);

            var task = Task.Run(async () => await this.MakePOSTRequest(this.baseUrl + this.modeUrl, jsonString, headers));
            task.Wait();

            if (task.Result != null && task.Result.WasSuccessful)
            {
                BrightnessResponse response = JsonSerializer.Deserialize<BrightnessResponse>(task.Result.JsonResponse, JsonOptions);
                if (response != null)
                {
                    response.Value = value;
                    ConsoleOutput.WriteLine(string.Format("  Brightness Set: ({0})", response.Value), ConsoleColor.Cyan);
                    return response;
                }
            }
            else
            {
                ConsoleOutput.WriteLine(string.Format("  Set Brightness Failed ({0})", task.Result?.JsonResponse), ConsoleColor.Red);
            }

            return null;
        }
    }
}
