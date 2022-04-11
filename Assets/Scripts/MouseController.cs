using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CursorType
{
    Default,
    Attack
}

public class MouseController : MonoBehaviour
{
    [SerializeField]
    private Texture2D defaultCursor;
    [SerializeField]
    private Texture2D attackCursor;

    private CursorType cursorType;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    private void Start()
    {
        ChangeDefaultCursor();
    }

    private void Update()
    {
        MouseOver();
    }

    // Start is called before the first frame update
    private void ChangeDefaultCursor()
    {
        cursorType = CursorType.Default;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private void ChangeAttackCursor()
    {
        cursorType = CursorType.Attack;
        Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
    }

    public void MouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            if (raycastHit.collider.CompareTag("Enemy"))
            {
                // Enemy tag에 마우스 올리면 Attack 커서로 변경
                if (cursorType != CursorType.Attack)
                {
                    ChangeAttackCursor();
                }
            }
            else
            {
                ChangeDefaultCursor();
            }
        }
        else
        {
            ChangeDefaultCursor();
        }
    }
}
