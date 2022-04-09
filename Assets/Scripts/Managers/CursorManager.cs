using UnityEngine;
/*********************************************************
 *
 * -------------------- CursorManager --------------------
 * This class manages the cursor selection.
 *
 *********************************************************/
public class CursorManager : MonoBehaviour
{
    public Texture2D cursorArrow;
    public Texture2D cursorAttack;
    public Texture2D cursorSelect;

    public Texture2D cursorArrow4k;
    public Texture2D cursorAttack4k;
    public Texture2D cursorSelect4k;

    GameManager gameManager;
    GlobalSelection globalSelection;
    public bool use4k;

    void Start()
    {
        globalSelection = GameObject.Find("Event System").GetComponent<GlobalSelection>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(use4k)
        {
            cursorArrow = cursorArrow4k;
            cursorAttack = cursorAttack4k;
            cursorSelect = cursorSelect4k;
        }
    }


    void Update()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 50000.0f))
        {
            if (hitInfo.transform.gameObject.tag == "UnitBody" || hitInfo.transform.gameObject.tag == "BuildingBody")
            {
                //Debug.Log(hitInfo.transform.gameObject);
                UnitManager um = hitInfo.transform.GetComponentInParent<UnitManager>();
                BuildingManager bm = hitInfo.transform.GetComponentInParent<BuildingManager>();
                int side = 1;
                if (um == null) side = bm.side;
                else side = um.pm.side;
                if (side == gameManager.side)
                {
                    Cursor.SetCursor(cursorSelect, Vector2.zero, CursorMode.ForceSoftware);
                }
                else
                {
                    Cursor.SetCursor(cursorAttack, Vector2.zero, CursorMode.ForceSoftware);
                }
            }
            else
            {
                Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
            }
        }
    }
}
