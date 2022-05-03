using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITab : MonoBehaviour
{
    // Tab을 직접 할당
    [SerializeField]
    private UITabButton[] tabs;

    [SerializeField]
    private GameObject[] tabDetails;

    private int selectedIndex = 0;

    public void Open(int index)
    {
        // Tab click event 함수에 Open(index)를 전달해야 함
        tabs[selectedIndex].UnSelect();
        tabDetails[selectedIndex].SetActive(false);

        tabs[index].Select();
        tabDetails[index].SetActive(true);
        selectedIndex = index;
    }
}
