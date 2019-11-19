using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationExampleData : ScriptableObject
{
    public Item[] items;

    [System.Serializable]
    public class Item
    {
        public int testInt;
        public TestEnum testEnum;
        public float testFloat;
        public string testString;
    }

    public enum TestEnum
    {
        None,
        Test1,
        Test2,
    }
}
