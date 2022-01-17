class CheckMaterial
{
    private bool _exist;
    private int _materialItemId;
    private int _requiredNumber;
    
    public int Id { get { return _materialItemId; }}
    public int Count { get { return _requiredNumber; }}
    public bool Exist { get { return _exist; }}
    
    public CheckMaterial(int materialItemId, int requiredNumber, bool exist)
    {
        _exist = exist;
        _materialItemId = materialItemId;
        _requiredNumber = requiredNumber;
    }
}