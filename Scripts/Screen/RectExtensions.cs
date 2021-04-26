using D2Inventory;
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
            Vector2 unscaledSize = Rect.size * normalized.size;
            Vector2 scaledSize = unscaledSize / UnitSizeSetter.LocalScale;

            Vector2 size = Rect.size * normalized.size / UnitSizeSetter.LocalScale; 
            //           start pos         shift by normPos * size of container   shift by half of difference between unscaled and scaled pistures
            float xPos = Rect.position.x + Rect.size.x * normalized.position.x - (scaledSize.x - unscaledSize.x) / 2;
            float yPos = Rect.position.y + Rect.size.y - size.y - Rect.size.y * normalized.position.y + (scaledSize.y - unscaledSize.y) / 2;
            Vector2 pos = new Vector2(xPos, yPos);
            return new Rect(pos, size);
        }

        public static Rect ScreenRectFromRectTransform(this RectTransform rectTransform)
        {
            // Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
            // return new Rect(corners[0], size);
        }

    }
    
}
