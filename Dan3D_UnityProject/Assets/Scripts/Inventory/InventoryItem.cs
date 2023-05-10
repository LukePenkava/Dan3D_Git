[System.Serializable]
public class InventoryItem {

    public int slotIndex;
    public Items.ItemName itemName;

    public InventoryItem(int SlotIndex, Items.ItemName ItemName) {
        slotIndex = SlotIndex;
        itemName = ItemName;
    }
}