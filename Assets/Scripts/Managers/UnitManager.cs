using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*********************************************************
 *
 * --------------------- UnitManager --------------------
 * One of the main game logic class.
 * This class holds and manage the properties of a unit.
 * Every unit game object should have this class attached.
 *
 *********************************************************/
public class UnitManager : MonoBehaviour
{
    public int netId = -1;
    public float health;
    public float maxHealth;
    public float walkSpeed;
    public float attackSpeed;
    public float attackRange;
    public float autoDamage;

    public bool followHero = false;
    public bool autoSelectTarget = true;
    public bool onGuard = true;
    public float chaseDistance = 20;
    public Vector3 guardLoc;

    public GameObject body;
    public GameObject deadBody;
    public GameObject healthBar;
    public GameObject miniMapIcon;
    public GameObject flyer1;
    public float flyer1Speed;
    public float releaseHeight;
    public int flyer1Damage;
    GameObject DBInstance;

    public PlayerManager pm;
    public Unit type;
    public bool selected = false;
    bool isUnderAttack = false;
    bool isAlive = true;
    float deathCountDown = 15.0f;
    public bool attacking = false;
    public bool moving = false;
    public bool touchTarget = false;
    public GameObject target;
    public List<GameObject> enemyOnSight = new List<GameObject>();
    public List<GameObject> allyOnSight = new List<GameObject>();

    LineRenderer line;
    int segments = 50;
    public float radius = 2.5f;

    public float width = 5.0f;
    public float length = 5.0f;
    public Grid gridMap;
    TestController testController;

    public int avoidDir = 0;

    GameObject gameManagerObj;
    GameManager gameManager;
    GameSetting gameSetting;
    NetworkManager networkManager;

    public int numOfSkills = 3;
    [SerializeField] GameObject[] skills;

    public void Init(PlayerManager playerManager, Unit _type)
    {
        pm = playerManager;
        type = _type;
    }

