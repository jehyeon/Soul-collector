using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillDes
{
    public string Name;
    public string Description;
    public Color Color;

    public SkillDes(string name, string description, int colorId)
    {
        Name = name;
        Description = description.Replace("nn", "\n");

        Color fontColor;
        
        if (colorId == 1)
        {
            ColorUtility.TryParseHtmlString("#28B71FFF", out fontColor);
        }
        else if (colorId == 2)
        {
            ColorUtility.TryParseHtmlString("#3275F8FF", out fontColor);
        }
        
        else if (colorId == 3)
        {
            ColorUtility.TryParseHtmlString("#B71B1BFF", out fontColor);
        }
        else
        {
            fontColor = Color.white;
        }
        
        Color = fontColor;
    }
}

public class SkillManager : MonoBehaviour
{
    private List<Dictionary<string, object>> data;
    
    private static SkillManager instance = null;
    public static SkillManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            data = CSVReader.Read("Skill");
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public SkillDes Get(int skillId)
    {
        return new SkillDes(data[skillId]["name"].ToString(), data[skillId]["description"].ToString(), (int)data[skillId]["rank"]);
    }
}