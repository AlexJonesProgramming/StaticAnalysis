using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JLi_Utility
{
#if UNITY_EDITOR
    public class SetPlayerIcons : EditorWindow
    {
        private static List<BuildTargetGroup> targetGroups = new List<BuildTargetGroup>() { BuildTargetGroup.Android, BuildTargetGroup.iOS };

        public bool showDebug = true;
        public Texture2D tex2d;

        private void SetAllIcons()
        {
            foreach(BuildTargetGroup group in targetGroups)
            {
                SetIcons(group);
            }

            Debugger.Log("Completed!");
        }

        private void SetIcons(BuildTargetGroup platform)
        {
            if(tex2d == null)
            {
                Debugger.LogError("There is no image assigned! Cannot set player icons.");
                return;
            }

            Debugger.ShowLog(showDebug, string.Format("Attempting to set Player Icons in Player Settings for target '{0}'...", platform.ToString()));

            Texture2D[] icons = PlayerSettings.GetIconsForTargetGroup(platform);
            for(int i = 0; i < icons.Length; i++)
            {
                icons[i] = tex2d;
            }
            PlayerSettings.SetIconsForTargetGroup(platform, icons);

            Debugger.ShowLog(showDebug, "\tDone!");
        }

        private void OnGUI()
        {
            showDebug = EditorGUILayout.Toggle("Show Debug", showDebug);
            tex2d = (Texture2D)EditorGUILayout.ObjectField("Icon:", tex2d, typeof(Texture2D), false);

            if (GUILayout.Button("Set Player Icons"))
                SetAllIcons();
        }

        [MenuItem("Window/SetPlayerIcons")]
        public static void ShowWindow() => GetWindow(typeof(SetPlayerIcons), false, nameof(SetPlayerIcons));
    }
#endif
}