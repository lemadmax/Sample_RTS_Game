using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
/*********************************************************
 *
 * ------------------- PlayerManager --------------------
 * One of the main game logic class.
 * This class holds and manage the properties of a player.
 * Every Player game object should have this class attached.
 * Handle keyboard and mouse input.
 *
 *********************************************************/
public class PlayerManager : MonoBehaviour
{
    public int netId = -1;
    public bool AIActive = false;
    public GameObject hero;

    public Hero hero_t;
    public int side;
    public int playerId;
    public int gold;
    public int autoIncomeRate = 10;

    public GameObject gameManagerObj;
    public GlobalSelection globalSelection;
    public CrowdArrange crowdArrange;
    public GameObject inGameUI;

    public GameObject baseBuilding;
    public GameManager gameManager;
    public GameSetting gameSetting;
    public GameObject cameraRig;
    private NetworkManager networkManager;

    public List<GameObject> units = new List<GameObject>();
    public List<GameObject> buildings = new List<GameObject>();

    private List<GameObject> deathList = new List<GameObject>();
    private List<GameObject> destroyedList = new List<GameObject>();

    public List<GameObject> enemyOnTerritory = new List<GameObject>();

    private float incomeTimer = 1.0f;

    private SimplePlayerAI playerAI;
    [SerializeField] private GameObject AIEnemy;

