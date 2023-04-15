using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utils.MultiBuild.Editor {

    public static class Storage {

        const string SettingsFilePath = "Assets/MultiBuild/MultiBuildSettings.asset";

        /// <summary>
        /// Try to load saved settings
        /// </summary>
        /// <returns>Settings instance if present, or null if not</returns>
        public static Settings LoadSettings() {
            return AssetDatabase.LoadAssetAtPath(SettingsFilePath, typeof(Settings)) as Settings;
        }

        /// <summary>
        /// Try to load settings, and if they do not exist, create new instance
        /// </summary>
        /// <returns>Loaded settings, or new instance. From this point will be saved directly.</returns>
        public static Settings LoadOrCreateSettings() {

            // try to load first
            Settings s = LoadSettings();

            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            if (s != null)
                return s;
            // Create new
            s = ScriptableObject.CreateInstance<Settings>();
            s.Reset();
            // Should not save during play, probably won't happen but check
            if (EditorApplication.isPlayingOrWillChangePlaymode) {
                EditorApplication.delayCall += () => CreateNewSettingsAsset(s);
            } else {
                CreateNewSettingsAsset(s);
            }
            return s;
        }


        static void CreateNewSettingsAsset(Object s) {
            string dir = Path.GetDirectoryName(SettingsFilePath);
            if(!Directory.Exists(dir)) {
                if (dir != null)
                    Directory.CreateDirectory(dir);
            }
            AssetDatabase.CreateAsset(s, SettingsFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // After this the settings asset is saved with other assets
        }
    }
}