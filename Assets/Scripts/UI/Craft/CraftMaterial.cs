public class CraftMaterial
{
    private bool exist;
    private int id;
    private int requiredNumber;
    private int count;

    public int Id { get { return id; }}
    public int RequiredNumber { get { return requiredNumber; }}
    public int Count { get { return count; } }
    public bool Exist { get { return exist; }}
    
    public CraftMaterial(int gold)
    {
        // gold
        id = 0;
        requiredNumber = gold;
        count = 0;
    }

    public CraftMaterial(string[] materialInfo)
    {
        id = int.Parse(materialInfo[0]);
        requiredNumber = int.Parse(materialInfo[1]);
        count = 0;
    }

    public void SetCount(int myCount)
    {
        count = myCount;
        exist = count >= requiredNumber;
    }

    public override string ToString()
    {
        return string.Format("({3}) {0}: {1}/{2}", id, count, requiredNumber, exist);
    }
}