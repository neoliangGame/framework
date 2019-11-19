using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageData : ScriptableObject
{
    public IntItem[] intItems;
    [System.Serializable]
    public class IntItem
    {
        public string name;
        public int value;
    }

    public FloatItem[] floatItems;
    [System.Serializable]
    public class FloatItem
    {
        public string name;
        public float value;
    }

    public StringItem[] stringItems;
    [System.Serializable]
    public class StringItem
    {
        public string name;
        public string value;
    }
}
