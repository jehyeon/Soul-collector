public class CraftMaterial
{
    // private bool _exist;
    private int id;
    private int requiredNumber;
    
    public int Id { get { return id; }}
    public int RequiredNumber { get { return requiredNumber; }}
    // public bool Exist { get { return _exist; }}
    
    public CraftMaterial(int gold)
    {
        // gold
        id = 1627;
        requiredNumber = gold;
    }

    public CraftMaterial(string[] materialInfo)
    {
        id = int.Parse(materialInfo[0]);
        requiredNumber = int.Parse(materialInfo[1]);
    }
}