using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{

    // 드랍 테이블로 수정
    [SerializeField]
    private GameObject dropWeapon;
    [SerializeField]
    public GameObject dropBox;

    private int _enemyId;
    private int _minGold;
    private int _maxGold;
    private string _dropItems;

    // Enemy가 drop하는 gold와 item의 id
    public int _dropGold;
    public int _dropItemId;

    private void Awake()
    {
        // 드랍 테이블 가져오기
        // _enemyId = gameObject.GetComponent<Enemy>()._id;
        GetDropTable();
        _dropItemId = -1;
    }
    private void Start()
    {
        SetDrop();
    }
    public void DropItem()
    {
        if (_dropItemId == -1)
        {
            // 드랍 아이템이 없음
            return;
        }
        if (_dropItemId >= 1600)
        {
            // resources는 item id가 1600 이상
            dropBox.GetComponent<DroppedItem>().SetDrop(_dropItemId);
            Instantiate(dropBox, this.transform.position, this.transform.rotation);
        }
        else
        {
            // 장비 아이템 (현재 weapon prefab만 사용)
            dropBox.GetComponent<DroppedItem>().SetDrop(_dropItemId);
            Instantiate(dropWeapon, this.transform.position, this.transform.rotation);
        }
    }
    public void DropGold()
    {
        // gameObject.GetComponent<Enemy>().player.GetComponent<Player>().GetGold(_dropGold);
    }

    private void GetDropTable()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("Drop");
        _minGold = (int)data[_enemyId]["minGold"];
        _maxGold = (int)data[_enemyId]["maxGold"];
        _dropItems = data[_enemyId]["drop"].ToString();
    }

    private void SetDrop()
    {
        // Set gold
        _dropGold = Random.Range(_minGold, _maxGold + 1);

        // Set Item
        float percent = 0f;
        float rand = Random.value;

        // _dropItems는 확률 오름차순 정렬
        string[] itemInfos = _dropItems.Split('/');
        for (int i = 0; i < itemInfos.Length; i++)
        {
            string[] itemInfo = itemInfos[i].Split('|');

            percent += (float)System.Convert.ToDouble(itemInfo[1]);

            if (rand < percent)
            {
                _dropItemId = int.Parse(itemInfo[0]);
                return;
            }
        }
    }
}
