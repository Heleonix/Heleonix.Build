// <copyright file="GitHubRelease.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Heleonix.Build.Properties;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Creates a release on GitHub using the GitHub API.
    /// </summary>
    public class GitHubRelease : BaseTask
    {
        /// <summary>
        /// Gets or sets the url of the GitHub Releases API.
        /// </summary>
        [Required]
#pragma warning disable CA1056 // Uri properties should not be strings
        public string ReleasesApiUrl { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <summary>
        /// Gets or sets a personal access token to authorize to the GitHub Releases API.
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the User-Agent request header.
        /// </summary>
        [Required]
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets a name of the tag to create and/or from which the release should be created.
        /// </summary>
        [Required]
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets any branch or commit SHA from which the tag should be created.
        /// It is ignored if the tag already exists. Otherwise, name of the default branch (usually master) is used.
        /// </summary>
        public string TagSource { get; set; }

        /// <summary>
        /// Gets or sets a name of the release. If not specified, the tag name is used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create a draft (unpublished) release, or a published one.
        /// </summary>
        public bool IsDraft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to identify the release as a prerelease, or as a full release.
        /// </summary>
        public bool IsPrerelease { get; set; }

        /// <summary>
        /// Executes the GitHubRelease.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var content = "{ " +
                $"\"tag_name\": \"{this.TagName}\", " +
                $"\"target_commitish\": \"{this.TagSource}\", " +
                $"\"name\": \"{this.Name}\", " +
                $"\"draft\": {(this.IsDraft ? "true" : "false")}, " +
                $"\"prerelease\": {(this.IsPrerelease ? "true" : "false")}" +
                " }";

            this.Log.LogMessage(Resources.GitHubRelease_CreatingRelease, content);

            using (var client = new HttpClient())
            using (var requestContent = new StringContent(content, Encoding.UTF8, "application/json"))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"token {this.Token}");
                client.DefaultRequestHeaders.Add("User-Agent", this.UserAgent);

                var response = client.PostAsync(new Uri(this.ReleasesApiUrl), requestContent);

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
}
