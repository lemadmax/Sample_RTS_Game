using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SLua;
/*********************************************************
 *
 * ------------------- NetworkManager --------------------
 * The gameObject of this class will be passed to the game
 * scene. 
 * This class maintains the Lua service of SLua which handles
 * the client server communication.
 * This class serves as a bridge between network and local.
 *
 *********************************************************/
public class NetworkManager : MonoBehaviour
{
    // Lua service and lua functions.
    LuaSvr luaSvr;
    LuaTable self;
    LuaFunction test;
    LuaFunction connect;
    LuaFunction update;
    LuaFunction fixedUpdate;
    LuaFunction clientReady;
    LuaFunction initLockStep;
    LuaFunction sendCommand;

    public bool networkEnabled = false; // Game mode(Single-player/Multi-player).

    [SerializeField] private GameObject ipAddrEditor;
    TMP_InputField ipAddrIF;

    PlayerInitSetting pis;
    NetworkHelper nh;

    bool gameStarted = false;
    int nextId = 15;

    Dictionary<int, GameObject> netIdMap = new Dictionary<int, GameObject>();
    List<GameObject> netObjs = new List<GameObject>();

    //Queue<Command> commands = new Queue<Command>();
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ipAddrIF = ipAddrEditor.GetComponent<TMP_InputField>();
        pis = GameObject.Find("PlayerInitSetting").GetComponent<PlayerInitSetting>();
        nh = GameObject.Find("CalledBySLua").GetComponent<NetworkHelper>();
        DontDestroyOnLoad(nh.gameObject);


