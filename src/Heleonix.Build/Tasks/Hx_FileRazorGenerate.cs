// <copyright file="Hx_FileRazorGenerate.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using RazorEngineCore;

public class Hx_FileRazorGenerate : BaseTask
{
    [Required]
    public string TemplateFile { get; set; }

    [Required]
    public string GeneratedFile { get; set; }

    public ITaskItem[] Data { get; set; }

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
