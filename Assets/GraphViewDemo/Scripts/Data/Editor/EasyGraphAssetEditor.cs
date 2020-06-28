using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

namespace AillieoUtils.GraphViewDemo.Editor
{
    [CustomEditor(typeof(EasyGraphAsset))]
    public class EasyGraphAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            DrawDefaultInspector();
            EditorGUI.EndDisabledGroup();
        }

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceID);
            return TryOpenGraphViewWindow(path);
        }

        private static bool TryOpenGraphViewWindow(string path)
        {
            var extension = Path.GetExtension(path);
            extension = extension.Substring(1);

            if (extension != "asset")
            {
                return false;
            }

            var guid = AssetDatabase.AssetPathToGUID(path);
            var foundWindow = false;
            foreach (var w in Resources.FindObjectsOfTypeAll<EasyGraphWindow>())
            {
                if (w.AssetGuid == guid)
                {
                    foundWindow = true;
                    w.Focus();
                    break;
                }
            }

            if (!foundWindow)
            {
                var window = CreateInstance<EasyGraphWindow>();
                window.Show();
                window.Initialize(guid);
            }
            return true;
        }

    }
}

