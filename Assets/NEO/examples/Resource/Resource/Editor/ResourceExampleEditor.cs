using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class ResourceExampleEditor
{
    [MenuItem("NEO/Resource/Create Resource Data")]
    static void CreateResourceData()
    {
        string title = "ResourceData";
        string directory = "";
        string defaultName = "resourceData";
        string extension = "asset";

        string path = EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
        int startIndex = path.IndexOf("/Assets/");
        path = path.Substring(startIndex+1);
        NEO.SUPPORT.Utility.CreateAsset<NEO.MODULE.ResourceData>(path);
        Debug.Log("Create ResourceData Succeed : " + path);
    }
}
