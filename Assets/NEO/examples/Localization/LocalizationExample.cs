using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEO.MODULE;

public class LocalizationExample : MonoBehaviour
{
    //正式项目需要用资源管理系统管理
    //这里是为了不牵连耦合其他模块，简单了事
    #region 语言资源文件获取
    const string rootPath = "language/";

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
        return rootPath + assetName;
    }

    LocalizationData GetLanguageAsset(Localization.Language language)
    {
        string path = LanguageAssetPath(language);
        LocalizationData languageAsset = Resources.Load<LocalizationData>(path);
        return languageAsset;
    }
    #endregion

    public Localization.Language language = Localization.Language.None;

    public void ChangeLanguage()
    {
        if (language == Localization.Language.None)
            return;
        LocalizationData languageAsset = GetLanguageAsset(language);
        Localization.instance.ChangeLanguage(language, languageAsset);
    }

    private void Start()
    {
        ChangeLanguage();
    }
}
