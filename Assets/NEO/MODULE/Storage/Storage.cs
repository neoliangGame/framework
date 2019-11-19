using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NEO.MODULE
{
    /// <summary>
    /// 用户数据存取
    /// </summary>
    /// 开发环境：用AssetDataBase把数据存放在工程项目中，方便操作：复制，删除，修改
    /// 发布环境：直接用Unity默认的PlayerPrefs
    public class Storage
    {
        public static int GetInt(string intName, int defaultValue = 0)
        {
#if UNITY_EDITOR
            if(NAMEtoINDEX_INT.TryGetValue(intName, out int index) == false)
            {
                StorageData.IntItem[] newIntItems = new StorageData.IntItem[data.intItems.Length+1];
                data.intItems.CopyTo(newIntItems, 0);
                newIntItems[data.intItems.Length] = new StorageData.IntItem() {
                    name = intName,value = defaultValue
                };
                index = data.intItems.Length;
                NAMEtoINDEX_INT[intName] = index;
                data.intItems = newIntItems;
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
            }
            return data.intItems[index].value;
#endif
            if (PlayerPrefs.HasKey(intName) == false)
            {
                PlayerPrefs.SetInt(intName, defaultValue);
            }
            return PlayerPrefs.GetInt(intName);
        }

        public static void SetInt(string intName, int value)
        {
#if UNITY_EDITOR
            if (NAMEtoINDEX_INT.TryGetValue(intName, out int index) == false)
            {
                StorageData.IntItem[] newIntItems = new StorageData.IntItem[data.intItems.Length + 1];
                data.intItems.CopyTo(newIntItems, 0);
                newIntItems[data.intItems.Length] = new StorageData.IntItem()
                {
                    name = intName,
                    value = value
                };
                NAMEtoINDEX_INT[intName] = data.intItems.Length;
                data.intItems = newIntItems;
            }
            else
            {
                data.intItems[index].value = value;
            }
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            return;
#endif
            PlayerPrefs.SetInt(intName, value);
        }

        public static float GetFloat(string floatName, float defaultValue = 0f)
        {
#if UNITY_EDITOR
            if (NAMEtoINDEX_FLOAT.TryGetValue(floatName, out int index) == false)
            {
                StorageData.FloatItem[] newFloatItems = new StorageData.FloatItem[data.floatItems.Length + 1];
                data.floatItems.CopyTo(newFloatItems, 0);
                newFloatItems[data.floatItems.Length] = new StorageData.FloatItem()
                {
                    name = floatName,
                    value = defaultValue
                };
                index = data.floatItems.Length;
                NAMEtoINDEX_FLOAT[floatName] = index;
                data.floatItems = newFloatItems;
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
            }
            return data.floatItems[index].value;
#endif
            if (PlayerPrefs.HasKey(floatName) == false)
            {
                PlayerPrefs.SetFloat(floatName, defaultValue);
            }
            return PlayerPrefs.GetFloat(floatName);
        }

        public static void SetFloat(string floatName, float value)
        {
#if UNITY_EDITOR
            if (NAMEtoINDEX_FLOAT.TryGetValue(floatName, out int index) == false)
            {
                StorageData.FloatItem[] newFloatItems = new StorageData.FloatItem[data.floatItems.Length + 1];
                data.floatItems.CopyTo(newFloatItems, 0);
                newFloatItems[data.floatItems.Length] = new StorageData.FloatItem()
                {
                    name = floatName,
                    value = value
                };
                NAMEtoINDEX_FLOAT[floatName] = data.floatItems.Length;
                data.floatItems = newFloatItems;
            }
            else
            {
                data.floatItems[index].value = value;
            }
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            return;
#endif
            PlayerPrefs.SetFloat(floatName, value);
        }

        public static string GetString(string stringName, string defaultValue = "")
        {
#if UNITY_EDITOR
            if (NAMEtoINDEX_STRING.TryGetValue(stringName, out int index) == false)
            {
                StorageData.StringItem[] newStringItems = new StorageData.StringItem[data.stringItems.Length + 1];
                data.stringItems.CopyTo(newStringItems, 0);
                newStringItems[data.stringItems.Length] = new StorageData.StringItem()
                {
                    name = stringName,
                    value = defaultValue
                };
                index = data.stringItems.Length;
                NAMEtoINDEX_STRING[stringName] = index;
                data.stringItems = newStringItems;
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
            }
            return data.stringItems[index].value;
#endif
            if (PlayerPrefs.HasKey(stringName) == false)
            {
                PlayerPrefs.SetString(stringName, defaultValue);
            }
            return PlayerPrefs.GetString(stringName);
        }

        public static void SetString(string stringName, string value)
        {
#if UNITY_EDITOR
            if (NAMEtoINDEX_STRING.TryGetValue(stringName, out int index) == false)
            {
                StorageData.StringItem[] newStringItems = new StorageData.StringItem[data.stringItems.Length + 1];
                data.stringItems.CopyTo(newStringItems, 0);
                newStringItems[data.stringItems.Length] = new StorageData.StringItem()
                {
                    name = stringName,
                    value = value
                };
                NAMEtoINDEX_STRING[stringName] = data.stringItems.Length;
                data.stringItems = newStringItems;
            }
            else
            {
                data.stringItems[index].value = value;
            }
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            return;
#endif
            PlayerPrefs.SetString(stringName, value);
        }

#if UNITY_EDITOR
        //这个路径根据实际需求修改
        const string dataPath = "Assets/NEO/examples/Storage/";
        const string dataName = "data.asset";
        static StorageData _data = null;
        static StorageData data
        {
            get {
                if (_data == null)
                {
                    if(Directory.Exists(dataPath) == false)
                    {
                        Directory.CreateDirectory(dataPath);
                    }
                    _data = AssetDatabase.LoadAssetAtPath<StorageData>(dataPath + dataName);
                    if(_data == null)
                    {
                        _data = new StorageData();
                        _data.intItems = new StorageData.IntItem[0];
                        _data.floatItems = new StorageData.FloatItem[0];
                        _data.stringItems = new StorageData.StringItem[0];
                        AssetDatabase.CreateAsset(_data, dataPath + dataName);
                    }
                }
                return _data;
            }
        }

        static Dictionary<string, int> _NAMEtoINDEX_INT = null;
        static Dictionary<string, int> NAMEtoINDEX_INT
        {
            get
            {
                if (_NAMEtoINDEX_INT == null)
                {
                    _NAMEtoINDEX_INT = new Dictionary<string, int>();
                    for(int i = 0;i < data.intItems.Length; i++)
                    {
                        _NAMEtoINDEX_INT[data.intItems[i].name] = i;
                    }
                }
                return _NAMEtoINDEX_INT;
            }
        }

        static Dictionary<string, int> _NAMEtoINDEX_FLOAT = null;
        static Dictionary<string, int> NAMEtoINDEX_FLOAT
        {
            get
            {
                if (_NAMEtoINDEX_FLOAT == null)
                {
                    _NAMEtoINDEX_FLOAT = new Dictionary<string, int>();
                    for (int i = 0; i < data.floatItems.Length; i++)
                    {
                        _NAMEtoINDEX_FLOAT[data.floatItems[i].name] = i;
                    }
                }
                return _NAMEtoINDEX_FLOAT;
            }
        }

        static Dictionary<string, int> _NAMEtoINDEX_STRING = null;
        static Dictionary<string, int> NAMEtoINDEX_STRING
        {
            get
            {
                if (_NAMEtoINDEX_STRING == null)
                {
                    _NAMEtoINDEX_STRING = new Dictionary<string, int>();
                    for (int i = 0; i < data.stringItems.Length; i++)
                    {
                        _NAMEtoINDEX_STRING[data.stringItems[i].name] = i;
                    }
                }
                return _NAMEtoINDEX_STRING;
            }
        }
#endif
    }
}

