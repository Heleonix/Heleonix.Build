// <copyright file="Hx_MetadataToCmdArgs.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Collections;
using System.Text;

public class Hx_MetadataToCmdArgs : BaseTask
{
    public ITaskItem Item { get; set; }

    public string MetadataSeparator { get; set; } = " ";

    public string KeyValueSeparator { get; set; } = "=";

    public bool DottedKeys { get; set; }

    [Output]
    public string Result { get; set; }

    protected override void ExecuteInternal()
    {
        if (this.Item == null)
        {
            return;
        }

        var result = new StringBuilder();

        var metadata = this.Item.CloneCustomMetadata();

        foreach (DictionaryEntry pair in metadata)
        {
            var key = Regex.Replace(Regex.Replace(pair.Key.ToString(), "^__", "--"), "^_", "-");

            key = this.DottedKeys ? key.Replace("_", ".") : key;

            if (string.IsNullOrEmpty(pair.Value.ToString()))
            {
                result.Append($"{key}{this.MetadataSeparator}");
            }
            else
            {
                result.Append($"{key}{this.KeyValueSeparator}{pair.Value}{this.MetadataSeparator}");
            }
        }

        this.Result = result.ToString();
    }
}
