# Heleonix.Build

[![CI/CD](https://github.com/Heleonix/Heleonix.Build/actions/workflows/hx-net-nuget.yml/badge.svg?event=push)](https://github.com/Heleonix/Heleonix.Build/actions/workflows/hx-net-nuget.yml)

The MSBuild-based build framework for applications on CI and CD systems.

## Install

https://www.nuget.org/packages/Heleonix.Build

## The main idea

This framework is developed to simplify writing scripts to build applications on CI/CD systems like GoCD, Jenkins, TeamCity etc.

The framework provides parameterized targets such as Hx_NetBuild, Hx_OpenCover, Hx_NUnit, Hx_Validate etc., which represent separate steps in CI pipelines.

Each company designs some custom standards in organization of source code, so this build framework supports detailed customization.
Basically it follows "configurable conventions" approach.
It means, that source code file patterns, folders organization is configured on CI/CD servers and is passed into the build framework,
so all projects in your company just follow those conventions and are successfully recognized and built on CI servers.

Default values of target parameters follow well-known standards in source code organization.
So, if you follow standards as well, you even do not neeed to write custom build scripts, like "Build.props", "MySuperProjectBuild.targets", "bla-bla.props" etc.

## Writing custom scripts

### Naming Conventions:

#### Global Property Name:
`<Ns><_Sys | _WS | _In | _Run>_<PropertyName>[Dir(s) | File(s) | <Ext>]`

`Hx_Internal_*` it is only to be used within the Heleonix.Build.

#### Target Name:
`<Ns>_<TargetName>[_Variation]`
Artifacts directory for all variations is the same

#### Target Parameter Name:
`<TargetName>_<ParameterName>[Dir(s) | File(s) | <Ext>]`

#### Target Private Parameter Name:
`_<TargetName>_<ParameterName>[Dir(s) | File(s) | <Ext>]`

### Notes
Use slash `/` in paths in MSBuild scripts, not backslash `\`, because MSBuild replaces them on *nix OSs anyway
Use slash `/` in path regular expressions in Heleonix.Build tasks.
