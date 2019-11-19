using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace NEO.MODULE
{
    /// <summary>
    /// 多语言设置组件自定义Inspector界面
    /// 目的：为了方便Editor模式下编辑数据和查看效果
    /// 功能：
    ///     1。直接通过id切换文本内容；
    ///     2。直接通过language切换语言显示内容；
    ///     3。可以直接在对应Text中编辑文本内容
    ///         点击【save change】会保存对应language的id的内容的修改
    /// </summary>
    [CustomEditor(typeof(LocalizationConfiguration))]
    public class LocalizationConfigurationEditor : Editor
    {
        //这里要根据实际存放位置进行修改
        #region 语言文件资源路径
        const string rootPath = "Assets/NEO/examples/Localization/Resources/language/";
        const string suffix = ".asset";

        string LanguageAssetPath(Localization.Language language)
        {
            string assetName = "english";
            switch (language)
            {
                case Localization.Language.English:
                    assetName = "english";
                    break;
                case Localization.Language.Chinese_Simple:
                    assetName = "chinese_simple";
                    break;
                case Localization.Language.Chinese_Traditional:
                    assetName = "chinese_traditional";
                    break;
                //......
            }
            return rootPath + assetName + suffix;
        }
        #endregion

        SerializedProperty id;

        private void OnEnable()
        {
            id = serializedObject.FindProperty("id");
        }

        override public void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(id);
            bool idChanged = EditorGUI.EndChangeCheck();
            if (idChanged)
            {
                ChangeID();
            }
            EditorGUILayout.Space();

            System.Enum language = EditorGUILayout.EnumPopup("language", Localization.instance.CurrentLanguage());
            if((Localization.Language)language != Localization.instance.CurrentLanguage())
            {
                ChangeLanguage((Localization.Language)language);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("save change"))
            {
                SaveChange();
            }
        }

        void ChangeID()
        {
            ((LocalizationConfiguration)target).id = (LocalizationData.ID)id.intValue;
            ((LocalizationConfiguration)target).GetComponent<Text>().text = Localization.instance.Content((LocalizationData.ID)id.intValue);
        }

        void ChangeLanguage(Localization.Language language)
        {
            string path = LanguageAssetPath(language);
            LocalizationData languageData = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);
            Localization.instance.ChangeLanguage(language, languageData);
        }

        void SaveChange()
        {
            string path = LanguageAssetPath(Localization.instance.CurrentLanguage());
            LocalizationData languageData = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);
            LocalizationData.ID id = (LocalizationData.ID)this.id.intValue;
            for(int i = 0;i < languageData.items.Length; ++i)
            {
                if(languageData.items[i].id == id)
                {
                    languageData.items[i].content = ((LocalizationConfiguration)target).GetComponent<Text>().text;
                    break;
                }
            }
            EditorUtility.SetDirty(languageData);
            AssetDatabase.SaveAssets();
        }

    }
}

