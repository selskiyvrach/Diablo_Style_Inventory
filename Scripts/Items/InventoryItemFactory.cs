
public static class InventoryItemFactory 
{
    public static InventoryItem GetInventoryItem(InventoryItemData data, float unitSize)
        => new InventoryItem(data);
}
