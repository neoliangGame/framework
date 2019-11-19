using UnityEngine;
using NEO.MODULE;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIExample : MonoBehaviour
{
    UIManager manager;
    public Canvas root;

    private void Awake()
    {
        manager = new UIManager(root);
        manager.SetResourceHandler(LoadPanel);
    }

    UIBase LoadPanel(UIPanelID id)
    {
        GameObject panelPrefab = null;
        GameObject panelGO = null;
        UIBase panel = null;
#if UNITY_EDITOR
        switch (id)
        {
            case UIPanelID.Test1:
                panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/NEO/examples/UI/panel1.prefab");
                break;
            case UIPanelID.Test2:
                panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/NEO/examples/UI/panel2.prefab");
                break;
            case UIPanelID.Test3:
                panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/NEO/examples/UI/panel3.prefab");
                break;
            case UIPanelID.Test4:
                panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/NEO/examples/UI/panel4.prefab");
                break;
            case UIPanelID.Test5:
                panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/NEO/examples/UI/panel5.prefab");
                break;
        }
        panelGO = GameObject.Instantiate(panelPrefab);
        panel = panelGO.GetComponent<UIBase>();
#endif
        return panel;
    }

    public void ShowPanel1()
    {
        UIExamplePanel1 panel = manager.Create(UIPanelID.Test1) as UIExamplePanel1;
        panel.example = this;
        panel.Show();

    }

    public void ShowPanel2()
    {
        manager.Create(UIPanelID.Test2).Show();
    }

    public void ShowPanel3()
    {
        manager.Create(UIPanelID.Test3).Show();
    }

    public void ShowPanel4()
    {
        manager.Create(UIPanelID.Test4).Show();
    }

    public void ShowPanel5()
    {
        manager.Create(UIPanelID.Test5).Show();
    }
    
    public void ClearBackPanels()
    {
        manager.ClearBackPanels();
    }

    public void OpenPanel2WithParameter(int para)
    {
        UIExamplePanel2 panel = manager.Create(UIPanelID.Test2) as UIExamplePanel2;
        panel.para = para;
        panel.Show();
    }

    public void OpenPanel3WithCloseEndListener(UnityAction closeEndListener)
    {
        UIExamplePanel3 panel = manager.Create(UIPanelID.Test3) as UIExamplePanel3;
        panel.AddCloseEndListener(closeEndListener);
        panel.Show();
    }

}
