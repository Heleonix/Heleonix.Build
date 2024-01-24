// <copyright file="Hx_MetadataToCmdArgs.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Collections;
using System.Text;

/// <summary>
/// Stringifies metadata to be used as command line arguments.
/// </summary>
/// <example>
/// <![CDATA[
/// Metadata:
/// <_Key_One>one</_Key_One>
/// <__Key_Two>two</__Key_Two>
/// Becomes: -Key.One=one --Key.Two=two
/// ]]>
/// </example>
public class Hx_MetadataToCmdArgs : BaseTask
{
    /// <summary>
    /// An MSBuild item with metadata to stringify. Leading '_' are replaced with '-'.
    /// </summary>
    public ITaskItem Item { get; set; }

    /// <summary>
    /// The separator string to separate metadata key/value pairs. Default is " ".
    /// </summary>
    public string MetadataSeparator { get; set; } = " ";

    /// <summary>
    /// The separator string to separate key and value of every metadata. Default is "=".
    /// </summary>
    public string KeyValueSeparator { get; set; } = "=";

    /// <summary>
    /// A value indicating whether metadata keys like "Key_Name" should be changed to the "Key.Name" or not.
    /// </summary>
    public bool DottedKeys { get; set; }

    /// <summary>
    /// The stringified metadata.
    /// </summary>
    [Output]
    public string Result { get; set; }

    /// <summary>
    /// Reads a file with specified regular expression and content.
    /// </summary>
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
            var key = Regex.Replace(Regex.Replace(pair.Key.ToString(), "^_", "-"), "^_", "-");

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
