
public static class InventoryItemFactory 
{
    public static InventoryItem GetInventoryItem(InventoryItemData data)
        => new InventoryItem(data);
}
