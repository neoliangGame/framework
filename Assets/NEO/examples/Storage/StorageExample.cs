using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEO.MODULE;

public class StorageExample : MonoBehaviour
{
    public string valueName;
    public Text text;

    public void SetInt(int value)
    {
        Storage.SetInt(valueName, value);
    }

    public void GetInt()
    {
        int value = Storage.GetInt(valueName);
        text.text = valueName + " = " + value.ToString();
    }

    public void SetFloat(float value)
    {
        Storage.SetFloat(valueName, value);
    }

    public void GetFloat()
    {
        float value = Storage.GetFloat(valueName);
        text.text = valueName + " = " + value.ToString();
    }

    public void SetString(string value)
    {
        Storage.SetString(valueName, value);
    }

    public void GetString()
    {
        string value = Storage.GetString(valueName);
        text.text = valueName + " = " + value;
    }

}
