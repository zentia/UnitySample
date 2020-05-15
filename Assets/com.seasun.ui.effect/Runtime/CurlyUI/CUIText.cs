namespace UnityEngine.UI.Effect
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("UI/Effects/顶点Text动画")]
    public class CUIText : CUIGraphic
    {
        public override void ReportSet()
        {
            if (uiGraphic == null)
                uiGraphic = GetComponent<Text>();

            base.ReportSet();
        }
    }
}