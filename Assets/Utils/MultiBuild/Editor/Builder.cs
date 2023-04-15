using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using static UnityEditor.BuildTarget;

namespace Utils.MultiBuild.Editor {

    public static class Builder {

        /// <summary>
        /// Build with default saved options
        /// </summary>
        [Obsolete("Obsolete")]
        // ReSharper disable once UnusedMember.Global
        public static bool Build() {
            Settings settings = Storage.LoadSettings();
            if (settings == null) {
                throw new InvalidOperationException("No saved settings found, cannot build");
            }
            return Build(settings, null);
        }

        // ReSharper disable once CommentTypo
        // ReSharper disable once InvalidXmlDocComment
        /// </summary>
        /// <returns></returns>
        [Obsolete("Obsolete")]
        // ReSharper disable once UnusedMember.Global
        public static void BuildCommandLine() {
            // ReSharper disable once CommentTypo
            // We get all the args, including Unity.exe, -quit -batchmode etc
            // read everything after our execute call
            string[] args = Environment.GetCommandLineArgs();
            // 0 = looking for args
            // 1 = expecting output folder
            // 2 = expecting dev boolean
            // 3 = expecting target
            int stage = 0;
            Settings settings = ScriptableObject.CreateInstance<Settings>();
            settings.Reset();

            const string usage = "\nUsage:\n  Unity <args> -executeMethod MultiBuild.Builder.BuildCommandLine <outputFolder> <is_dev> <targetName> [targetName...]\n";

            foreach (string t in args) {
                switch (stage) {
                    case 0:
                        // Skipping over all args until we see ours
                        if (t.Equals("MultiBuild.Builder.BuildCommandLine")) {
                            stage++;
                        }
                        break;
                    case 1:
                        // next arg is output
                        settings.outputFolder = t;
                        stage++;
                        break;
                    case 2:
                        // next arg is dev flag
                        try {
                            settings.developmentBuild = bool.Parse(t);
                            stage++;
                        } catch (FormatException) {
                            throw new ArgumentException("Development build argument was not a valid boolean" + usage);
                        }
                        break;
                    default:
                        // all subsequent args should be targets
                        try {
                            settings.targets.Add((Target)Enum.Parse(typeof(Target), t));
                        } catch (ArgumentException) {
                            throw new ArgumentException(string.Format("Invalid target '{0}'", t));
                        }
                        break;
                }
            }
            if (stage != 3 || settings.targets.Count == 0) {
                throw new ArgumentException("Not enough arguments." + usage);
            }

            Build(settings, null);

        }

        /// <summary>
        /// Build with given settings, call back if required
        /// </summary>
        /// <param name="settings">Settings to build with</param>
        /// <param name="callback">Callback which is called before and after a
        /// given build target, being passed the build options, a float from
        /// 0..1 indicating how far through the process we are, and a bool which
        /// is false for the pre-call and true for the post-call. Return true to
        /// continue or false to abort.</param>
        /// <returns>True if the process completed fully or false if was cancelled by callback</returns>
        [Obsolete("Obsolete")]
        public static bool Build(Settings settings, Func<BuildPlayerOptions, float, bool, bool> callback) {

            List<BuildPlayerOptions> buildSteps = SelectedBuildOptions(settings);
            int i = 1;
            foreach (BuildPlayerOptions opts in buildSteps) {
                if (callback != null &&
                    !callback(opts, i / (float)buildSteps.Count, false)) {
                    return false; // cancelled
                }
#if UNITY_2018_1_OR_NEWER
                BuildReport report = BuildPipeline.BuildPlayer(opts);
                string err = report.summary.result == BuildResult.Succeeded ? string.Empty : "See log";
#elif UNITY_5_5_OR_NEWER
                var err = BuildPipeline.BuildPlayer(opts);
#else
                string err = BuildPipeline.BuildPlayer(
                        opts.scenes,
                        opts.locationPathName,
                        opts.target,
                        opts.options);
#endif
                if (!string.IsNullOrEmpty(err)) {
                    throw new InvalidOperationException(string.Format("Build error: {0}", err));
                }
                ++i;
                if (callback != null &&
                    !callback(opts, i / (float)buildSteps.Count, true)) {
                    return false; // cancelled
                }
            }
            return true;
        }

