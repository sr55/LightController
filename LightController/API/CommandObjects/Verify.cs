// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Verify.cs" company="LightController Project (http://github.com/sr55/LightController)">
//   This file is part of the LightController source code - It may be used under the terms of the MIT License.
// </copyright>
// <summary>
//   Defines the Verify type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LightController.API.CommandObjects
{
    using System.Text.Json.Serialization;

    public class Verify
    {
        public Verify(string challengeResponse)
        {
            this.ChallengeResponse = challengeResponse;
        }

        [JsonPropertyName("challenge-response")]
        public string ChallengeResponse { get; set; }
    }
}
