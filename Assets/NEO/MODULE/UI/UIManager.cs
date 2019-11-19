using System.Collections.Generic;
using UnityEngine;

namespace NEO.MODULE
{
    /// <summary>
    /// UI面板管理
    /// </summary>
    /// 提供自动层级管理方案：
    ///     给定三个大层：
    ///         bottom
    ///         middle
    ///         top
    ///     细分层通过动态设置当前展示的面板为最上层
    /// 提供返回堆栈功能；
    /// 提供面板生命周期事件：
    ///     面板开始展示
    ///     面板展示动画播放结束
    ///     面板开始关闭
    ///     面板关闭动画播放结束
    public class UIManager
    {
        Dictionary<UIPanelID, UIBase> IDtoPANEL;
        List<UIBase> backPanels;
        public delegate UIBase ResourceHandler(UIPanelID id);
        ResourceHandler resourceHandler;

        Transform root;
        Transform topRoot;
        Transform middleRoot;
        Transform bottomRoot;

        public UIManager(Canvas root)
        {
            IDtoPANEL = new Dictionary<UIPanelID, UIBase>();
            backPanels = new List<UIBase>();

            this.root  = root.transform;
            bottomRoot = CreateLayerRoot("bottom");
            middleRoot = CreateLayerRoot("middle");
            topRoot    = CreateLayerRoot("top");
        }

        Transform CreateLayerRoot(string name)
        {
            Transform layerRoot = new GameObject(name).transform;
            layerRoot.parent = root;
            layerRoot.localPosition = Vector3.zero;
            layerRoot.localEulerAngles = Vector3.zero;
            layerRoot.localScale = Vector3.one;
            layerRoot.SetAsLastSibling();

            RectTransform layerRect = layerRoot.gameObject.AddComponent<RectTransform>();
            layerRect.anchoredPosition = Vector2.one * 0.5f;
            layerRect.anchorMin = Vector2.zero;
            layerRect.anchorMax = Vector2.one;
            layerRect.offsetMin = Vector2.zero;
            layerRect.offsetMax = Vector2.zero;

            return layerRect.transform;
        }

        public void SetResourceHandler(ResourceHandler resourceHandler)
        {
            this.resourceHandler = resourceHandler;
        }

        #region 面板
        public UIBase Create(UIPanelID id)
        {
            if(IDtoPANEL.TryGetValue(id,out UIBase panel))
            {
                return panel;
            }

            panel = resourceHandler(id);
            SetPanelParent(panel);
            SetPanelTransform(panel);
            panel.Initialize(this);
            IDtoPANEL[id] = panel;
            return panel;
        }

        void SetPanelParent(UIBase panel)
        {
            switch (panel.GetPanelLayer())
            {
                case UIPanelLayer.Bottom:
                    panel.transform.parent = bottomRoot;
                    break;
                case UIPanelLayer.Middle:
                    panel.transform.parent = middleRoot;
                    break;
                case UIPanelLayer.Top:
                    panel.transform.parent = topRoot;
                    break;
            }
        }

        void SetPanelTransform(UIBase panel)
        {
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localEulerAngles = Vector3.zero;
            panel.transform.localScale = Vector3.one;
            panel.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            panel.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        }

        public void DestroyPanel(UIBase panel)
        {
            UIPanelID id = UIPanelID.None;
            Dictionary<UIPanelID, UIBase>.Enumerator enutor = IDtoPANEL.GetEnumerator();
            while (enutor.MoveNext())
            {
                if(enutor.Current.Value == panel)
                {
                    id = enutor.Current.Key;
                    break;
                }
            }
            IDtoPANEL.Remove(id);

            backPanels.Remove(panel);
        }
        #endregion

        #region 返回堆栈
        public void AddBackPanel(UIBase panel)
        {
            backPanels.Add(panel);
            panel.transform.SetAsLastSibling();
        }

        public UIBase PopBackPanel()
        {
            if(backPanels.Count > 0)
            {
                UIBase topPanel = backPanels[backPanels.Count - 1];
                backPanels.Remove(topPanel);
                if(backPanels.Count > 0)
                {
                    backPanels[backPanels.Count - 1].gameObject.SetActive(true);
                    backPanels[backPanels.Count - 1].transform.SetAsLastSibling();
                }
                return topPanel;
            }
            return null;
        }

        public void ClearBackPanels()
        {
            for(int i = 0;i < backPanels.Count; ++i)
            {
                backPanels[i].gameObject.SetActive(false);
            }
            backPanels.Clear();
        }
        #endregion
    }
}

