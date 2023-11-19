using UnityEngine;

public class CanvasFollowMouse : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (rectTransform == null) return;

        Vector2 canvasPos;
        RectTransform canvasRect = rectTransform.parent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out canvasPos);

        rectTransform.anchoredPosition = canvasPos;
    }
}
