using UnityEngine.UI;
using NEO.MODULE;

public class UIExamplePanel5 : UIBase
{
    public Text title;
    protected override void OnInitialize()
    {
        title.text = "panel 5";
    }

    protected override void OnShow()
    {
        title.text = "show panel 5";
    }

    protected override void OnClose()
    {
        title.text = "close panel 5";
    }

    protected override void OnDestryPanel()
    {
        title.text = "destroy panel 5";
    }
}