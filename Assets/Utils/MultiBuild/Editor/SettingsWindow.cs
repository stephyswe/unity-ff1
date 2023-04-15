using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/* Easiest way to check if we're running 5.4 or lower. */
#if UNITY_5_5_OR_NEWER
#else
namespace UnityEditor
{
	public struct BuildPlayerOptions
	{
		public string[] scenes { get; set; }
		public string locationPathName { get; set; }
		public string assetBundleManifestPath { get; set; }
		public BuildTargetGroup targetGroup { get; set; }
		public BuildTarget target { get; set; }
		public BuildOptions options { get; set; }
	}
}
#endif

namespace Utils.MultiBuild.Editor {
	public class SettingsWindow : EditorWindow {

		// Manually format the descriptive names
		// Simpler than DescriptionAttribute style IMO
		static Dictionary<Target, string> _targetNames;
		static Dictionary<Target, string> TargetNames {
			get {
				return _targetNames ??= new Dictionary<Target, string> {
					{Target.Android, "Android"},
					{Target.IOS, "iOS"},
					{Target.Linux32, "Linux 32-bit"},
					{Target.Linux64, "Linux 64-bit"},
					{Target.Mac32, "Mac 32-bit"},
					{Target.Mac64, "Mac 64-bit"},
					{Target.MacUniversal, "Mac Universal"},
					{Target.WebGL, "WebGL"},
					{Target.Win32, "Windows 32-bit"},
					{Target.Win64, "Windows 64-bit"},
					{Target.WinStore, "Windows Store App"},
					{Target.Tizen, "Tizen"},
					{Target.PS4, "Playstation 4"},
					{Target.XboxOne, "Xbox One"},
					{Target.SamsungTV, "Samsung TV"},
					{Target.WiiU, "Nintendo WiiU"},
					{Target.Tvos, "tvOS"},
					{Target.Nintendo3Ds, "Nintendo 3DS"},
#if UNITY_5_6_OR_NEWER
					{Target.Switch, "Nintendo Switch"},
#endif
				};
			}
		}

		Settings settings;
		Settings Settings {
			get {
				// ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
				if (settings == null) {
					settings = Storage.LoadOrCreateSettings();
				}
				return settings;
			}
		}
		SerializedObject serializedSettings;
		SerializedObject SerializedSettings {
			get { return serializedSettings ??= new SerializedObject(Settings); }
		}

		// Because we need to sort and Unity Popup doesn't have a data tag
		Dictionary<string, Target> targetNameToValue;
		Dictionary<string, Target> TargetNameToValue {
			get {
				if (targetNameToValue != null)
					return targetNameToValue;
				targetNameToValue = new Dictionary<string, Target>();
				foreach (Target target in TargetNames.Keys) {
					targetNameToValue[TargetNames[target]] = target;
				}
				return targetNameToValue;
			}
		}



		Target[] targets;
		IEnumerable<Target> Targets {
			get { return targets ??= (Target[])Enum.GetValues(typeof(Target)); }
		}

		GUIStyle actionButtonStyle;
		GUIStyle ActionButtonStyle {
			get {
				if (actionButtonStyle != null)
					return actionButtonStyle;
				actionButtonStyle = new GUIStyle(GUI.skin.button) {
					fontStyle = FontStyle.Bold,
					normal = {
						textColor = Color.white
					}
				};
				return actionButtonStyle;
			}
		}
		GUIStyle labelMarginStyle;
		GUIStyle LabelMarginStyle {
			get {
				if (labelMarginStyle != null)
					return labelMarginStyle;
				labelMarginStyle = new GUIStyle {
					margin = {
						left = GUI.skin.label.margin.left
					}
				};
				return labelMarginStyle;
			}
		}
		GUIStyle removeButtonContainerStyle;
		GUIStyle RemoveButtonContainerStyle {
			get {
				return removeButtonContainerStyle ??= new GUIStyle {
					margin = {
						left = 30
					}
				};
			}
		}
		List<string> targetNamesNotAdded;
		bool targetsDirty = true;
		int targetToAddIndex;

		[MenuItem("Tools/MultiBuild...")]
		public static void ShowWindow() {
			GetWindow(typeof(SettingsWindow), false, "MultiBuild");
		}

