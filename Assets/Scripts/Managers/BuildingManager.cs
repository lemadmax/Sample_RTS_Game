using System.Collections.Generic;
using UnityEngine;
/*********************************************************
 *
 * ------------------ BuildingManager --------------------
 * One of the main game logic class.
 * This class holds and manage the properties of a building.
 * Every building game object should have this class attached.
 *
 *********************************************************/
public class BuildingManager : MonoBehaviour
{
    public int netId = -1;
    public int side = 0;
    public float width;
    public float length;
    public float health;
    public float maxHealth;
    public int damage = 40;

    public Building type;

    [SerializeField] private GameObject structure;
    [SerializeField] private GameObject destroyed;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject miniMapIcon;
    private GameObject DBInstance;

    [SerializeField] private float deathCountDown = 5.0f;
    private bool selected = false;
    private Vector3 rallyPoint;
    private bool isAlive = true;

    public GameObject target;
    public List<GameObject> enemyOnSight = new List<GameObject>();
    public List<GameObject> allyOnSight = new List<GameObject>();

    private Grid gridMap;
    public int w = 4;
    public int l = 4;

    private LineRenderer line;
    private const int cornerCnt = 4;
    private float[,] corners = {    { -10, -10 },
                                    {-10, 10 },
                                    {10, 10 },
                                    {10, -10 }};

    private GameObject gameManagerObj;
    private GameSetting gameSetting;
    private GlobalSelection globalSelection;
    private NetworkManager networkManager;

    public void init(int _side, Building _type)
    {
        side = _side;
        type = _type;
    }
    void Start()
    {
        gameManagerObj = GameObject.Find("GameManager");
        gameSetting = gameManagerObj.GetComponent<GameSetting>();
        gridMap = gameManagerObj.GetComponent<Grid>();
        globalSelection = GameObject.Find("Event System").GetComponent<GlobalSelection>();

        rallyPoint = transform.position + transform.forward * width;

        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = cornerCnt;
        line.useWorldSpace = false;
        line.startColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        line.endColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        line.startWidth = 0.3f;
        line.endWidth = 0.3f;
        CreatePoints();
        line.loop = true;
        line.enabled = false;

        if(side != GameObject.Find("PlayerInitSetting").GetComponent<PlayerInitSetting>().side)
        {
            healthBar.GetComponent<HealthBarManager>().SetColor(new Color(255.0f, 0.0f, 0.0f));
            miniMapIcon.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            miniMapIcon.GetComponent<Renderer>().material.color = Color.blue;
        }
        GameObject network = GameObject.Find("Network");
        if (network != null)
        {
            networkManager = network.GetComponent<NetworkManager>();
        }
    }
    public void LogicUpdate(float logicLen)
    {
        if (isAlive)
        {
            SetGrid();
        }
        switch (type)
        {
            case Building.Defense:
                {
                    if (target == null) target = GetATarget();
                    else
                    {
                        if (target.tag == "Dead" || !enemyOnSight.Contains(target)) target = null;
                    }
                }
                break;
            default:
                break;
        }
    }
    public void RenderUpdate(float renderLerpValue)
    {
        healthBar.GetComponent<HealthBarManager>().SetBar(new Vector3(health / maxHealth, 1.0f, 1.0f));
    }

    void CreatePoints()
    {
        for(int i = 0; i < cornerCnt; i++)
        {
            line.SetPosition(i, new Vector3(corners[i, 0], 0.3f, corners[i, 1]));
        }
    }

