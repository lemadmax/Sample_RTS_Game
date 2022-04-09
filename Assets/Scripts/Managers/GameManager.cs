using System.Collections.Generic;
using UnityEngine;
/*********************************************************
 *
 * --------------------- GameManager --------------------
 * Start and initialize game.
 * End and conclude game.
 * Hold this client's player information.
 *
 *********************************************************/
public class GameManager : MonoBehaviour
{
    public int id = 0;
    public int side = 1;
    public int winStatus = -1;
    public GameObject playerPrefab;
    public GameObject blueBase;
    public GameObject redBase;
    public GameObject player;

    public GameObject cameraRig;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] initGameObjects;

    Dictionary<CommandType, KeyCode> keyMap = new Dictionary<CommandType, KeyCode>();

    GameSetting gameSetting;
    GameObject playerInitSetting;
    NetworkManager networkManager;

    void Start()
    {
        gameSetting = GetComponent<GameSetting>();
        playerInitSetting = GameObject.Find("PlayerInitSetting");
        networkManager = GameObject.Find("Network").GetComponent<NetworkManager>();
        for (int i = 0; i < initGameObjects.Length; i++)
        {
            if (initGameObjects[i].tag == "Building")
            {
                networkManager.AddSyncItem(initGameObjects[i].GetComponent<BuildingManager>().netId, initGameObjects[i]);
            }
            else if (initGameObjects[i].tag == "Player")
            {
                networkManager.AddSyncItem(initGameObjects[i].GetComponent<PlayerManager>().netId, initGameObjects[i]);
            }
            else if (initGameObjects[i].tag == "Unit")
            {
                networkManager.AddSyncItem(initGameObjects[i].GetComponent<UnitManager>().netId, initGameObjects[i]);
            }
        }
        if (networkManager.networkEnabled)
        {
            if (playerInitSetting != null)
            {
                PlayerInitSetting pis = playerInitSetting.GetComponent<PlayerInitSetting>();

                for (int i = 0; i < 8; i++)
                {
                    int pid = i + 1;
                    string name = pis.allPlayers[i].name;
                    Debug.Log("creating player: " + name);
                    if (name == "Empty") continue;
                    Hero pHero_t = pis.allPlayers[i].heroType;
                    Vector3 pSpawnPoint = spawnPoints[i].position;
                    GameObject pPlayer = Instantiate(playerPrefab);
                    if (pid == pis.playerId)
                    {
                        id = pid;
                        side = pis.side;
                        player = pPlayer;
                    }
                    PlayerManager ppm = pPlayer.GetComponent<PlayerManager>();
                    GameObject pHero = Instantiate(gameSetting.heroPrefabs[(int)pHero_t], pSpawnPoint, new Quaternion());
                    ppm.hero = pHero;
                    ppm.hero_t = pHero_t;
                    ppm.side = i / 4 + 1;
                    ppm.playerId = pid;
                    ppm.gold = pis.gold;
                    ppm.baseBuilding = ppm.side == 1 ? blueBase : redBase;
                    pHero.GetComponent<UnitManager>().pm = ppm;

                    pHero.GetComponent<UnitManager>().netId = networkManager.AddSyncItem(-1, pHero);
                    ppm.netId = networkManager.AddSyncItem(-1, pPlayer);
                }
            }
        }
        else
        {
            PlayerInitSetting pis = playerInitSetting.GetComponent<PlayerInitSetting>();
            id = pis.playerId;
            side = pis.side;
            Vector3 heroSpawnPoint = spawnPoints[id].position;
            player = Instantiate(playerPrefab);
            PlayerManager pm = player.GetComponent<PlayerManager>();
            GameObject hero = Instantiate(pis.hero, heroSpawnPoint, new Quaternion());
            pm.hero = hero;
            pm.hero_t = pis.heroType;
            pm.side = pis.side;
            pm.playerId = id;
            pm.gold = pis.gold;
            pm.baseBuilding = side == 1 ? blueBase : redBase;
            hero.GetComponent<UnitManager>().pm = pm;

            hero.GetComponent<UnitManager>().netId = networkManager.AddSyncItem(-1, hero);
            pm.netId = networkManager.AddSyncItem(-1, player);
        }
        InitializeKeyMap();
    }
    private void Update()
    {
        if(blueBase.GetComponent<BuildingManager>().tag == "Dead")
        {
            if(side == 1)
            {
                winStatus = 0;
            }
            else
            {
                winStatus = 1;
            }
        }
        if (redBase.GetComponent<BuildingManager>().tag == "Dead")
        {
            if (side == 1)
            {
                winStatus = 1;
            }
            else
            {
                winStatus = 0;
            }
        }
    }
    private void LateUpdate()
    {
        if(networkManager.networkEnabled) networkManager.ClientReady();
    }
    private void InitializeKeyMap()
    {
        keyMap[CommandType.Focus] = KeyCode.Space;
        keyMap[CommandType.Follow] = KeyCode.F;
        keyMap[CommandType.Skill_0] = KeyCode.Q;
        keyMap[CommandType.Spawn_0] = KeyCode.A;
    }
    public KeyCode GetKeyCode(CommandType controll)
    {
        return keyMap[controll];
    }
    public bool NetWorkEnabled()
    {
        return networkManager.networkEnabled;
    }
}
