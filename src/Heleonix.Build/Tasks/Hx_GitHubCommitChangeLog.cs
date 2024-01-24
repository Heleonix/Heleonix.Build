// <copyright file="Hx_GitHubCommitChangeLog.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Globalization.NumberFormatInfo;

/// <summary>
/// Collects changes from commits since the latest release on GitHub using the GitHub API, and the new version.
/// </summary>
public class Hx_GitHubCommitChangeLog : BaseTask
{
    /// <summary>
    /// The url of the GitHub API for the repository.
    /// </summary>
    [Required]
    public string GitHubRepositoryApiUrl { get; set; }

    /// <summary>
    /// A personal access token to authorize to the GitHub API.
    /// </summary>
    [Required]
    public string Token { get; set; }

    /// <summary>
    /// The User-Agent request header.
    /// </summary>
    [Required]
    public string UserAgent { get; set; }

    /// <summary>
    /// A regular expression to extract a version from a tag name of the latest release in format '11.22.33'.
    /// </summary>
    [Required]
    public string VersionTagRegExp { get; set; }

    /// <summary>
    /// A regular expression to identify a major change.
    /// </summary>
    [Required]
    public string MajorChangeRegExp { get; set; }

    /// <summary>
    /// A regular expression to identify a minor change.
    /// </summary>
    [Required]
    public string MinorChangeRegExp { get; set; }

    /// <summary>
    /// A regular expression to identify a patch change.
    /// </summary>
    [Required]
    public string PatchChangeRegExp { get; set; }

    /// <summary>
    /// A regular expression to capture a change for the change log.
    /// </summary>
    [Required]
    public string ChangeLogRegExp { get; set; }

    /// <summary>
    /// The .NET regular expression options for regexp patterns. Default is "None".
    /// </summary>
    public string RegExpOptions { get; set; } = "None";

    /// <summary>
    /// The calculated version based on the change conventions [Output].
    /// </summary>
    [Output]
    public string Version { get; set; }

    /// <summary>
    /// The list of changes with metadata as captured groups names and values. The 'Version' and 'PreviousVersion' metadata specify the new calculated and previous versions [Output].
    /// </summary>
    [Output]
    public ITaskItem[] Changes { get; set; }

    /// <summary>
    /// Executes the task.
    /// </summary>
    protected override void ExecuteInternal()
    {
        this.Log.LogMessage(MessageImportance.High, Resources.GitHubCommitChangeLog_GettingLatestRelease);

        var regExpOptions = (RegexOptions)Enum.Parse(typeof(RegexOptions), this.RegExpOptions, true);

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Accept", $"application/vnd.github+json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.Token}");
        client.DefaultRequestHeaders.Add("User-Agent", this.UserAgent);

        var release = client.GetFromJsonAsync<JsonElement>($"{this.GitHubRepositoryApiUrl}/releases/latest");

        release.Wait(3 * 60 * 1000);

        string createdAt = null;
        string tagSource = null;
        var latestVersion = (Major: 0, Minor: 0, Patch: 0);

        try
        {
            createdAt = release.Result.GetProperty("created_at").GetString();
            tagSource = release.Result.GetProperty("target_commitish").GetString();

            var tagVersion = Regex.Match(
                release.Result.GetProperty("tag_name").GetString(),
                this.VersionTagRegExp,
                regExpOptions).Value.Split('.');

            latestVersion.Major = Convert.ToInt32(tagVersion[0], InvariantInfo);
            latestVersion.Minor = Convert.ToInt32(tagVersion[1], InvariantInfo);
            latestVersion.Patch = Convert.ToInt32(tagVersion[2], InvariantInfo);
        }
        catch (KeyNotFoundException)
        {
            this.Log.LogMessage(MessageImportance.High, Resources.GitHubCommitChangeLog_NoReleaseFound);
        }

        this.Log.LogMessage(MessageImportance.High, Resources.GithubCommitChangeLog_CollectChanges, createdAt);

        this.CollectChanges(client, createdAt, latestVersion, regExpOptions, tagSource);
    }

    private static TaskItem CreateTaskItemFromChangeMatch(Regex changeLogRegExp, Match changeLogMatch)
    {
        var item = new TaskItem(changeLogMatch.Value);

        foreach (var groupName in changeLogRegExp.GetGroupNames())
        {
            if (changeLogMatch.Groups[groupName].Success)
            {
                item.SetMetadata(groupName, changeLogMatch.Groups[groupName].Value);
            }
        }

        return item;
    }

    private void CollectChanges(
        HttpClient client,
        string createdAt,
        (int Major, int Minor, int Patch) latestVersion,
        RegexOptions regExpOptions,
        string tagSource)
    {
        var previousVersion = $"{latestVersion.Major}.{latestVersion.Minor}.{latestVersion.Patch}";

        var pageNumber = 1;

        Task<JsonArray> commitPage;

        var changes = new List<ITaskItem>();

        var majorChangeRegExp = new Regex(this.MajorChangeRegExp, regExpOptions);
        var minorChangeRegExp = new Regex(this.MinorChangeRegExp, regExpOptions);
        var patchChangeRegExp = new Regex(this.PatchChangeRegExp, regExpOptions);
        var changeLogRegExp = new Regex(this.ChangeLogRegExp, regExpOptions);

        var wasMajorIncreased = false;
        var wasMinorIncreased = false;
        var wasPatchIncreased = false;

        do
        {
            commitPage = client.GetFromJsonAsync<JsonArray>(
                $"{this.GitHubRepositoryApiUrl}/commits?sha={tagSource}&per_page={100}&page={pageNumber}"
                + (createdAt != null ? $"&since={createdAt}" : string.Empty));

            commitPage.Wait(3 * 60 * 1000);

            foreach (var c in commitPage.Result)
            {
                var rawMessage = c["commit"]["message"].GetValue<string>();

                if (!wasPatchIncreased && patchChangeRegExp.IsMatch(rawMessage))
                {
                    latestVersion.Patch += 1;

                    wasPatchIncreased = true;
                }

                if (!wasMinorIncreased && minorChangeRegExp.IsMatch(rawMessage))
                {
                    latestVersion.Minor += 1;
                    latestVersion.Patch = 0;

                    wasMinorIncreased = wasPatchIncreased = true;
                }

                if (!wasMajorIncreased && majorChangeRegExp.IsMatch(rawMessage))
                {
                    latestVersion.Major += 1;
                    latestVersion.Minor = 0;
                    latestVersion.Patch = 0;

                    wasMajorIncreased = wasMinorIncreased = wasPatchIncreased = true;
                }

                var changeLogMatch = changeLogRegExp.Match(rawMessage);

                if (changeLogMatch.Success)
                {
                    changes.Add(CreateTaskItemFromChangeMatch(changeLogRegExp, changeLogMatch));
                }
            }

            pageNumber += 1;
        }
        while (commitPage.Result.Count == 100);

        if (latestVersion.Major == 0)
        {
            latestVersion.Major = 1;
            latestVersion.Minor = 0;
            latestVersion.Patch = 0;
        }

        this.Version = $"{latestVersion.Major}.{latestVersion.Minor}.{latestVersion.Patch}";

        changes.Add(new TaskItem(this.Version, new Dictionary<string, string> { { nameof(this.Version), this.Version } }));
        changes.Add(new TaskItem(previousVersion, new Dictionary<string, string> { { "PreviousVersion", previousVersion } }));

        this.Changes = changes.ToArray();
    }
}
