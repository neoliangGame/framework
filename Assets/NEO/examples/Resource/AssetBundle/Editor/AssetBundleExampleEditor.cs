using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class AssetBundleExampleEditor
{
    [MenuItem("NEO/Resource/Create AssetBundle Data")]
    static void CreateResourceData()
    {
        string title = "AssetBundleData";
        string directory = "";
        string defaultName = "assetBundleData";
        string extension = "asset";

        string path = EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
        int startIndex = path.IndexOf("/Assets/");
        path = path.Substring(startIndex + 1);
        NEO.SUPPORT.Utility.CreateAsset<NEO.MODULE.AssetBundleData>(path);
        Debug.Log("Create AssetBundleData Succeed : " + path);
    }
}