		[Obsolete("Obsolete")]
		void OnGUI() {
			if (targetsDirty) {
				UpdateTargetsNotAdded();
			}

			// Use SerializedObject as a proxy to get dirty flags, undo & asset save
			GUILayout.Label("Output Settings", EditorStyles.boldLabel);

			// Need to use nested verticals and flexible space to align text & button vertically
			EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(GUI.skin.button.CalcHeight(new GUIContent("..."), 30)));
			// Also need a style on label to make it line up on the left edge
			EditorGUILayout.BeginVertical(LabelMarginStyle);
			GUILayout.FlexibleSpace();
			EditorGUILayout.PropertyField(SerializedSettings.FindProperty("outputFolder"));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) {
				SerializedProperty prop = SerializedSettings.FindProperty("outputFolder");
				string fld = EditorUtility.OpenFolderPanel("Pick build folder", prop.stringValue, PlayerSettings.productName);
				if (!string.IsNullOrEmpty(fld)) {
					prop.stringValue = fld;
				}
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(SerializedSettings.FindProperty("useProductName"));
			if (!Settings.useProductName) {
				EditorGUILayout.PropertyField(SerializedSettings.FindProperty("overrideName"));
			}
			EditorGUILayout.Space();


			GUILayout.Label("Platforms To Build", EditorStyles.boldLabel);

			bool removeTargetAtEnd = false;
			Target targetToRemove = Target.IOS;
			foreach (Target target in Settings.targets) {
				EditorGUILayout.BeginHorizontal(RemoveButtonContainerStyle, GUILayout.MaxHeight(23));
				EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
				GUILayout.FlexibleSpace();
				GUI.backgroundColor = new Color(0.6f, 0, 0, 1);
				if (GUILayout.Button(" X ", ActionButtonStyle)) {
					// Don't do this now, breaks iteration
					targetToRemove = target;
					removeTargetAtEnd = true;
				}
				GUI.backgroundColor = Color.white;
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				EditorGUILayout.LabelField(TargetNames[target]);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
			if (Settings.targets.Count == 0) {
				GUILayout.Label("No platforms selected! Add one below.", EditorStyles.centeredGreyMiniLabel);
			}
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(GUI.skin.button.CalcHeight(new GUIContent("..."), 30)));
			EditorGUILayout.BeginVertical(LabelMarginStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label("Add platform");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			targetToAddIndex = EditorGUILayout.Popup(targetToAddIndex, targetNamesNotAdded.ToArray());
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add")) {
				// Ugh dealing with arrays in SerializedObject is awful
				string newTargetName = targetNamesNotAdded[targetToAddIndex];
				Target newTarget = TargetNameToValue[newTargetName];
				// Insert in order
				// ReSharper disable once IdentifierTypo
				SerializedProperty proplist = SerializedSettings.FindProperty("targets");
				int insertIndex;
				for (insertIndex = 0; insertIndex < proplist.arraySize; ++insertIndex) {
					string targetName = TargetNames[(Target)proplist.GetArrayElementAtIndex(insertIndex).enumValueIndex];
					if (string.Compare(newTargetName, targetName, StringComparison.OrdinalIgnoreCase) < 0) {
						break;
					}
				}
				proplist.arraySize++;
				// Move all existing items forward to make room for insert in order
				for (int i = proplist.arraySize - 1; i > insertIndex; --i) {
					proplist.GetArrayElementAtIndex(i).enumValueIndex =
						proplist.GetArrayElementAtIndex(i - 1).enumValueIndex;
				}
				proplist.GetArrayElementAtIndex(insertIndex).enumValueIndex = (int)newTarget;
				targetsDirty = true;
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			GUILayout.Label("Additional options", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(SerializedSettings.FindProperty("developmentBuild"));

			if (removeTargetAtEnd) {
				int index = Settings.targets.IndexOf(targetToRemove);
				// ReSharper disable once IdentifierTypo
				SerializedProperty proplist = SerializedSettings.FindProperty("targets");
				proplist.DeleteArrayElementAtIndex(index);
				targetsDirty = true;
			}

			// This applies any changes to the underlying asset and marks dirty if needed
			// this is what ensures the asset gets saved
			SerializedSettings.ApplyModifiedProperties();
			if (targetsDirty) {
				Repaint();
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			GUI.backgroundColor = new Color(0, 0.6f, 0, 1);
			if (GUILayout.Button("Build Selected Platforms", ActionButtonStyle, GUILayout.MinHeight(30))) {
				// do eet
				Build();
			}
		}

		void UpdateTargetsNotAdded() {
			targetNamesNotAdded = new List<string>();
			foreach (Target target in Targets) {
				if (!Settings.targets.Contains(target))
					targetNamesNotAdded.Add(TargetNames[target]);
			}
			targetNamesNotAdded.Sort();
			targetsDirty = false;
		}

		[Obsolete("Obsolete")]
		void Build() {

			BuildTarget savedTarget = EditorUserBuildSettings.activeBuildTarget;

			bool ok;
			try {
				ok = Builder.Build(Settings, (opts, progress, done) => {
					string message = done ?
						string.Format("Building {0} Done", opts.target.ToString()) :
						string.Format("Building {0}...", opts.target.ToString());
					return !EditorUtility.DisplayCancelableProgressBar(
						"Building project...",
						message,
						progress);
					// cancel
				});
			} catch (Exception e) {
				EditorUtility.DisplayDialog("Build error", e.Message, "Close");
				ok = false;
			}

			EditorUtility.ClearProgressBar();
			if (!ok) {
				EditorUtility.DisplayDialog("Cancelled", "Build cancelled before finishing.", "Close");
			}

			// Building can change the active target, can cause warnings or odd behaviour
			// Put it back to how it was
			if (EditorUserBuildSettings.activeBuildTarget != savedTarget) {
#if UNITY_5_6_OR_NEWER
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(Builder.GroupForTarget(savedTarget), savedTarget);
#else
                EditorUserBuildSettings.SwitchActiveBuildTarget(savedTarget);
#endif
			}
		}

	}

}
