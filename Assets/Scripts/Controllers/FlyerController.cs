using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerController : MonoBehaviour
{
    public PlayerManager pm;
    public NetworkManager nm;
    public GameObject owner;
    public GameObject target;
    public float speed;
    public int damage;

    public bool directional = true;
    public Vector3 startPos;
    public Vector3 targetPos;

    GameObject gameManager;

    Vector3 newPosition;

    HashSet<int> hitUnits = new HashSet<int>();

    void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    
    void Update()
    {
        if(directional)
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
            UnitManager um = target.GetComponent<UnitManager>();
            BuildingManager bm = target.GetComponent<BuildingManager>();
            if(um != null && !um.IsAlive())
            {
                Destroy(gameObject);
                return;
            }
            else if(bm != null && !bm.IsAlive)
            {
                Destroy(gameObject);
                return;
            }
            Vector3 targetPos = target.transform.position;
            targetPos.y = targetPos.y + 5.0f;
            Vector3 direction = Vector3.Normalize(targetPos - transform.position);
            transform.forward = direction;
            newPosition = transform.position + speed * direction * Time.deltaTime;
            transform.position = newPosition;
        }
        else
        {
            transform.position = transform.position + speed * transform.forward * Time.deltaTime;
            if((targetPos - startPos).magnitude <= (transform.position - startPos).magnitude)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pm.playerId != gameManager.GetComponent<GameManager>().id)
            return;
        if (directional)
        {
            if (target != null && other.transform.parent.gameObject != null && target == other.transform.parent.gameObject)
            {
                if (hitUnits.Contains(target.GetInstanceID()))
                    return;
                //DealDamage();
                if(owner.tag == "Unit")
                {
                    UnitManager um = owner.GetComponent<UnitManager>();
                    um.SendDamageCmd(target, damage);
                    hitUnits.Add(target.GetInstanceID());
                }
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.tag == "UnitBody" && other.transform.parent.GetComponent<UnitManager>().pm.side != pm.side)
            {
                if (hitUnits.Contains(other.transform.parent.gameObject.GetInstanceID()))
                    return;
                //DealDamage(other.transform.parent.gameObject);
                if (owner.tag == "Unit")
                {
                    UnitManager um = owner.GetComponent<UnitManager>();
                    um.SendDamageCmd(other.transform.parent.gameObject, damage);
                    hitUnits.Add(other.transform.parent.gameObject.GetInstanceID());
                }
            }
        }
    }


    public void DealDamage()
    {
        if (target != null && target.tag == "Unit")
        {
            UnitManager targetManager = target.GetComponent<UnitManager>();
            bool killed = targetManager.OnDamage(owner, damage);
            if (killed)
            {
                GameSetting gameSetting = gameManager.GetComponent<GameSetting>();
                int bounty = (int)(gameSetting.UnitPrice[(int)targetManager.pm.hero_t, (int)targetManager.type] * gameSetting.bountyRate);
                if(pm != null) pm.AddGold(bounty);
            }
        }
        if (target != null && target.tag == "Building")
        {
            BuildingManager targetManager = target.GetComponent<BuildingManager>();
            bool destroyed = target.GetComponent<BuildingManager>().OnDamage(owner, damage);
            if (destroyed)
            {
                GameSetting gameSetting = gameManager.GetComponent<GameSetting>();
                int bounty = (int)(gameSetting.BuildingPrice[targetManager.side, (int)targetManager.type] * gameSetting.bountyRate);
                if (pm != null) pm.AddGold(bounty);
            }
        }
    }
    public void DealDamage(GameObject target)
    {
        if (target != null && target.tag == "Unit")
        {
            UnitManager targetManager = target.GetComponent<UnitManager>();
            bool killed = targetManager.OnDamage(owner, damage);
            if (killed)
            {
                GameSetting gameSetting = gameManager.GetComponent<GameSetting>();
                int bounty = (int)(gameSetting.UnitPrice[(int)targetManager.pm.hero_t, (int)targetManager.type] * gameSetting.bountyRate);
                if (pm != null) pm.AddGold(bounty);
            }
        }
        if (target != null && target.tag == "Building")
        {
            BuildingManager targetManager = target.GetComponent<BuildingManager>();
            bool destroyed = target.GetComponent<BuildingManager>().OnDamage(owner, damage);
            if (destroyed)
            {
                GameSetting gameSetting = gameManager.GetComponent<GameSetting>();
                int bounty = (int)(gameSetting.BuildingPrice[targetManager.side, (int)targetManager.type] * gameSetting.bountyRate);
                if (pm != null) pm.AddGold(bounty);
            }
        }
    }
}
