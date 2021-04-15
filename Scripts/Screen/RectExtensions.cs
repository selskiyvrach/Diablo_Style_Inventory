using UnityEngine;

namespace MNS.Utils
{

    public static class RectExtensions 
    {
        
        public static Vector2 ScreenPointToNormalized(this Rect Rect, Vector2 screenPos)
            => (screenPos - Rect.position) / Rect.size;

        public static Vector2 NormalizedRectPointToScreen(this Rect Rect, Vector2 normalizedRectPoint)
        {
            float xPos = Rect.position.x + Rect.size.x * normalizedRectPoint.x;
            float yPos = Rect.position.y + Rect.size.y - Rect.size.y * normalizedRectPoint.y;
            Vector2 pos = new Vector2(xPos, yPos);
            return pos;
        }

        public static Rect NormalizedRectToScreenRect(this Rect Rect, Rect normalized)
        {
            Vector2 size = Rect.size * normalized.size;
            float xPos = Rect.position.x + Rect.size.x * normalized.position.x;
            float yPos = Rect.position.y + Rect.size.y - size.y - Rect.size.y * normalized.position.y;
            Vector2 pos = new Vector2(xPos, yPos);
            return new Rect(pos, size);
        }

        public static Rect ScreenRectFromRectTransform(this RectTransform rectTransform)
        {
            Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[0], size);
        }

    }
    
}
