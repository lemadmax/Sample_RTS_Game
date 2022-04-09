using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MiniMapManager : MonoBehaviour, IPointerClickHandler
{
    public GameObject mainCamera;
    public GameObject gameManager;

    public UnityEvent leftClick;
    public UnityEvent rightClick;

    private CameraController cameraCtrl;
    private PlayerManager pm;
    private NetworkManager networkManager;
    Vector3 v;

    void Start()
    {
        v = new Vector3();
        GameManager manager = gameManager.GetComponent<GameManager>();
        pm = manager.player.GetComponent<PlayerManager>();
        cameraCtrl = mainCamera.GetComponent<CameraController>();
        GameObject network = GameObject.Find("Network");
        if (network != null)
        {
            networkManager = network.GetComponent<NetworkManager>();
        }

        leftClick.AddListener(new UnityAction(LeftClick));
        rightClick.AddListener(new UnityAction(RightClick));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            leftClick.Invoke();
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            rightClick.Invoke();
        }
    }

    private void LeftClick()
    {
        v = Input.mousePosition;
        Debug.Log(v);
        if (v.x <= 280 && v.y >= 10 && v.y <= 290)
        {
            v = MiniMapToMap(v);
            cameraCtrl.LookAtGivenPoint(v);
        }
    }
    // A small bug exists - conflict between this right click below and the right click in PM.
    private void RightClick()
    {
        v = Input.mousePosition;
        Debug.Log(v);
        if (v.x <= 280 && v.y >= 10 && v.y <= 290)
        {
            v = MiniMapToMap(v);
            if (pm.globalSelection.selectTable.IsEmpty() && pm.hero.tag != "Dead")
            {
                pm.hero.GetComponent<UnitManager>().Select();
                GameObject[] followingUnits = pm.crowdArrange.GetFollowingUnits(pm.units);
                int[] unitIds = new int[followingUnits.Length + 1];
                for (int i = 0; i < followingUnits.Length; i++)
                {
                    unitIds[i] = followingUnits[i].GetComponent<UnitManager>().netId;
                }
                unitIds[followingUnits.Length] = pm.hero.GetComponent<UnitManager>().netId;
                networkManager.PushCommand(CommandType.SetTarget, v, -1, followingUnits.Length + 1, unitIds);
            }
            else if (pm.globalSelection.selectTable.sType == SelectType.Unit)
            {
                pm.hero.GetComponent<UnitManager>().UnSelect();
                foreach (GameObject unit in pm.units)
                {
                    if (unit.GetComponent<UnitManager>().followHero && !unit.GetComponent<UnitManager>().selected)
                    {
                        unit.GetComponent<LineRenderer>().enabled = false;
                    }
                }
                GameObject[] selectedUnits = pm.globalSelection.selectTable.GetUnitArray();
                int[] unitIds = new int[selectedUnits.Length];
                for (int i = 0; i < selectedUnits.Length; i++)
                {
                    unitIds[i] = selectedUnits[i].GetComponent<UnitManager>().netId;
                }
                networkManager.PushCommand(CommandType.SetTarget, v, -1, selectedUnits.Length, unitIds);
            }
        }
    }

    public Vector3 MiniMapToMap(Vector3 miniMapPos)
    {
        Vector3 mapPos = new Vector3((-25.0f / 7.0f) * (miniMapPos.y - 150.0f), 0.0f, (25.0f / 7.0f) * (miniMapPos.x - 140.0f));
        return mapPos;
    }
}
