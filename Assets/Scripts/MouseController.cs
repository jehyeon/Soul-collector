using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CursorType
{
    Default,
    Drop,
    Attack
}

public class MouseController : MonoBehaviour
{
    [SerializeField]
    private Texture2D defaultCursor;
    [SerializeField]
    private Texture2D attackCursor;
    [SerializeField]
    private Texture2D dropCursor;

    private CursorType cursorType;

    public Vector2 offset;

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
        Cursor.SetCursor(defaultCursor, offset, CursorMode.Auto);
    }

    private void ChangeAttackCursor()
    {
        cursorType = CursorType.Attack;
        Cursor.SetCursor(attackCursor, offset, CursorMode.Auto);
    }

    private void ChangeDropCursor()
    {
        cursorType = CursorType.Drop;
        Cursor.SetCursor(dropCursor, offset, CursorMode.Auto);
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
            else if (raycastHit.collider.CompareTag("Item"))
            {
                // Enemy tag에 마우스 올리면 Attack 커서로 변경
                if (cursorType != CursorType.Drop)
                {
                    ChangeDropCursor();
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
