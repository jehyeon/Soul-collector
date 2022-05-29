using System.Collections.Generic;

public class ItemManager
{
    private List<Dictionary<string, object>> data;
    
    public ItemManager()
    {
        data = CSVReader.Read("Item");
    }

    public Item Get(int itemId)
    {
        return new Item(itemId, data[itemId]);
    }
}