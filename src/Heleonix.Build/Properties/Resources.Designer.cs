﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Heleonix.Build.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Heleonix.Build.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cleaning directory &apos;{0}&apos; started..
        /// </summary>
        internal static string DirectoryClean_CleaningDirectoryStarted {
            get {
                return ResourceManager.GetString("DirectoryClean_CleaningDirectoryStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The directory &apos;{0}&apos; not found. Skipping..
        /// </summary>
        internal static string DirectoryClean_DirectoryNotFound {
            get {
                return ResourceManager.GetString("DirectoryClean_DirectoryNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Copying file: from &apos;{0}&apos; to &apos;{1}&apos;..
        /// </summary>
        internal static string FileCopy_CopyingFile {
            get {
                return ResourceManager.GetString("FileCopy_CopyingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file to copy &apos;{0}&apos; not found..
        /// </summary>
        internal static string FileCopy_FileNotFound {
            get {
                return ResourceManager.GetString("FileCopy_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A directory &apos;{0}&apos; to cory from was specified, but it does not belong to the file &apos;{1}&apos;. Skipping..
        /// </summary>
        internal static string FileCopy_WithSubDirsFromIsInvalid {
            get {
                return ResourceManager.GetString("FileCopy_WithSubDirsFromIsInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The template file &apos;{0}&apos; not found..
        /// </summary>
        internal static string FileRazorGenerate_TemplateNotFound {
            get {
                return ResourceManager.GetString("FileRazorGenerate_TemplateNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; is not found..
        /// </summary>
        internal static string FileRead_FileNotFound {
            get {
                return ResourceManager.GetString("FileRead_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found {0}..
        /// </summary>
        internal static string FileSystemSearch_FoundItem {
            get {
                return ResourceManager.GetString("FileSystemSearch_FoundItem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The starting directory &apos;{0}&apos; not found. Stopping..
        /// </summary>
        internal static string FileSystemSearch_StartingDirectoryNotFound {
            get {
                return ResourceManager.GetString("FileSystemSearch_StartingDirectoryNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start searching in &apos;{0}&apos;; Type: {1}; Direction: {2}; PathRegExp: {3}; ContentRegExp: {4}..
        /// </summary>
        internal static string FileSystemSearch_StartSearching {
            get {
                return ResourceManager.GetString("FileSystemSearch_StartSearching", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; not found. Stopping..
        /// </summary>
        internal static string FileUpdate_FileNotFound {
            get {
                return ResourceManager.GetString("FileUpdate_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updating file &apos;{0}&apos;..
        /// </summary>
        internal static string FileUpdate_UpdatingFile {
            get {
                return ResourceManager.GetString("FileUpdate_UpdatingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A file &apos;{0}&apos; to validate not found..
        /// </summary>
        internal static string FileValidate_FileNotFound {
            get {
                return ResourceManager.GetString("FileValidate_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; violates the rule &apos;{1}={2}&apos;..
        /// </summary>
        internal static string FileValidate_RuleViolated {
            get {
                return ResourceManager.GetString("FileValidate_RuleViolated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validating file &apos;{0}&apos; with patterns: {1}..
        /// </summary>
        internal static string FileValidate_ValidatingFile {
            get {
                return ResourceManager.GetString("FileValidate_ValidatingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Collecting changes. Since: {0}..
        /// </summary>
        internal static string GithubCommitChangeLog_CollectChanges {
            get {
                return ResourceManager.GetString("GithubCommitChangeLog_CollectChanges", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Getting the latest release..
        /// </summary>
        internal static string GitHubCommitChangeLog_GettingLatestRelease {
            get {
                return ResourceManager.GetString("GitHubCommitChangeLog_GettingLatestRelease", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No release found to collect changes and calculate version from..
        /// </summary>
        internal static string GitHubCommitChangeLog_NoReleaseFound {
            get {
                return ResourceManager.GetString("GitHubCommitChangeLog_NoReleaseFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating a release: {0}..
        /// </summary>
        internal static string GitHubRelease_CreatingRelease {
            get {
                return ResourceManager.GetString("GitHubRelease_CreatingRelease", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating release failed with HTTP response: {0} - {1}..
        /// </summary>
        internal static string GitHubRelease_Failed {
            get {
                return ResourceManager.GetString("GitHubRelease_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Installation of the {0} tool, package {1}, version {2} failed with exit code {3}..
        /// </summary>
        internal static string NetSetupTool_InstallationFailed {
            get {
                return ResourceManager.GetString("NetSetupTool_InstallationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Task failed: &apos;{0}&apos;..
        /// </summary>
        internal static string TaskFailed {
            get {
                return ResourceManager.GetString("TaskFailed", resourceCulture);
            }
        }
    }
}
