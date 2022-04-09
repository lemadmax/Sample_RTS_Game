using UnityEngine;
/*********************************************************
 *
 * ---------------- PlayerInitSetting --------------------
 * The gameObject of this class will be passed to the game
 * scene. 
 * The instance of this class hold the setting details of 
 * the game for the game scene to initialize the players.
 *
 *********************************************************/
public class PlayerInitSetting : MonoBehaviour
{
    // Information of the current player
    public string       nickName;
    public int          side;
    public Hero         heroType;
    public GameObject   hero;
    public int          playerId = 0;
    public int          gold = 500;

    // Information of all players in case of the multi-player mode.
    public PlayerInfo[] allPlayers = new PlayerInfo[8];
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }

}

// Struct that stores the information of each player.
public struct PlayerInfo
{
    public int      id;
    public string   name;
    public Hero     heroType;
    public string   status;
}