    void Start()
    {
        gameManagerObj = GameObject.Find("GameManager");
        globalSelection = GameObject.Find("Event System").GetComponent<GlobalSelection>();
        crowdArrange = GameObject.Find("Event System").GetComponent<CrowdArrange>();
        inGameUI = GameObject.Find("GoldText");

        gameManager = gameManagerObj.GetComponent<GameManager>();
        gameSetting = gameManagerObj.GetComponent<GameSetting>();
        cameraRig = GameObject.Find("Camera Rig");
        if(side == 2) playerAI = new SimplePlayerAI(this, baseBuilding, AIEnemy);
        GameObject network = GameObject.Find("Network");
        if (network != null)
        {
            networkManager = network.GetComponent<NetworkManager>();
        }
    }
    public void LogicUpdate(float LogicLen)
    {
        if (side == 0 || gameManager == null) return;
        if(side != 0) AutoIncoming();
        if (side != gameManager.side && side != 0 && AIActive)
        {
            AutoIncoming();
            if (playerAI == null) playerAI = new SimplePlayerAI(this, baseBuilding, AIEnemy);
            playerAI.Act(LogicLen);
        }
        if (deathList.Count > 0)
        {
            List<GameObject> killList = new List<GameObject>();
            foreach (GameObject ob in deathList)
            {
                if (ob.GetComponent<UnitManager>().DeathCountDown())
                {
                    killList.Add(ob);
                }
            }
            foreach (GameObject ob in killList)
            {
                deathList.Remove(ob);
                Destroy(ob);
            }
        }
        if (destroyedList.Count > 0)
        {
            List<GameObject> killList = new List<GameObject>();
            foreach (GameObject ob in destroyedList)
            {
                if (ob.GetComponent<BuildingManager>().DeathCountDown())
                {
                    killList.Add(ob);
                }
            }
            foreach (GameObject ob in killList)
            {
                destroyedList.Remove(ob);
                Destroy(ob);
            }
        }
    }
    public void RenderUpdate(float renderLerpValue)
    {

    }
    void Update()
    {
        if (playerId != gameManager.id || side == 0) return;
        SetInGameUI();
        if (Input.GetKey(KeyCode.Space))
        {
            cameraRig.GetComponent<CameraController>().LookAtHero(hero);
            globalSelection.selectTable.DeselectAll();
            foreach (GameObject unit in units)
            {
                if (unit.GetComponent<UnitManager>().followHero)
                {
                    unit.GetComponent<LineRenderer>().enabled = true;
                }
            }
        }
        if (globalSelection.selectTable.IsEmpty() && hero.tag != "Dead")
        {
            hero.GetComponent<UnitManager>().Select();
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                Physics.Raycast(ray, out hitInfo, 50000.0f);
                if (hitInfo.transform.gameObject.tag == "UnitBody" && hitInfo.transform.GetComponentInParent<UnitManager>().pm.side != side)
                {

                    networkManager.PushCommand(CommandType.SetTarget, hitInfo.transform.parent.gameObject.transform.position, 
                        hitInfo.transform.GetComponentInParent<UnitManager>().netId, 1, new int[]{ hero.GetComponent<UnitManager>().netId});

                }
                else if (hitInfo.transform.gameObject.tag == "BuildingBody" && hitInfo.transform.GetComponentInParent<BuildingManager>().side != gameManager.side)
                {

                    networkManager.PushCommand(CommandType.SetTarget, hitInfo.transform.parent.gameObject.transform.position,
                        hitInfo.transform.GetComponentInParent<BuildingManager>().netId, 1, new int[] { hero.GetComponent<UnitManager>().netId });

                }
                else if(!EventSystem.current.IsPointerOverGameObject())
                {
                    GameObject[] followingUnits = crowdArrange.GetFollowingUnits(units);
                    int[] unitIds = new int[followingUnits.Length + 1];
                    for (int i = 0; i < followingUnits.Length; i++)
                    {
                        unitIds[i] = followingUnits[i].GetComponent<UnitManager>().netId;
                    }
                    unitIds[followingUnits.Length] = hero.GetComponent<UnitManager>().netId;
                    networkManager.PushCommand(CommandType.SetTarget, hitInfo.point,
                        -1, followingUnits.Length + 1, unitIds);
                }
            }
            HandleSkills();
        }
        else if(globalSelection.selectTable.sType == SelectType.Building)
        {
            hero.GetComponent<UnitManager>().UnSelect();
            foreach (GameObject unit in units)
            {
                if (unit.GetComponent<UnitManager>().followHero && !unit.GetComponent<UnitManager>().selected) unit.GetComponent<LineRenderer>().enabled = false;
            }
            if (globalSelection.selectTable.building.GetComponent<BuildingManager>().type == Building.Barracks)
            {
                if(Input.GetKeyDown(KeyCode.A))
                {
                    networkManager.PushCommand(CommandType.Spawn_0, globalSelection.selectTable.building.GetComponent<BuildingManager>().RallyPoint,
                        netId, 1, new int[] { globalSelection.selectTable.building.GetComponent<BuildingManager>().netId });
                }
                if(Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    Physics.Raycast(ray, out hitInfo, 50000.0f);
                    if(hitInfo.transform.gameObject.name == "Terrain")
                    {
                        globalSelection.selectTable.building.GetComponent<BuildingManager>().RallyPoint = hitInfo.point;
                    }
                }
            }
        }
        else if(globalSelection.selectTable.sType == SelectType.Unit)
        {
            hero.GetComponent<UnitManager>().UnSelect();
            foreach (GameObject unit in units)
            {
                if (unit.GetComponent<UnitManager>().followHero && !unit.GetComponent<UnitManager>().selected) unit.GetComponent<LineRenderer>().enabled = false;
            }
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                Physics.Raycast(ray, out hitInfo, 50000.0f);
                GameObject[] selectedUnits = globalSelection.selectTable.GetUnitArray();
                if(hitInfo.transform.gameObject.tag == "UnitBody" && hitInfo.transform.GetComponentInParent<UnitManager>().pm.side != gameManager.side)
                {
                    int[] unitIds = new int[selectedUnits.Length];
                    for(int i = 0; i < selectedUnits.Length; i++)
                    {
                        unitIds[i] = selectedUnits[i].GetComponent<UnitManager>().netId;
                    }
                    networkManager.PushCommand(CommandType.SetTarget, hitInfo.transform.parent.gameObject.transform.position,
                        hitInfo.transform.GetComponentInParent<UnitManager>().netId, selectedUnits.Length, unitIds);
                }
                else if(hitInfo.transform.gameObject.tag == "BuildingBody" && hitInfo.transform.GetComponentInParent<BuildingManager>().side != gameManager.side)
                {
                    int[] unitIds = new int[selectedUnits.Length];
                    for (int i = 0; i < selectedUnits.Length; i++)
                    {
                        unitIds[i] = selectedUnits[i].GetComponent<UnitManager>().netId;
                    }
                    networkManager.PushCommand(CommandType.SetTarget, hitInfo.transform.parent.gameObject.transform.position,
                        hitInfo.transform.GetComponentInParent<BuildingManager>().netId, selectedUnits.Length, unitIds);
                }
                else if (!EventSystem.current.IsPointerOverGameObject())
                {
                    int[] unitIds = new int[selectedUnits.Length];
                    for (int i = 0; i < selectedUnits.Length; i++)
                    {
                        unitIds[i] = selectedUnits[i].GetComponent<UnitManager>().netId;
                    }
                    networkManager.PushCommand(CommandType.SetTarget, hitInfo.point,
                        -1, selectedUnits.Length, unitIds);
                }
                //globalSelection.selectTable.DeselectAll();
            }
            if(Input.GetKeyDown(KeyCode.F))
            {
                GameObject[] selectedUnits = globalSelection.selectTable.GetUnitArray();
                globalSelection.selectTable.DeselectAll();

                int[] unitIds = new int[selectedUnits.Length + 1];
                for (int i = 0; i < selectedUnits.Length; i++)
                {
                    unitIds[i] = selectedUnits[i].GetComponent<UnitManager>().netId;
                }
                unitIds[selectedUnits.Length] = hero.GetComponent<UnitManager>().netId;
                cameraRig.GetComponent<CameraController>().LookAtHero(hero);
                networkManager.PushCommand(CommandType.SetTarget, hero.transform.position,
                    -1, selectedUnits.Length + 1, unitIds);
            }
        }
    }
    public GameObject UnitSpawn(Unit unit_t, BuildingManager bm)
    {
        if (!bm.IsAlive) return null;
        GameObject unit = null;
        int price = gameSetting.UnitPrice[(int)hero_t, (int)unit_t];
        if (gold < price)
        {
            ShowPopupMessage("not enough gold");
        }
        else
        {
            unit = bm.SpawnUnit(this, unit_t);
            units.Add(unit);
            gold -= price;
        }
        return unit;
    }
    public void UnitDeath(GameObject ob)
    {
        units.Remove(ob);
        deathList.Add(ob);
    }
    public void BuildingDestroyed(GameObject ob)
    {
        buildings.Remove(ob);
        destroyedList.Add(ob);
    }
    public void AddGold(int amount)
    {
        gold += amount;
    }
    void HandleSkills()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Command cmd = hero.GetComponent<UnitManager>().GetSkillCmd(0);
            networkManager.PushCommand(CommandType.Skill_0, cmd.vec3,
                cmd.id, cmd.cnt, cmd.ids);
        }
    }
    void AutoIncoming()
    {
        incomeTimer -= Time.deltaTime;
        if(incomeTimer <= 0.0f)
        {
            gold += autoIncomeRate;
            incomeTimer = 1.0f;
        }
    }
    void SetInGameUI()
    {
        TextMeshProUGUI textMesh = inGameUI.GetComponent<TextMeshProUGUI>();
        string text = "Gold: " + gold;
        textMesh.text = text;
    }
    void ShowPopupMessage(string message)
    {
        Debug.Log(message);
    }
}
