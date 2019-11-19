using UnityEngine.UI;
using NEO.MODULE;
using UnityEngine;

public class UIExamplePanel1 : UIBase
{
    [HideInInspector]
    public UIExample example;
    public Text title;
    protected override void OnInitialize()
    {
        title.text = "panel 1";
    }

    protected override void OnShow()
    {
        title.text = "show panel 1";
    }

    protected override void OnClose()
    {
        title.text = "close panel 1";
    }

    protected override void OnDestryPanel()
    {
        title.text = "destroy panel 1";
    }

    public void OpenPanel2WithParameter(int para = 0)
    {
        example.OpenPanel2WithParameter(para);
    }

    public void OpenPanel3WithCloseEndListener()
    {
        example.OpenPanel3WithCloseEndListener(OnPanel3Closed);
    }

    void OnPanel3Closed()
    {
        Close();
    }
}
