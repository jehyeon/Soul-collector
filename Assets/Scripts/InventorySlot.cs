public class InventorySlot : Slot
{
    private int slotIndex;

    private void Start()
    {
        slotIndex = -1;
        if (gameObject.name != "Item Image")
        {
            slotIndex = int.Parse(gameObject.name.Split('(')[1].Split(')')[0]);
        }
    }
}