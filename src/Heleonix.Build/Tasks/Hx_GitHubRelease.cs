// <copyright file="Hx_GitHubRelease.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Net;
using System.Text;
using System.Text.Json;

public class Hx_GitHubRelease : BaseTask
{
    [Required]
    public string GitHubRepositoryApiUrl { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public string UserAgent { get; set; }

    [Required]
    public string TagName { get; set; }

    public string TagSource { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public bool IsDraft { get; set; }

    public bool IsPrerelease { get; set; }

    protected override void ExecuteInternal()
    {
        var content = new
        {
            tag_name = this.TagName,
            target_commitish = this.TagSource,
            name = this.Name,
            body = this.Body,
            draft = this.IsDraft,
            prerelease = this.IsPrerelease,
        };

        this.Log.LogMessage(MessageImportance.High, Resources.GitHubRelease_CreatingRelease, content);

        using (var client = new HttpClient())
        using (var requestContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"token {this.Token}");
            client.DefaultRequestHeaders.Add("User-Agent", this.UserAgent);

            var response = client.PostAsync($"{this.GitHubRepositoryApiUrl}/releases", requestContent);

            response.Wait(3 * 60 * 1000);

            var responseContent = response.Result.Content.ReadAsStringAsync();

            responseContent.Wait(3 * 60 * 1000);

            if (response.Result.StatusCode != HttpStatusCode.Created)
            {
                this.Log.LogError(
                    Resources.GitHubRelease_Failed,
                    response.Result.StatusCode,
                    responseContent.Result);
            }
        }
    }
}
