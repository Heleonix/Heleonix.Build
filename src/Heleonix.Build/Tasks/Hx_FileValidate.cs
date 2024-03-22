// <copyright file="Hx_FileValidate.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Text.RegularExpressions;

public class Hx_FileValidate : BaseTask
{
    [Required]
    public ITaskItem[] Files { get; set; }

    public string RegExpOptions { get; set; } = "IgnoreCase";

    protected override void ExecuteInternal()
    {
        var regExpOptions = (RegexOptions)Enum.Parse(typeof(RegexOptions), this.RegExpOptions, true);

        foreach (var file in this.Files)
        {
            var patterns = file.CloneCustomMetadata() as IDictionary<string, string>;

            this.Log.LogMessage(
                MessageImportance.High,
                Resources.FileValidate_ValidatingFile,
                file.ItemSpec,
                string.Join(";", patterns.Select(m => $"{m.Key}={m.Value}")));

            if (!File.Exists(file.ItemSpec))
            {
                this.Log.LogError(Resources.FileValidate_FileNotFound, file.ItemSpec);

                continue;
            }

            var input = File.ReadAllText(file.ItemSpec);

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(input, pattern.Value, regExpOptions);

                if (matches.Count == 0)
                {
                    this.Log.LogError(
                        Resources.FileValidate_RuleViolated,
                        file.ItemSpec,
                        pattern.Key,
                        pattern.Value);
                }
            }
        }
    }
}
