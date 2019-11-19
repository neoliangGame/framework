using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NEO.MODULE
{
    /// <summary>
    /// 多语言方案
    /// </summary>
    /// 这个类不需要创建和引用，直接通过instance获取
    /// 唯一需要外部调用的操作就是ChangeLanguage()
    public class Localization
    {
        Language language = Language.None;
        Dictionary<LocalizationData.ID, string> IDtoContent;
        UnityAction OnLanguageChanged;

        static Localization _instance = null;
        public static Localization instance
        {
            get {
                if(_instance == null)
                {
                    _instance = new Localization();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        void Initialize()
        {
            IDtoContent = new Dictionary<LocalizationData.ID, string>();
            language = Language.None;
        }

        /// <summary>
        /// 改变语言
        /// 同时还需要外部传递对应语言文件内容
        /// </summary>
        /// <param name="language">要切换到的语言</param>
        /// <param name="data">语言文件内容</param>
        public void ChangeLanguage(Language language, LocalizationData data)
        {
            if (this.language == language || language == Language.None)
                return;
            this.language = language;

            IDtoContent.Clear();
            IDtoContent = new Dictionary<LocalizationData.ID, string>();
            for(int i = 0;i < data.items.Length; ++i)
            {
                IDtoContent[data.items[i].id] = data.items[i].content;
            }

            OnLanguageChanged?.Invoke();
        }

        public void AddLanguageChangeListener(UnityAction languageChangeListener)
        {
            OnLanguageChanged += languageChangeListener;
        }
        public void RemoveLanguageChangeListener(UnityAction languageChangeListener)
        {
            OnLanguageChanged -= languageChangeListener;
        }

        public string Content(LocalizationData.ID id)
        {
            if (IDtoContent.TryGetValue(id, out string content))
            {
                return content;
            }
            return "";
        }

        public Language CurrentLanguage()
        {
            return language;
        }

        public enum Language
        {
            None,
            /// <summary>
            /// 英文
            /// </summary>
            English,
            /// <summary>
            /// 简体中文
            /// </summary>
            Chinese_Simple,
            /// <summary>
            /// 繁体中文
            /// </summary>
            Chinese_Traditional,

            //......
        }
    }
}

