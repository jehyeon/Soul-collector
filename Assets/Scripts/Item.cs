using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Use,
    Material,
}

public class Item
{
    private Dictionary<string, object> rawData;     // 아이템 매니저로부터 가져온 데이터

    // 공용
    public int _id;             // 아이템 ID
    private string _itemName;   // 아이름 이름
    private int _imageId;       // 아이템 이미지 ID
    private ItemType _itemType;      
    private int _rank;          
    private string _des;        // description

    // for view in slot
    private Color _backgroundColor;
    private Color _fontColor;
    private Sprite _itemImage;
    private Sprite _itemFrame;

    public int Id { get { return _id; }}
    public string ItemName { get { return _itemName; }}
    public int ImageId { get { return _imageId; }}
    public ItemType ItemType { get { return _itemType; }}
    public int Rank { get { return _rank; }}
    public string Des { get { return _des; }}

    public Color BackgroundColor { get { return _backgroundColor; }}
    public Color FontColor { get { return _fontColor; }}
    public Sprite ItemImage { get { return _itemImage; }}
    public Sprite ItemFrame { get { return _itemFrame; }}

    public Item(int itemId, Dictionary<string, object> data)
    {
        rawData = data;
        
        _id = itemId;

        // 아이템 테이블로부터 가져온 정보
        _imageId = (int)data["imageId"];
        if ((int)data["itemType"] == 0)
        {
            _itemType = ItemType.Weapon;
        }
        else if ((int)data["itemType"] > 0 && (int)data["itemType"] < 12)
        {
            _itemType = ItemType.Armor;
        }
        else if ((int)data["itemType"] == 12)
        {
            _itemType = ItemType.Material;
        }
        else
        {
            _itemType = ItemType.Use;
        }
        _rank = (int)data["rank"];
        _itemName = data["itemName"].ToString();
        // !!! move to weaponItem, armorItem
        // _defaultDamage = (int)data["defaultDamage"];
        // _maxDamage = (int)data["maxDamage"];
        // _minDamage = (int)data["minDamage"];
        // _attackSpeed = (int)data["attackSpeed"];
        // _damageReduction = (int)data["damageReduction"];
        // _evasionPercent = (int)data["evasionPercent"];
        // _maxHp = (int)data["maxHp"];
        // _hpRecovery = (int)data["hpRecovery"];
        // _moveSpeed = (int)data["moveSpeed"];
        // _criticalPercent = (int)data["criticalPercent"];
        _des = data["des"].ToString();

        // 아이템 이미지
        _itemImage = Resources.Load<Sprite>("Item Images/" + _imageId);

        // 배경색, 폰트색, 아이템 프레임
        Color backgroundColor;
        Color fontColor;    // 나중에 업데이트
        if (_rank == 2 || _rank == 7)
        {
            // green
            ColorUtility.TryParseHtmlString("#28B71FFF", out fontColor);
            ColorUtility.TryParseHtmlString("#142E22FF", out backgroundColor);
            _itemFrame = Resources.Load<Sprite>("sprites/frame_2");
        }
        else if (_rank == 3 || _rank == 8)
        {
            // blue
            ColorUtility.TryParseHtmlString("#021334FF", out backgroundColor);
            ColorUtility.TryParseHtmlString("#3275F8FF", out fontColor);
            _itemFrame = Resources.Load<Sprite>("sprites/frame_3");
        }
        else if (_rank == 4 || _rank == 9)
        {
            // red
            ColorUtility.TryParseHtmlString("#B71B1BFF", out fontColor);
            ColorUtility.TryParseHtmlString("#270A08FF", out backgroundColor);
            _itemFrame = Resources.Load<Sprite>("sprites/frame_4");
        }
        else if (_rank == 5)
        {
            // purple
            ColorUtility.TryParseHtmlString("#AA4FFFFF", out fontColor);
            ColorUtility.TryParseHtmlString("#210D34FF", out backgroundColor);
            _itemFrame = Resources.Load<Sprite>("sprites/frame_5");
        }
        else
        {
            // Include rank 1, 6
            // default
            ColorUtility.TryParseHtmlString("#C3C3C3FF", out fontColor);
            ColorUtility.TryParseHtmlString("#28241DFF", out backgroundColor);
            _itemFrame = Resources.Load<Sprite>("sprites/frame_1");
        }

        // Slot background
        _backgroundColor = backgroundColor;
        _fontColor = fontColor;
    }

    // public override string ToString()
    // {
    //     if (_itemType == 0)
    //     {
    //         // 무기 description
    //         string description = "";

    //         if (_attackSpeed != 0)
    //         {
    //             description += "공격속도 " + _attackSpeed.ToString() + "%\n";
    //         }
    //         if (_defaultDamage != 0)
    //         {
    //             description += "기본 데미지 " + _defaultDamage.ToString() + "\n";
    //         }
    //         if (_minDamage != 0 && _maxDamage != 0)
    //         {
    //             description += "무기 데미지 " + _minDamage.ToString() + " ~ " + _maxDamage.ToString() + "\n";
    //         }

    //         return description;
    //     }
    //     else if (_itemType >= 2 || _itemType <=9)
    //     {
    //         // 방어구 description
    //         string description = "";
    //         if (_damageReduction != 0)
    //         {
    //             description += "데미지 감소 " + _damageReduction.ToString() + "\n";
    //         }
    //         if (_maxHp != 0)
    //         {
    //             description += "최대 HP " + _maxHp.ToString() + "\n";
    //         }
    //         if (_hpRecovery != 0)
    //         {
    //             description += "HP 자동 회복 " + _hpRecovery.ToString() + "\n";
    //         }
    //         if (_evasionPercent != 0)
    //         {
    //             description += "회피율 " + _evasionPercent.ToString() + "%\n";
    //         }
    //         if (_criticalPercent != 0)
    //         {
    //             description += "치명타 확률 " + _criticalPercent.ToString() + "%\n";
    //         }
    //         if (_moveSpeed != 0)
    //         {
    //             description += "이동 속도 " + _moveSpeed.ToString() + "%\n";
    //         }

    //         return description;
    //     }
    //     else
    //     {
    //         return _des;
    //     }
    // }
}
