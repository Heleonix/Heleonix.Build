# Heleonix.Build

[![CI/CD](https://github.com/Heleonix/Heleonix.Build/actions/workflows/hx-net-nuget.yml/badge.svg?event=push)](https://github.com/Heleonix/Heleonix.Build/actions/workflows/hx-net-nuget.yml)

The MSBuild-based build framework for applications on CI and CD systems.

## Install

https://www.nuget.org/packages/Heleonix.Build

## The main idea

This framework is developed to simplify writing scripts to build applications on CI/CD systems like GoCD, Jenkins, TeamCity etc.

The framework provides parameterized targets such as Hx_NetBuild, Hx_Coverlet, Hx_NetTest, Hx_Validate etc., which represent separate steps in CI pipelines.

Each company designs some custom standards in organization of source code, so this build framework supports detailed customization.
Basically it follows "configurable conventions" approach.
It means, that source code file patterns, folders organization is configured on CI/CD servers and is passed into the build framework,
so all projects in your company just follow those conventions and are successfully recognized and built on CI servers.

Default values of target parameters follow well-known standards in source code organization.
So, if you follow standards as well, you even do not neeed to write custom build scripts, like "Build.props", "MySuperProjectBuild.targets", "bla-bla.props" etc.

## Writing custom scripts

### Naming Conventions:

#### Global Property Name:
`<Ns><_Sys | _WS | _Run>_<PropertyName>[Dir(s) | File(s) | Url(s) | <Ext>]`

Global properties are defined out of targets, so they can be overriden in your custom *.hxbproj files out of targets or from the command line.

#### Target Name:
`<Ns>_<TargetName>[_Variation]`
Artifacts directory for all variations is the same
`Ns_Int_*` supposed to be used within the framework and your custom targets internally, but not by CI/CD.

#### Target's Parameter Name:
`<TargetName>_<ParameterName>[Dir(s) | File(s) | <Ext>]`

If a parameter is a single item, then it should be an MSBuild property.
If a parameter is multiple items (the name ending with (s)), then it should be an MSBuild item.

#### Target's Private Parameter Name:
`_<TargetName>_<ParameterName>[Dir(s) | File(s) | <Ext>]`

#### Target's artifacts:
The properties `Ns_TargetName_ArtifactsDir` defining paths to artifacts directories are defined out of targets, as the global properties,
because targets can depend on each other via artifacts. I.e. Hx_ChangeLog_ArtifactsDir is used by Hx_NetBuild, Hx_NetNugetPush etc.

Targets can depend on each other only via artifacts, because if every target is executed in a separate dotnet.exe run,
then their properties and items are not defined.

#### Target template

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <Ns_TargetName_ArtifactsDir>$(Hx_Run_ArtifactsDir)/TargetName</Ns_TargetName_ArtifactsDir>
  </PropertyGroup>

  <Target Name="Ns_TargetName">
    <Message Text="> RUNNING Ns_TargetName ..." Importance="high"/>

    <Hx_NetSetupTool DotnetExe="$(Hx_Sys_DotnetExe)" Name="NameOfFile">
      <Output TaskParameter="ToolPath" PropertyName="_Ns_TargetName_NameOfFileExe"/>
    </Hx_NetSetupTool>

    <!--Validation of required parameters-->

    <!--Target's property groups, setting default vaues-->

    <!--Target's item groups, setting default values-->

    <Message Text="> 1/3: Doing step one" Importance="high"/>
    <!--tasks, targets etc.-->
    <!--tasks, targets etc.-->

    <Message Text="> 2/3: Doing step two" Importance="high"/>
    <!--tasks, targets etc.-->
    <!--tasks, targets etc.-->

    <Message Text="> 3/3: Doing step three" Importance="high"/>
    <!--tasks, targets etc.-->
    <!--tasks, targets etc.-->

    <Message Text="> DONE Ns_TargetName" Importance="high"/>

    <OnError ExecuteTargets="Hx_OnError"/>
  </Target>
</Project>
```

### Notes
Use slash `/` in paths in MSBuild scripts, not backslash `\`, because MSBuild replaces them on *nix OSs anyway

Use slash `/` in path regular expressions in Heleonix.Build tasks.

### ToDo
- describe Hx_Int_ targets in xsd schemas