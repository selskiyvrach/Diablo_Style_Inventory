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
            // try to extract required item from underlying space
            if(item != null && _space.TryExtractItem(item.TopLeftCornerPosInt, out IVector2IntItem extracted))
                // cast the result to return type
                return (InventoryItem)extracted;
            return null;
        }

        public override Projection GetProjection(InventoryItem item, Vector2 screenPos)
        {
            // return empty projection if carried item/cursor doesn't overlap screen rect of the slot
            if(!ActiveOnScreen || !HasProjection(item, screenPos))
                return lastProjection = Projection.EmptyProjection;

            if(item != null)
                return GetCarriedItemProjection(item);
            else 
                return GetEmptyCursorProjection(screenPos);
        }

        private Projection GetEmptyCursorProjection(Vector2 screenPos)
        {
            // declare placeholders for the future projection fields
            Rect rect = new Rect(); bool canPlace = true; InventoryItem replacement = null; InventoryItem[] refugees = null;

            var normPos = screenRect.ScreenToNormalized(screenPos);
            var cellCoord = NormPosToCellPos(normPos);

            if (_space.PeekItem(cellCoord, out IVector2IntItem peeked))
            {
                rect = GetNormRectForItem(peeked.TopLeftCornerPosInt, peeked.SizeInt);
                replacement = (InventoryItem)peeked;
            }
            else
                rect = new Rect(CellPosToNormPos(NormPosToCellPos(normPos)), new Vector2(1, 1) / (Vector2)sizeData.SizeInt);

            return GetSameOrNewProjection(rect, canPlace, replacement, refugees);
        }

        private Projection GetCarriedItemProjection(InventoryItem item)
        {
            // declare placeholders for the future projection fields
            Rect rect = new Rect(); bool canPlace = true; InventoryItem replacement = null; InventoryItem[] refugees = null;

            var cornerPosScreen = item.GetCornerCenterInScreen(0, screenRect.Rect.size.x / sizeData.SizeInt.x);
            var cornerPosCell = NormPosToCellPos(screenRect.ScreenToNormalized(cornerPosScreen));

            var overlaps = _space.GetOverlaps(cornerPosCell, item.SizeInt).Select(n => (InventoryItem)n);
            var overlapsCount = overlaps.Count();

            // if overlapped only one item - it can be replaced; it's rect will be highlighted
            if(overlapsCount == 1)
            {
                replacement = overlaps.First();
                // rect will be potential raplacement's rect
                rect = GetNormRectForItem(replacement.TopLeftCornerPosInt, replacement.SizeInt);
            }
            else 
            {
                // if overlaps are empty - no refugees, otherwise refugees are overlaps
                refugees = overlapsCount > 1 ? overlaps.ToArray() : null;
                // rect will be of carried item
                rect = GetNormRectForItem(cornerPosCell, item.SizeInt);
            }
            canPlace = fitRule.CanFit(item.FitRule) && overlapsCount < 2;

            return GetSameOrNewProjection(rect, canPlace, replacement, refugees);
        }

        private Projection GetSameOrNewProjection(Rect normRect, bool canPlace, InventoryItem replacement, InventoryItem[] refugees)
        {
            // getting screen rect from normalized used internally
            var scrRect = screenRect.NormalizedRectToScreenRect(normRect);
            return
                lastProjection.FieldsEqual(scrRect, canPlace, replacement, refugees) ? 
                    // return "same" if fields haven't changed
                    Projection.SameProjection : 
                    // return new one otherwise and set it as "lastProjection"
                    lastProjection = new Projection(scrRect, canPlace, replacement, refugees);
        }

        ///<summary>
        ///whether diagonally oppposite corners of a dragged item (if present) or cursor pos are/is inside screen rectangle of a slot</summary>
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
            // if no projection - get new one
            lastProjection ??= GetProjection(item, item.ScreenPos);
            if(lastProjection.CanPlace)
            {
                // placeholder for return value
                InventoryItem replaced = null;
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
            Debug.LogError("Tried to put item when projection had CanPlace with false value");
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
            => item.ScreenPos = screenRect.NormalizedRectPointToScreen(GetNormRectForItem(item.TopLeftCornerPosInt, item.SizeInt).center);
        
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