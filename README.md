# Heleonix.Build

The MSBuild-based build framework for applications on CI and CD systems.

## Install

https://www.nuget.org/packages/Heleonix.Build

## The small idea

This framework is developed to simplify writing scripts to build applications on CI/CD systems like GoCD, Jenkins, TeamCity etc.

The framework provides parameterized targets such as Hx_Build, Hx_OpenCover, Hx_NUnit, Hx_Validate etc., which represent separate steps in CI pipelines.

Each company designs some custom standards in organization of source code, so this build framework supports detailed customization.
Basically it follows "configurable conventions" approach.
It means, that source code file patterns, folders organization is configured on CI/CD servers and is passed into the build framework,
so all projects in your company just follow those conventions and are successfully recognized and built on CI servers.

Default values of target parameters follow well-known standards in source code organization.
So, if you follow standards as well, you even do not neeed to write custom build scripts, like "Build.props", "MySuperProjectBuild.targets", "bla-bla.props" etc.

## Writing custom scripts

Naming Conventions:

Global properties: `Ns_WS|System|Input|Build_Property[File(s)|Dir(s)|Ext]`

Target Name: `Ns[_Net|JS|Internal]_Target[_B|A_Target]`

Target Parameter: `Target_Parameter[File(s)|Dir(s)|Ext]`

Target Variable: `_Target_Variable[File(s)|Dir(s)|Ext]`

Use slash `/` in paths, not backslash `\`, because MSBuild replaces them on *nix OSs anyway