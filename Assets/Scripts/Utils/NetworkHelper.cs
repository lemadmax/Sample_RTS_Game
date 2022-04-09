using UnityEngine;
using SLua;
/*********************************************************
 *
 * -------------------- NetworkHelper --------------------
 * This class can be instantiated and called from SLua.
 * Work as an interface of C# for Lua.
 *
 *********************************************************/
[CustomLuaClass]
public class NetworkHelper : MonoBehaviour
{
    public PlayerInitSetting pis;
    [SerializeField] private NetworkManager networkManager;
    public void SetPlayerId(int id)
    {
        pis.playerId = id;
    }
    public void SetPlayer(int id, string name, Hero hero_t, bool ready)
    {
        int idx = id - 1;
        pis.allPlayers[idx].id = id;
        pis.allPlayers[idx].name = name;
        pis.allPlayers[idx].heroType = hero_t;
        if (ready) pis.allPlayers[idx].status = "(ready)";
        else pis.allPlayers[idx].status = "()";
    }
    public void StartGame()
    {
        networkManager.StartGame = true;
    }
    public void LogicUpdate(float logicLen)
    {
        networkManager.LogicUpdate(logicLen);
    }
    public void RenderUpdate(float renderLerpValue)
    {
        networkManager.RenderUpdate(renderLerpValue);
    }
    public void DispatchCommand(CommandType type, Vector3 vec3, int id, int cnt, int[] ids)
    {
        networkManager.DispatchCommand(type, vec3, id, cnt, ids);
    }
}
