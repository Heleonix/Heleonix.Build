// <copyright file="Hx_FileSystemSearch.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Text.RegularExpressions;

public class Hx_FileSystemSearch : BaseTask
{
    [Required]
    public string StartDir { get; set; }

    public string Direction { get; set; }

    public string Types { get; set; }

    public string PathRegExp { get; set; }

    public string PathRegExpOptions { get; set; } = "IgnoreCase";

    public string ContentRegExp { get; set; }

    public string ContentRegExpOptions { get; set; } = "IgnoreCase";

    [Output]
    public string[] FoundFiles { get; set; }

    [Output]
    public string[] FoundDirs { get; set; }

    [Output]
    public string[] FoundItems { get; set; }

    protected override void ExecuteInternal()
    {
        if (!Directory.Exists(this.StartDir))
        {
            this.Log.LogMessage(MessageImportance.High, Resources.FileSystemSearch_StartingDirectoryNotFound, this.StartDir);

            this.FoundFiles = Array.Empty<string>();
            this.FoundDirs = Array.Empty<string>();
            this.FoundItems = Array.Empty<string>();

            return;
        }

        var pathRegExpOptions = (RegexOptions)Enum.Parse(typeof(RegexOptions), this.PathRegExpOptions);
        var pathRegExp = string.IsNullOrEmpty(this.PathRegExp)
            ? null
            : new Regex(this.PathRegExp.Replace("/", new string(Path.DirectorySeparatorChar, 2)).Replace("//", "/"), pathRegExpOptions);

        var contentRegExpOptions = (RegexOptions)Enum.Parse(typeof(RegexOptions), this.ContentRegExpOptions);
        var contentRegExp = string.IsNullOrEmpty(this.ContentRegExp)
            ? null
            : new Regex(this.ContentRegExp, contentRegExpOptions);

        var foundFiles = new List<string>();
        var foundDirs = new List<string>();
        var foundItems = new List<string>();

        this.Log.LogMessage(
            MessageImportance.High,
            Resources.FileSystemSearch_StartSearching,
            this.StartDir,
            this.Types,
            this.Direction,
            this.PathRegExp + $" (Transformed: {pathRegExp})",
            this.ContentRegExp);

        var startDir = this.StartDir.TrimEnd(Path.DirectorySeparatorChar);
        this.Search(
            startDir,
            startDir.Length,
            pathRegExp,
            contentRegExp,
            foundFiles,
            foundDirs,
            foundItems);

        this.FoundFiles = foundFiles.ToArray();
        this.FoundDirs = foundDirs.ToArray();
        this.FoundItems = foundItems.ToArray();
    }

    private void Search(
        string currentDir,
        int startAt,
        Regex pathRegExp,
        Regex contentRegExp,
        ICollection<string> foundFiles,
        ICollection<string> foundDirs,
        ICollection<string> foundItems)
    {
        if (string.IsNullOrEmpty(currentDir))
        {
            return;
        }

        if (string.IsNullOrEmpty(this.Types) || this.Types == "Directories" || this.Types == "All")
        {
            if (this.Direction == "Up")
            {
                var dirs = Directory.GetDirectories(currentDir).Where(d => pathRegExp?.IsMatch(d) ?? true);

                foreach (var dir in dirs)
                {
                    foundDirs.Add(dir);
                    foundItems.Add(dir);

                    this.Log.LogMessage(MessageImportance.High, Resources.FileSystemSearch_FoundItem, dir);
                }
            }

            if ((string.IsNullOrEmpty(this.Direction) || this.Direction == "Down")
                && ((pathRegExp == null) || pathRegExp.IsMatch(currentDir[startAt..])))
            {
                foundDirs.Add(currentDir);
                foundItems.Add(currentDir);

                this.Log.LogMessage(MessageImportance.High, Resources.FileSystemSearch_FoundItem, currentDir);
            }
        }

        if (string.IsNullOrEmpty(this.Types) || this.Types == "Files" || this.Types == "All")
        {
            var files = Directory.GetFiles(currentDir)
                .Where(f =>
                    (pathRegExp?.IsMatch(f[((string.IsNullOrEmpty(this.Direction) || this.Direction == "Down") ? startAt : 0) ..]) ?? true)
                    && (contentRegExp?.IsMatch(File.ReadAllText(f)) ?? true));

            foreach (var file in files)
            {
                foundFiles.Add(file);
                foundItems.Add(file);

                this.Log.LogMessage(MessageImportance.High, Resources.FileSystemSearch_FoundItem, file);
            }
        }

        if (string.IsNullOrEmpty(this.Direction) || this.Direction == "Down")
        {
            foreach (var subDir in Directory.GetDirectories(currentDir))
            {
                this.Search(
                    subDir,
                    startAt,
                    pathRegExp,
                    contentRegExp,
                    foundFiles,
                    foundDirs,
                    foundItems);
            }
        }

        if (this.Direction == "Up")
        {
            this.Search(
                Path.GetDirectoryName(currentDir),
                0,
                pathRegExp,
                contentRegExp,
                foundFiles,
                foundDirs,
                foundItems);
        }
    }
}
