# Heleonix.Build

[![Release: .NET / NuGet](https://github.com/Heleonix/Heleonix.Build/actions/workflows/release-net-nuget.yml/badge.svg)](https://github.com/Heleonix/Heleonix.Build/actions/workflows/release-net-nuget.yml)

The MSBuild-based build framework for applications on CI/CD systems.

## Install

https://www.nuget.org/packages/Heleonix.Build

## The idea

This framework is aiming to simplify implementation of different CI/CD stages for applications on CI/CD systems,
like GoCD, Jenkins, TeamCity etc.

The build framework consists of parameterized MSBuild [Targets](https://heleonix.github.io/docs/Heleonix.Build/api/Targets/index.html),
such as [Hx_NetBuild](https://heleonix.github.io/docs/Heleonix.Build/api/Targets/Hx_NetBuild.html),
[Hx_NetTest](https://heleonix.github.io/docs/Heleonix.Build/api/Targets/Hx_NetTest.html),
[Hx_NetValidate](https://heleonix.github.io/docs/Heleonix.Build/api/Targets/Hx_NetValidate.html) etc.,
which usually represent separate steps in CI pipelines.

The build framework also provides set of reusable [Tasks](https://heleonix.github.io/docs/Heleonix.Build/api/Tasks/index.html).

Usually source code is organized by following some well-known or custom standards, that's why this build framework
supports solid customization. Basically it follows the "configurable conventions" approach.

Default values of properties and items of [Targets](https://heleonix.github.io/docs/Heleonix.Build/api/Targets/index.html) follow well-known practices of arrangement of source code.
So, if you follow well-known standards too, you do not even neeed to write custom build scripts.

## More details

- [Usage](https://heleonix.github.io/docs/Heleonix.Build/usage.html)
- [Extensibility](https://heleonix.github.io/docs/Heleonix.Build/extensibility.md)
- [API](https://heleonix.github.io/docs/Heleonix.Build/api/Heleonix.Build.html)

## Contribution Guideline

1. [Create a fork](https://github.com/Heleonix/Heleonix.Build/fork) from the main repository
2. Implement whatever is needed
3. [Create a Pull Request](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-a-pull-request-from-a-fork).
   Make sure the assigned [Checks](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/collaborating-on-repositories-with-code-quality-features/about-status-checks#checks) pass successfully.
   You can watch the progress in the [PR: .NET](https://github.com/Heleonix/Heleonix.Build/actions/workflows/pr-net.yml) GitHub workflows
4. [Request review](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/requesting-a-pull-request-review) from the code owner
5. Once approved, merge your Pull Request via [Squash and merge](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/incorporating-changes-from-a-pull-request/about-pull-request-merges#squash-and-merge-your-commits)

   > [!IMPORTANT]  
   > While merging, enter a [Conventional Commits](https://www.conventionalcommits.org/) commit message.
   > This commit message will be used in automatically generated [Github Release Notes](https://github.com/Heleonix/Heleonix.Build/releases)
   > and [NuGet Release Notes](https://www.nuget.org/packages/Heleonix.Build/#releasenotes-body-tab)

5. Monitor the [Release: .NET / NuGet](https://github.com/Heleonix/Heleonix.Build/actions/workflows/release-net-nuget.yml) GitHub workflow to make sure your changes are delivered successfully
5. In case of any issues, please contact [heleonix.sln@gmail.com](mailto:heleonix.sln@gmail.com)