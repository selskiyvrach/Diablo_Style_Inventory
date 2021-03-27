using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public class MultiItemContainer : ContainerBase
    {
        private Vector2IntSpacing _space;

        private void Awake() {
            _space = new Vector2IntSpacing(sizeData.SizeInt);
        }

        public override InventoryItem ExtractItem(InventoryItem item)
        {
            if(item != null && _space.TryExtractItem(item.TopLeftCornerPosInt, out IVector2IntItem extracted))
                return (InventoryItem)extracted;
            return null;
        }

        public override Projection GetProjection(InventoryItem item, Vector2 screenPos)
        {
            if(!HasProjection(item, screenPos))
                return lastProjection = Projection.EmptyProjection;

            Rect rect = new Rect();
            bool canPlace = true;
            InventoryItem replacement = null;
            IReadOnlyCollection<InventoryItem> refugees = null;

            if(item != null)
            {
                var cornerPosScreen = item.GetCornerCenterInScreen(0, screenRect.Rect.size.x / sizeData.SizeInt.x);
                var cornerPosCell = NormPosToCellPos(screenRect.ScreenToNormalized(cornerPosScreen));

                rect = GetNormRectForItem(cornerPosCell, item.SizeInt);

                var overlaps = _space.GetOverlaps(cornerPosCell, item.SizeInt).Select(n => (InventoryItem)n);
                var overlapsCount = overlaps.Count();

                if(overlapsCount < 2)
                    replacement = overlaps.FirstOrDefault();
                else 
                    refugees = overlaps.ToArray();

                canPlace = fitRule.CanFit(item.FitRule) && overlapsCount < 2;
            }
            else
            {
                var normPos = screenRect.ScreenToNormalized(screenPos);
                var cellCoord = NormPosToCellPos(normPos);

                if (_space.PeekItem(cellCoord, out IVector2IntItem peeked))
                {
                    rect = GetNormRectForItem(peeked.TopLeftCornerPosInt, peeked.SizeInt);
                    replacement = (InventoryItem)peeked;
                }
                else
                    rect = new Rect(CellPosToNormPos(NormPosToCellPos(normPos)), new Vector2(1, 1) / (Vector2)sizeData.SizeInt);
            }
            var scrRect = screenRect.NormalizedRectToScreenRect(rect);

            return lastProjection = new Projection(scrRect, canPlace, replacement, refugees);
        }

        private bool HasProjection(InventoryItem item, Vector2 screenPos)
        {
            if(item != null)
            {
                var corn1 = item.GetCornerCenterInScreen(0, screenRect.Rect.size.x / sizeData.SizeInt.x);
                var corn2 = item.GetCornerCenterInScreen(1, screenRect.Rect.size.x / sizeData.SizeInt.x);
                return screenRect.ContainsPoint(corn1) && screenRect.ContainsPoint(corn2);
            }
            return screenRect.ContainsPoint(screenPos);
        }

        public override InventoryItem PlaceItem(InventoryItem item)
        {
            InventoryItem replaced = null;

            lastProjection ??= GetProjection(item, item.ScreenPos);
            if(lastProjection.CanPlace)
            {
                if(lastProjection.Replacement != null)
                {
                    _space.TryExtractItem(lastProjection.Replacement.TopLeftCornerPosInt, out IVector2IntItem extracted);
                    replaced = (InventoryItem)extracted;
                }
                var topLeftCornerCell = NormPosToCellPos(screenRect.ScreenToNormalized(item.GetCornerCenterInScreen(0, screenRect.Rect.size.x / sizeData.SizeInt.x)));
                _space.PlaceItemAtPos(item, topLeftCornerCell);
                AnchorItemImage(item);
                return replaced;
            }
            Debug.LogError("Tried to put item when projection has CanPlace with false value");
            return null;
        }

        public override bool TryPlaceItemAuto(InventoryItem item)
        {
            if(_space.TryPlaceItemAuto(item))
            {
                var itemRect = GetNormRectForItem(item.TopLeftCornerPosInt, item.SizeInt);
                item.ScreenPos = screenRect.NormalizedRectPointToScreen(itemRect.center);
                return true; 
            }
            Debug.LogError("Could't place item auto to Vector2IntSpace");
            return false;
        }

        private void AnchorItemImage(InventoryItem item)
            => item.ScreenPos = lastProjection.ScreenRect.center;
        
        private Rect GetNormRectForItem(Vector2Int posInt, Vector2Int itemSizeInt)
        {
            var sizeNorm = (Vector2)itemSizeInt / (Vector2)sizeData.SizeInt;
            var cornerPosNorm = (Vector2)posInt / (Vector2)sizeData.SizeInt;
            return new Rect(cornerPosNorm, sizeNorm);
        }

        private Vector2 CellPosToNormPos(Vector2Int cellPos)
            => (Vector2)cellPos / (Vector2)sizeData.SizeInt;

        private Vector2Int NormPosToCellPos(Vector2 normPos)
        {
            float x = normPos.x / ( 1f / sizeData.SizeInt.x);
            float y = sizeData.SizeInt.y - normPos.y / ( 1f / sizeData.SizeInt.y);
            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }
    }
}