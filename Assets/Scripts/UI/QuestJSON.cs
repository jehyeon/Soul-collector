using System.Collections.Generic;

public class QuestProgress
{
    public int Id;
    public int Progress;

    public QuestProgress()
    {
        Id = -1;
        Progress = -1;
    }

    public QuestProgress(int id, int progress)
    {
        Id = id;
        Progress = progress;
    }
}

public class QuestJSON
{
    public List<QuestProgress> Progress;
    public List<int> Done;
    public List<int> Ready;

    public QuestJSON()
    {
        
        Progress = new List<QuestProgress>()
        {
            new QuestProgress(0, 0)
        };

        Ready = new List<int>
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10
        };

        Done = new List<int>();
    }
}
