// <copyright file="FileRazorGenerate.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using RazorEngineCore;

/// <summary>
/// Generates a file using the passed Razor template file and passed list of data items.
/// </summary>
/// <example>
/// <![CDATA[
/// @using System
/// @using Microsoft.Build.Framework
/// @inherits RazorEngineCore.RazorEngineTemplateBase<ITaskItem[]>
/// @DateTime.UtcNow.ToShortDateString()
/// @foreach (var item in Model)
/// {
///     <text>- </text> @item.GetMetadata("description")
///     @:
/// }
/// ]]></example>
public class FileRazorGenerate : BaseTask
{
    /// <summary>
    /// Gets or sets path to the Razor template file.
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
            this.Log.LogError(Resources.FileRazorGenerate_TemplateNotFound, this.TemplateFile);

            return;
        }

        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        var engine = new RazorEngine();

        var template = engine.Compile<RazorEngineTemplateBase<ITaskItem[]>>(File.ReadAllText(this.TemplateFile));

        var result = template.Run(instance => instance.Model = this.Data ?? Array.Empty<ITaskItem>());

        File.WriteAllText(this.GeneratedFile, result);
    }

    /// <summary>
    /// Resolves assemblies known to this assembly.
    /// </summary>
    /// <param name="sender">An object which sent the event.</param>
    /// <param name="args">Event arguments to get the name of the missing assembly to resolve.</param>
    /// <returns>A resolved assembly or <c>null</c> otherwise.</returns>
    [ExcludeFromCodeCoverage]
    private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        // Resolve assemblies, which are referenced in the compiled template assembly.
        if (args.Name.Contains("RazorEngineCore"))
        {
            return typeof(RazorEngine).Assembly;
        }

        if (args.Name.Contains("Microsoft.Build.Framework"))
        {
            return typeof(ITaskItem).Assembly;
        }

        return null;
    }
}