        [Obsolete("Obsolete")]
        public static BuildTargetGroup GroupForTarget(BuildTarget t) {
            // Can't believe Unity doesn't have a method for this already
            switch (t) {
            case StandaloneLinux:
            case StandaloneLinux64:
            case StandaloneLinuxUniversal:
            case StandaloneOSX:
            case StandaloneWindows:
            case StandaloneWindows64:
                return BuildTargetGroup.Standalone;
            case iOS:
                return BuildTargetGroup.iOS;
            case Android:
                return BuildTargetGroup.Android;
            case WebGL:
                return BuildTargetGroup.WebGL;
            case WSAPlayer:
                return BuildTargetGroup.WSA;
            case Tizen:
                return BuildTargetGroup.Tizen;
            case PS4:
                return BuildTargetGroup.PS4;
            case XboxOne:
                return BuildTargetGroup.XboxOne;
            case WiiU:
                return BuildTargetGroup.WiiU;
            case tvOS:
                return BuildTargetGroup.tvOS;
#if UNITY_5_5_OR_NEWER
            case N3DS:
                return BuildTargetGroup.N3DS;
#else
            case BuildTarget.Nintendo3DS:
                return BuildTargetGroup.Nintendo3DS;
#endif
#if UNITY_5_6_OR_NEWER
            case Switch:
                return BuildTargetGroup.Switch;
#endif
                // TODO more platforms?
            case StandaloneOSXUniversal:
            case StandaloneOSXIntel:
            case WebPlayer:
            case WebPlayerStreamed:
            case PS3:
            case XBOX360:
            case WP8Player:
            case StandaloneOSXIntel64:
            case BlackBerry:
            case PSP2:
            case PSM:
            case SamsungTV:
            case Lumin:
            case Stadia:
            case CloudRendering:
            case GameCoreScarlett:
            case GameCoreXboxOne:
            case PS5:
            case EmbeddedLinux:
            case BuildTarget.iPhone:
            case NoTarget:
            default:
                return BuildTargetGroup.Unknown;
            }
        }

        [Obsolete("Obsolete")]
        static BuildTarget UnityTarget(Target t) {
            switch (t) {
            case Target.Win32:
                return StandaloneWindows;
            case Target.Win64:
                return StandaloneWindows64;
            case Target.Mac:
            case Target.Mac32:
                return StandaloneOSX;
            case Target.MacUniversal:
                return StandaloneOSX;
            case Target.Linux32:
                return StandaloneLinux;
            case Target.Linux64:
                return StandaloneLinux64;
            case Target.IOS:
                return iOS;
            case Target.Android:
                return Android;
            case Target.WebGL:
                return WebGL;
            case Target.WinStore:
                return WSAPlayer;
            case Target.Tizen:
                return Tizen;
            case Target.PS4:
                return PS4;
            case Target.XboxOne:
                return XboxOne;
            case Target.WiiU:
                return WiiU;
            case Target.Tvos:
                return tvOS;
#if UNITY_5_5_OR_NEWER
            case Target.Nintendo3Ds:
                return N3DS;
#else
            case Target.Nintendo3DS:
                return BuildTarget.Nintendo3DS;
#endif
#if UNITY_5_6_OR_NEWER
            case Target.Switch:
                return Switch;
#endif
                // TODO more platforms?
            case Target.SamsungTV:
            default:
                throw new NotImplementedException("Target not supported");
            }
        }

        [Obsolete("Obsolete")]
        static List<BuildPlayerOptions> SelectedBuildOptions(Settings settings) {
            return settings.targets.Select(target => BuildOpts(settings, target)).ToList();
        }

        [Obsolete("Obsolete")]
        static BuildPlayerOptions BuildOpts(Settings settings, Target target) {
            BuildPlayerOptions o = new BuildPlayerOptions {
                // Build all the scenes selected in build settings
                scenes = EditorBuildSettings.scenes
                    .Where(x => x.enabled)
                    .Select(x => x.path)
                    .ToArray()
            };
            string subfolder = target.ToString();
            o.locationPathName = Path.Combine(settings.outputFolder, subfolder);
            // location needs to include the output name too
            o.locationPathName = Path.Combine(o.locationPathName, settings.useProductName ? PlayerSettings.productName : settings.overrideName);
            // Need to append exe in Windows, isn't added by default
            // Weirdly .app is added automatically for Mac
            if (target is Target.Win32 or Target.Win64)
                o.locationPathName += ".exe";

            o.target = UnityTarget(target);
            BuildOptions opts = BuildOptions.None;
            if (settings.developmentBuild)
                opts |= BuildOptions.Development;
            o.options = opts;

            return o;
        }

    }

}