    // Some types of buildings can spawn a specific type of unit for a player.
    public GameObject SpawnUnit(PlayerManager pm, Unit unitType)
    {
        GameObject newUnit = Instantiate(gameSetting.UnitPrefabs[(int)pm.hero_t, (int)unitType], new Vector3(transform.position.x - 12.0f, transform.position.y, transform.position.z + 12.0f), new Quaternion());
        newUnit.GetComponent<UnitManager>().Init(pm, unitType);
        Vector3 rallyPoint = GetRallyPoint();
        //Debug.Log(rallyPoint);
        newUnit.GetComponent<TestController>().Init(rallyPoint);
        return newUnit;
    }
    public Vector3 GetRallyPoint()
    {
        return gridMap.GetNearestLoc(rallyPoint.x, rallyPoint.z);
    }
    // Get the nearest enemy within attack range.
    public GameObject GetATarget()
    {
        if (enemyOnSight.Count == 0) return null;
        GameObject res = null;
        float minHealth = 1000000f;
        List<GameObject> toRemove = new List<GameObject>();
        foreach(GameObject obj in enemyOnSight)
        {
            if(obj == null)
            {
                toRemove.Add(obj);
                continue;
            }
            UnitManager um = obj.GetComponent<UnitManager>();
            BuildingManager bm = obj.GetComponent<BuildingManager>();
            if(um != null)
            {
                if(!um.IsAlive())
                {
                    toRemove.Add(obj);
                    continue;
                }
                if(um.health < minHealth)
                {
                    res = obj;
                    minHealth = um.health;
                }
            }
            else if(bm != null)
            {
                if(!um.IsAlive())
                {
                    toRemove.Add(obj);
                    continue;
                }
                if(bm.health < minHealth)
                {
                    res = obj;
                    minHealth = bm.health;
                }
            }
        }
        foreach(GameObject obj in toRemove)
        {
            enemyOnSight.Remove(obj);
        }
        //Debug.Log("target gotten" + res);
        return res;
    }
    public bool OnDamage(GameObject ob, float damage)
    {
        if (!isAlive) return false;
        health = Mathf.Max(0.0f, health - damage);
        if(health == 0.0f)
        {
            OnDeath();
            return true;
        }
        return false;
    }
    public bool IsAlive
    {
        get { return isAlive; }
        set { isAlive = value; }
    }
    public Vector3 RallyPoint
    {
        get { return rallyPoint; }
        set { rallyPoint = value; }
    }
    public bool Selected
    {
        get { return selected; }
        set { selected = value; }
    }

    public void Select()
    {
        selected = true;
        line.enabled = true;
    }

    public void UnSelect()
    {
        selected = false;
        line.enabled = false;
    }
    public bool DeathCountDown()
    {
        deathCountDown -= Time.deltaTime;
        if (deathCountDown <= 0) return true;
        return false;
    }
    void SetGrid()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 startPos = new Vector3();
        if(w % 2 == 0)
        {
            startPos.x = transform.position.x - (w / 2 * gridMap.unitW - gridMap.unitW / 2);
        }
        else
        {
            startPos.x = transform.position.x - (w / 2 * gridMap.unitW);
        }
        if (l % 2 == 0)
        {
            startPos.z = transform.position.z - (l / 2 * gridMap.unitW - gridMap.unitW / 2);
        }
        else
        {
            startPos.z = transform.position.z - (l / 2 * gridMap.unitW);
        }
        for (int i = 0; i < w; i++)
        {
            for(int j = 0; j < l; j++)
            {
                Vector3 point = startPos;
                point.x = startPos.x + i * gridMap.unitW;
                point.z = startPos.z + j * gridMap.unitW;
                points.Add(point);
            }
        }
        gridMap.SetGridPos(points, gameObject.GetInstanceID());
    }
    void OnDeath()
    {
        isAlive = false;
        miniMapIcon.SetActive(false);
        if (selected)
        {
            globalSelection.selectTable.Deselect(gameObject);
        }
        gameObject.tag = "Dead";
        gridMap.RemoveObstacle(gameObject.GetInstanceID());
        structure.SetActive(false);
        healthBar.SetActive(false);
        DBInstance = Instantiate(destroyed, transform);
    }

    private void OnDestroy()
    {
        Destroy(DBInstance);
        networkManager.RemoveSyncItem(netId, gameObject);
    }
}