        luaSvr = new LuaSvr();
        luaSvr.init(null, () =>
        {
            self = (LuaTable)luaSvr.start("LuaScripts/TestClient");
            test = (LuaFunction)self["Test"];
            connect = (LuaFunction)self["Connect"];
            update = (LuaFunction)self["Update"];
            fixedUpdate = (LuaFunction)self["FixedUpdate"];
            clientReady = (LuaFunction)self["ClientReady"];
            initLockStep = (LuaFunction)self["InitLockStep"];
            sendCommand = (LuaFunction)self["SendCommand"];
        });
        initLockStep.call(self);
        //Vector3 testVec3 = (Vector3)test.call(self);
    }

    void Update()
    {
        if (networkEnabled)
        {
            update.call(self);
        }
    }

    private void FixedUpdate()
    {
        if(gameStarted && networkEnabled) fixedUpdate.call(self, Time.fixedDeltaTime);
        if(!networkEnabled)
        {
            LogicUpdate(Time.fixedDeltaTime);
            RenderUpdate(1.0f);
        }
    }

    // Setup connection to the given ip address.
    public void OnConnectClicked()
    {
        connect.call(self, ipAddrIF.text, pis.side, (int)pis.heroType, pis.nickName);
        pis.playerId = 10;
    }

    // Register a new object to be synchronized.
    // Registered object will be updated by the network service
    //     instead of unity's update loop.
    // Return a unique net id for the registered object.
    public int AddSyncItem(int id, GameObject obj)
    {
        if(id == -1)
        {
            id = nextId++;
        }
        netObjs.Add(obj);
        netIdMap.Add(id, obj);
        return id;
    }

    // Inform the server that this client is ready for the first
    //     frame.
    public void ClientReady()
    {
        if (!gameStarted) clientReady.call(self);
    }

    // Remove an object from the synchronized items.
    public void RemoveSyncItem(int id, GameObject obj)
    {
        netIdMap.Remove(id);
        netObjs.Remove(obj);
    }

    public bool StartGame
    {
        get { return gameStarted; }
        set { gameStarted = value; }
    }

    // Update logical frame for all synchronized objects.
    public void LogicUpdate(float logicLen)
    {
        foreach(GameObject obj in netObjs)
        {
            if(obj.tag == "Unit") obj.GetComponent<UnitManager>().LogicUpdate(logicLen);
            if(obj.tag == "Player") obj.GetComponent<PlayerManager>().LogicUpdate(logicLen);
            if(obj.tag == "Building") obj.GetComponent<BuildingManager>().LogicUpdate(logicLen);
        }
    }

    // Update rendering frame for all synchronized objects.
    public void RenderUpdate(float renderLerpValue)
    {
        foreach (GameObject obj in netObjs)
        {
            if(obj.tag == "Unit") obj.GetComponent<UnitManager>().RenderUpdate(renderLerpValue);
            if(obj.tag == "Player") obj.GetComponent<PlayerManager>().RenderUpdate(renderLerpValue);
            if(obj.tag == "Building") obj.GetComponent<BuildingManager>().RenderUpdate(renderLerpValue);
        }
    }

    // Submit a player's command for synchronization.
    public void PushCommand(CommandType type, Vector3 vec3, int id, int cnt, int[] ids)
    {
        if(!networkEnabled)
        {
            DispatchCommand(type, vec3, id, cnt, ids);
            return;
        }
        if (!gameStarted) return;
        //commands.Enqueue(new Command(type, vec3, id, cnt, ids));
        sendCommand.call(self, (int)type, vec3, id, cnt, ids);
    }

    // Dispatch and execute commands received from server.
    public void DispatchCommand(CommandType type, Vector3 vec3, int id, int cnt, int[] ids)
    {
        switch(type)
        {
            case CommandType.SetTarget:
                {
                    GameObject target = id == -1 ? null : netIdMap[id];
                    if(target != null)
                    {
                        for(int i = 0; i < cnt; i++)
                        {
                            GameObject obj = netIdMap[ids[i]];
                            //Debug.Log("applying command to: ", obj);
                            obj.GetComponent<UnitManager>().SetTarget(target, vec3);
                        }
                    }
                    else
                    {
                        cnt = cnt - 1;
                        GameObject hero = netIdMap[ids[cnt]];
                        if(hero.GetComponent<UnitManager>().type != Unit.Hero)
                        {
                            hero = null;
                            cnt++;
                        }
                        
                        GameObject[] units = new GameObject[cnt];
                        for(int i = 0; i < cnt; i++)
                        {
                            GameObject obj = netIdMap[ids[i]];
                            units[i] = obj;
                        }
                        CrowdArrange crowdArrange = GameObject.Find("Event System").GetComponent<CrowdArrange>();
                        Dictionary<int, Vector3> positions = crowdArrange.GetTargetPos(hero, units, vec3);
                        if (hero != null) hero.GetComponent<UnitManager>().SetTarget(null, vec3);
                        for (int i = 0; i < units.Length; i++)
                        {
                            if(hero != null) units[i].GetComponent<LineRenderer>().enabled = true;
                            units[i].GetComponent<UnitManager>().SetTarget(null, positions[units[i].GetInstanceID()], hero != null);
                        }
                    }
                }
                break;
            case CommandType.Skill_0:
                {
                    GameObject target = id == -1 ? null : netIdMap[id];
                    for(int i = 0; i < cnt; i++)
                    {
                        GameObject obj = netIdMap[ids[i]];
                        Debug.Log("Obj is using skill 0: ", obj);
                        if(obj.tag == "Unit")
                        {
                            obj.GetComponent<UnitManager>().StartSkillAnim(0, target, vec3);
                        }
                    }
                }
                break;
            case CommandType.Spawn_0:
                {
                    GameObject player = id == -1 ? null : netIdMap[id];
                    for(int i = 0; i < cnt; i++)
                    {
                        GameObject obj = netIdMap[ids[i]];
                        Debug.Log("spawning infantry");
                        if(obj.tag == "Building")
                        {
                            BuildingManager bm = obj.GetComponent<BuildingManager>();
                            bm.RallyPoint = vec3;
                            GameObject unit = player.GetComponent<PlayerManager>().UnitSpawn(Unit.Infantry, bm);
                            Debug.Log(unit);
                            if(unit != null)
                            {
                                unit.GetComponent<UnitManager>().netId = AddSyncItem(-1, unit);
                            }
                        }
                    }
                }
                break;
            case CommandType.Damage:
                {
                    GameObject enemy = id == -1 ? null : netIdMap[id];
                    GameObject dealer = netIdMap[ids[0]];
                    Debug.Log("dealing damage");
                    if(dealer.tag == "Unit")
                    {
                        dealer.GetComponent<UnitManager>().DealDamage(enemy, vec3.x);
                    }
                }
                break;
            default:
                {

                }
                break;
        }
    }
}
