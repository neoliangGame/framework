namespace NEO.MODULE
{
    public enum UIPanelType
    {
        /// <summary>
        /// 未定义窗口，一些系统支持之外的窗口
        /// </summary>
        None,

        /// <summary>
        /// 正常窗口，无返回，自己打开和关闭
        /// </summary>
        Normal,

        /// <summary>
        /// 有返回的窗口，即上层窗口关闭后，会显示这一层
        /// </summary>
        Back,
    }

    public enum UIPanelLayer
    {
        /// <summary>
        /// 无定义层级，不挂节点
        /// </summary>
        None,

        /// <summary>
        /// 底部
        /// </summary>
        Bottom,

        /// <summary>
        /// 中层
        /// </summary>
        Middle,

        /// <summary>
        /// 上层
        /// </summary>
        Top,
    }

    //为了独立而设置的面板ID，如果实际项目有统一的资源ID，则用资源ID
    public enum UIPanelID
    {
        None,

        //根据实际修改......
        Test1,
        Test2,
        Test3,
        Test4,
        Test5,
    }

    public class UIConstant
    {
        public const string ANIMATION_SHOW = "show";
        public const string ANIMATION_CLOSE = "close";
    }
}

