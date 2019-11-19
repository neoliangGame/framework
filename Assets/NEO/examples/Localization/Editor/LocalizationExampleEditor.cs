using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalizationExampleEditor
{
    [MenuItem("NEO/Localization/Create Localization Data")]
    static void CreateResourceData()
    {
        string title = "LocalizationData";
        string directory = "";
        string defaultName = "localizationData";
        string extension = "asset";

        string path = EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
        int startIndex = path.IndexOf("/Assets/");
        path = path.Substring(startIndex + 1);
        NEO.SUPPORT.Utility.CreateAsset<NEO.MODULE.LocalizationData>(path);
        Debug.Log("Create LocalizationData Succeed : " + path);
    }
}