    void Start()
    {
        guardLoc = transform.position;
        if (type == Unit.Hero)
        {
            autoSelectTarget = false;
            onGuard = false;
        }

        avoidDir = (int)(UnityEngine.Random.value * 100);

        gameManagerObj = GameObject.Find("GameManager");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        gridMap = gameManagerObj.GetComponent<Grid>();
        gameSetting = gameManagerObj.GetComponent<GameSetting>();
        testController = gameObject.GetComponent<TestController>();

        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.startWidth = 0.3f;
        line.endWidth = 0.3f;
        line.transform.localPosition.Set(0.0f, 0.4f, 0.0f);
        CreatePoints();
        line.enabled = false;

        if(pm.side != GameObject.Find("PlayerInitSetting").GetComponent<PlayerInitSetting>().side)
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
            if(networkManager.networkEnabled && GetComponentInChildren<CharColliderController>() != null)
            {
                GetComponentInChildren<CharColliderController>().enabled = false;
            }
        }
    }

    public void LogicUpdate(float logicLen)
    {
        if (gameManagerObj == null) return;
        if (isAlive)
        {
            SetGrid();
        }
        else return;
        if (autoSelectTarget)
        {
            AutoChangeTarget();
        }
        if (onGuard && testController.targetArrived)
        {
            AutoSelectTarget();
        }
        testController.LogicUpdate(logicLen);
    }
    public void RenderUpdate(float renderLerpValue)
    {
        if (!isAlive || gameManagerObj == null) return;
        healthBar.GetComponent<HealthBarManager>().SetBar(new Vector3(health / maxHealth, 1.0f, 1.0f));
        testController.RenderUpdate(renderLerpValue);
    }

    public bool IsUnderAttack()
    {
        return isUnderAttack;
    }
    public void AutoSelectTarget()
    {
        if (target != null && target.tag != "Dead") return;
        GameObject nTarget = GetATarget();
        if(nTarget != null)
        {
            if (networkManager.networkEnabled && pm.playerId == gameManager.id)
            {
                int targetId = nTarget.tag == "Unit" ? nTarget.GetComponent<UnitManager>().netId : nTarget.GetComponent<BuildingManager>().netId;
                networkManager.PushCommand(CommandType.SetTarget, nTarget.transform.position,
                        targetId, 1, new int[] { netId });
            }
            else
            {
                SetTarget(nTarget, nTarget.transform.position);
            }
        }
    }
    public void AutoChangeTarget()
    {
        if (target == null) return;
        GameObject nTarget = GetATarget();
        if(nTarget != null && nTarget != target)
        {
            float dis = (target.transform.position - transform.position).magnitude;
            if(dis > chaseDistance)
            {
                //Debug.Log("changing target");
                if (networkManager.networkEnabled && pm.playerId == gameManager.id)
                {
                    int targetId = nTarget.tag == "Unit" ? nTarget.GetComponent<UnitManager>().netId : nTarget.GetComponent<BuildingManager>().netId;
                    networkManager.PushCommand(CommandType.SetTarget, nTarget.transform.position,
                            targetId, 1, new int[] { netId });
                }
                else
                {
                    SetTarget(nTarget, nTarget.transform.position);
                }
            }
        }
    }
    public GameObject GetATarget()
    {
        GameObject res = null;
        float minDis = 5000.0f;
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
            if((um != null && !um.IsAlive()) || (bm != null && !bm.IsAlive))
            {
                toRemove.Add(obj);
                continue;
            }
            float dis = (obj.transform.position - transform.position).magnitude;
            if(dis < minDis)
            {
                minDis = dis;
                res = obj;
            }
        }
        foreach(GameObject obj in toRemove)
        {
            enemyOnSight.Remove(obj);
        }
        return res;
    }
    public void SetTarget(GameObject obj, Vector3 position)
    {
        target = obj;
        moving = true;
        testController.QuitAttack();
        testController.SetTargetPos(position);
        if (type == Unit.Hero)
        {
            for (int i = 0; i < numOfSkills; i++)
            {
                if (skills[i] != null && skills[i].activeInHierarchy)
                {
                    skills[i].GetComponent<SkillManager>().OnSetTarget();
                }
            }
        }
    }
    public void SetTarget(GameObject obj, Vector3 position, bool follow)
    {
        if (follow) FollowHero();
        else UnFollowHero();
        if (GetPlayerId() != gameManager.id) line.enabled = false;
        SetTarget(obj, position);
    }
    public void FollowHero()
    {
        followHero = true;
        line.startColor = new Color(1.0f, 0.7f, 0.0f);
        line.endColor = new Color(1.0f, 0.7f, 0.0f);
    }
    public void UnFollowHero()
    {
        followHero = false;
        line.startColor = new Color(1.0f, 1.0f, 1.0f);
        line.endColor = new Color(1.0f, 1.0f, 1.0f);
    }
    public void LooseFlyer1()
    {
        Vector3 position = transform.position;
        position.y = 5.0f;
        GameObject flyer = Instantiate(flyer1, position, transform.rotation);
        FlyerController fc = flyer.GetComponent<FlyerController>();
        fc.pm = pm;
        fc.owner = gameObject;
        fc.target = target;
        fc.speed = flyer1Speed;
        fc.damage = flyer1Damage;
    }
    public bool OnDamage(GameObject ob, float damage)
    {
        if (!isAlive) return false;
        isUnderAttack = true;
        health = Mathf.Max(0.0f, health - damage);
        if (health == 0.0f)
        {
            OnDeath();
            return true;
        }
        if(target == null && !moving && !networkManager.networkEnabled)
        {
            target = ob;
        }
        return false;
    }
    public Vector3 GetNearestLoc()
    {
        Vector3 res = gridMap.GetNearestLoc(target.transform.position.x, target.transform.position.z);
        Vector3 dir = Vector3.Normalize(target.transform.position - res);
        res = res + dir * radius;
        return res;
    }

    public int GetPlayerId()
    {
        return pm.playerId;
    }

    public void Select()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        selected = true;
        line.enabled = true;
    }

    public void UnSelect()
    {
        selected = false;
        line.enabled = false;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
    public void SendDamageCmd(GameObject enemy, float damage)
    {
        int targetId = 0;
        if (enemy.tag == "Unit") targetId = enemy.GetComponent<UnitManager>().netId;
        else targetId = enemy.GetComponent<BuildingManager>().netId;
        networkManager.PushCommand(CommandType.Damage, new Vector3(damage, 0.0f, 0.0f), targetId, 1, new int[] { netId });
    }
    public void DealDamage(GameObject enemy, float damage)
    {
        if (enemy != null && enemy.tag == "Unit")
        {
            UnitManager targetManager = enemy.GetComponent<UnitManager>();
            bool killed = targetManager.OnDamage(gameObject, damage);
            if (killed)
            {
                int bounty = (int)(gameSetting.UnitPrice[(int)targetManager.pm.hero_t, (int)targetManager.type] * gameSetting.bountyRate);
                pm.AddGold(bounty);
            }
        }
        if (enemy != null && enemy.tag == "Building")
        {
            BuildingManager targetManager = enemy.GetComponent<BuildingManager>();
            bool destroyed = enemy.GetComponent<BuildingManager>().OnDamage(gameObject, damage);
            if (destroyed)
            {
                int bounty = (int)(gameSetting.BuildingPrice[targetManager.side, (int)targetManager.type] * gameSetting.bountyRate);
                pm.AddGold(bounty);
            }
        }
    }
    public Command GetSkillCmd(int id)
    {
        if(skills[id] == null)
        {
            Debug.Log("skill obj is null");
        }
        return skills[id].GetComponent<SkillManager>().GetSkillCommand();
    }
    public void StartSkillAnim(int id, GameObject target, Vector3 vec3)
    {
        testController.animator.SetInteger("states", 11);
        if (skills[id] == null)
        {
            Debug.Log("skill obj is null");
        }
        skills[id].GetComponent<SkillManager>().OnSkillTriggered(target, vec3);
    }
    public void Halt()
    {
        testController.SetTargetPos(transform.position);
    }
    public void RotateTo(Vector3 dir)
    {
        transform.forward = dir;
    }
    public void StartSkill(int id)
    {
        skills[id].SetActive(true);
    }
    public void EndSkill(int id)
    {
        skills[id].SetActive(false);
        testController.animator.SetInteger("states", 0);
    }

    void CreatePoints()
    {
        float x;
        float z;
        float angle = 20f;
        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            line.SetPosition(i, new Vector3(x, 0.8f, z));
            angle += (360f / segments);
        }
    }

    void OnDeath()
    {
        isAlive = false;
        miniMapIcon.SetActive(false);
        if(selected)
        {
            pm.globalSelection.selectTable.Deselect(gameObject);
        }
        gridMap.RemoveObstacle(gameObject.GetInstanceID());
        gameObject.tag = "Dead";
        GetComponent<TestController>().animator.SetInteger("states", 3);
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<LineRenderer>().enabled = false;
        healthBar.SetActive(false);
        if(deadBody != null)
        {
            body.SetActive(false);
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = -90f;
            rotation.y = -90f;
            Quaternion rot = Quaternion.Euler(rotation);
            DBInstance = Instantiate(deadBody, transform.position, rot);
        }
        else
        {
            body.GetComponent<CapsuleCollider>().enabled = false;
        }
        if(type != Unit.Hero) pm.UnitDeath(gameObject);
    }
    
    void SetGrid()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 pos = transform.position;
        points.Add(pos);
        gridMap.SetGridPos(points, gameObject.GetInstanceID());
    }
    public bool DeathCountDown()
    {
        deathCountDown -= Time.deltaTime;
        if (deathCountDown <= 0) return true;
        return false;
    }

    private void OnDestroy()
    {
        if(DBInstance != null)
            Destroy(DBInstance);
        networkManager.RemoveSyncItem(netId, gameObject);
    }
    
}
