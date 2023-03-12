// <copyright file="FileT4Generate.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Text;
using Microsoft.Build.Utilities;
using Mono.TextTemplating;

/// <summary>
/// Generates a file using the passed T4 template file and passed list of data items.
/// </summary>
/// <example>
/// <list type="bullet">
/// <listheader>The following namespaces are already imported by default:</listheader>
/// <item>System.Linq</item>
/// <item>System.Text.RegularExpressions</item>
/// <item>System.Collections.Generic</item>
/// <item>System.IO</item>
/// <item>System.Linq</item>
/// <item>Microsoft.Build.Framework</item>
/// <item>Heleonix.Build.Tasks</item>
/// </list>
/// <![CDATA[
/// <#@ template hostspecific="true" #>
/// <# foreach (var item in Host.Data) #>
/// <#{#>
/// Item: <#= item.ItemSpec#>
/// <#}#>
/// ]]></example>
public class FileT4Generate : BaseTask
{
    /// <summary>
    /// Gets or sets path to the T4 template file.
    /// </summary>
    [Required]
    public string TemplateFile { get; set; }

    /// <summary>
    /// Gets or sets path to the generated file.
    /// </summary>
    [Required]
    public string GeneratedFile { get; set; }

    /// <summary>
    /// Gets or sets data items to be passed to the template file.
    /// </summary>
    public ITaskItem[] Data { get; set; }

    /// <summary>
    /// Executes the task.
    /// </summary>
    protected override void ExecuteInternal()
    {
        if (!File.Exists(this.TemplateFile))
        {
            this.Log.LogError(Resources.FileT4Generate_TemplateNotFound, this.TemplateFile);

            return;
        }

        var generator = new FileT4GenerateHost(this.Data);

        generator.Refs.Add(typeof(FileT4GenerateHost).Assembly.Location);
        generator.Refs.Add(typeof(ITaskItem).Assembly.Location);
        generator.Refs.Add(typeof(Task).Assembly.Location);

        generator.Imports.Add(typeof(Enumerable).Namespace);
        generator.Imports.Add(typeof(List<object>).Namespace);
        generator.Imports.Add(typeof(Regex).Namespace);
        generator.Imports.Add(typeof(File).Namespace);
        generator.Imports.Add(typeof(FileT4GenerateHost).Namespace);
        generator.Imports.Add(typeof(ITaskItem).Namespace);

        var task = generator.ProcessTemplateAsync(this.TemplateFile, this.GeneratedFile);

        task.Wait(30 * 60 * 1000);

        if (!task.Result)
        {
            var errors = new StringBuilder();

            foreach (var error in generator.Errors)
            {
                errors.Append(error.ToString());
                errors.AppendLine();
            }

            this.Log.LogError(Resources.FileT4Generate_GeneratorFailed, errors.ToString());
        }
    }

    /// <summary>
    /// Represents the custom host for the T4 generator with data items.
    /// </summary>
    public class FileT4GenerateHost : TemplateGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileT4GenerateHost"/> class.
        /// </summary>
        /// <param name="data">The data to be used in a template file.</param>
        public FileT4GenerateHost(ITaskItem[] data)
        {
            this.Data = data ?? Array.Empty<ITaskItem>();
        }

        /// <summary>
        /// Gets data items to be used in a template file.
        /// </summary>
        public ITaskItem[] Data { get; private set; }

        /// <inheritdoc/>
        public override Type SpecificHostType { get; } = typeof(FileT4GenerateHost);
    }
}
