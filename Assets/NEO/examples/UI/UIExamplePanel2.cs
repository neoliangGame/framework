using UnityEngine.UI;
using NEO.MODULE;

public class UIExamplePanel2 : UIBase
{
    public Text title;
    public int para = 0;
    protected override void OnInitialize()
    {
        title.text = "panel 2 : " + para.ToString();
    }

    protected override void OnShow()
    {
        title.text = "show panel 2 : " + para.ToString();
    }

    protected override void OnClose()
    {
        title.text = "close panel 2 : " + para.ToString();
    }

    protected override void OnDestryPanel()
    {
        title.text = "destroy panel 2 : " + para.ToString();
    }
}