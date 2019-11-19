using UnityEngine;
using UnityEngine.Events;

namespace NEO.MODULE
{
    /// <summary>
    /// 面板基类
    /// </summary>
    /// 实际面板类继承UIBase
    /// 把实际面板类挂载在对应面板GameObject的跟节点
    /// 
    /// 同时根据实际情况添加面板的展示和关闭动画，如果要添加，则：
    ///     在挂载此面板类的GameObject中挂载Animator组件
    ///     Animator中包含【show】和【close】两个状态
    ///     每个状态的AnimationClip结束帧添加动画事件
    ///     show的clip结束添加ShowEnd()事件
    ///     close的clip结束添加CloseEnd()事件
    /// 
    /// 只需实现：
    ///     OnInitialize()
    ///     OnShow()
    ///     OnClose()
    ///     OnDestroyPanel()
    ///     可选：
    ///     OnShowEnd()
    ///     OnCloseEnd()
    public class UIBase : MonoBehaviour
    {
        [SerializeField]
        UIPanelType panelType = UIPanelType.None;
        public UIPanelType GetPanelType()
        {
            return panelType;
        }

        [SerializeField]
        UIPanelLayer panelLayer = UIPanelLayer.None;
        public UIPanelLayer GetPanelLayer()
        {
            return panelLayer;
        }

        //单个生命周期内有效，下一次打开需要重新设置
        UnityAction showStartListener = null;
        UnityAction showEndListener = null;
        UnityAction closeStartListener = null;
        UnityAction closeEndListener = null;

        Animator animator = null;

        UIManager manager = null;
        public void Initialize(UIManager manager)
        {
            this.manager = manager;
            animator = GetComponent<Animator>();
            ClearListeners();
            OnInitialize();
        }

        void ClearListeners()
        {
            showStartListener = null;
            showEndListener = null;
            closeStartListener = null;
            closeEndListener = null;
        }

        protected virtual void OnInitialize()
        {
            //初始化：只执行一次，子类覆盖，自定义内容......
        }

        public void AddShowStartListener(UnityAction showStartListener)
        {
            this.showStartListener += showStartListener;
        }
        public void AddShowEndListener(UnityAction showEndListener)
        {
            this.showEndListener += showEndListener;
        }
        public void AddCloseStartListener(UnityAction closeStartListener)
        {
            this.closeStartListener += closeStartListener;
        }
        public void AddCloseEndListener(UnityAction closeEndListener)
        {
            this.closeEndListener += closeEndListener;
        }

        public void Show()
        {
            if(panelType == UIPanelType.Back)
            {
                manager.AddBackPanel(this);
            }
            OnShow();
            showStartListener?.Invoke();
            gameObject.SetActive(true);
            if(animator != null)
            {
                animator.Play(UIConstant.ANIMATION_SHOW);
            }
            else
            {
                ShowEnd();
            }
        }
        protected virtual void OnShow()
        {
            //开始展示面板：子类覆盖，根据实际实现......
        }

        public void ShowEnd()
        {
            showEndListener?.Invoke();
            OnShowEnd();
        }
        protected virtual void OnShowEnd()
        {
            //开始展示面板动画结束：子类覆盖，根据实际实现......
        }

        public void Close()
        {
            closeStartListener?.Invoke();
            if (animator != null)
            {
                animator.Play(UIConstant.ANIMATION_CLOSE);
            }
            else
            {
                gameObject.SetActive(false);
                CloseEnd();
            }
            OnClose();
        }
        protected virtual void OnClose()
        {
            //开始关闭面板：子类覆盖，根据实际实现......
        }

        public void CloseEnd()
        {
            if (panelType == UIPanelType.Back)
            {
                manager.PopBackPanel();
            }

            gameObject.SetActive(false);
            closeEndListener?.Invoke();
            OnCloseEnd();
        }
        protected virtual void OnCloseEnd()
        {
            //关闭面板结束：子类覆盖，根据实际实现......
        }

        public void DestroyPanel()
        {
            manager.DestroyPanel(this);
            OnDestryPanel();
        }
        protected virtual void OnDestryPanel()
        {
            //销毁面板：子类覆盖，根据实际实现......
        }
    }
}

