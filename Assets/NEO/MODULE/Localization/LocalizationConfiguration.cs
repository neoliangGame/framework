using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NEO.MODULE
{
    /// <summary>
    /// 多语言设置组件
    /// 添加到需要有多语言功能的Text组件对应物体上
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class LocalizationConfiguration : MonoBehaviour
    {
        public LocalizationData.ID id = LocalizationData.ID.None;

        private void OnEnable()
        {
            Localization.instance.AddLanguageChangeListener(OnLanguageChanged);
        }

        private void OnDisable()
        {
            Localization.instance.RemoveLanguageChangeListener(OnLanguageChanged);
        }

        public void OnLanguageChanged()
        {
            string content = Localization.instance.Content(id);
            GetComponent<Text>().text = content;
        }
    }
}
