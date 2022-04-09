using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


public class InGameBtnClick : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject curCamera;
    public GameObject gameSettingsUI;

    private CameraController cameraCtrl;
    private GameManager manager;
    private PlayerManager pm;
    private GameSetting setting;
    private NetworkManager networkManager;

    void Start()
    {
        cameraCtrl = curCamera.GetComponent<CameraController>();
        manager = gameManager.GetComponent<GameManager>();
        setting = gameManager.GetComponent<GameSetting>();
        pm = manager.player.GetComponent<PlayerManager>();
        GameObject network = GameObject.Find("Network");
        if (network != null)
        {
            networkManager = network.GetComponent<NetworkManager>();
        }
    }
    public void SettingsBtnClick()
    {
        gameSettingsUI.SetActive(true);
    }
    public void ExitBtnClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void ReturnGameBtnClick()
    {
        gameSettingsUI.SetActive(false);
    }
    public void ReturnMainBtnClick()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene(0);
#else
        Application.LoadLevel(0);
#endif
    }
    public void HeroAvatarClick()
    {
        cameraCtrl.LookAtHero(pm.hero);
    }
    public void BaseAvatarClick()
    {
        cameraCtrl.LookAtHero(pm.baseBuilding);
    }
    public void SkillBtnClick()
    {
        Command cmd = pm.hero.GetComponent<UnitManager>().GetSkillCmd(0);
        networkManager.PushCommand(CommandType.Skill_0, cmd.vec3,
            cmd.id, cmd.cnt, cmd.ids);
    }
    public void InfantryBtnClick()
    {
        networkManager.PushCommand(CommandType.Spawn_0, pm.baseBuilding.GetComponent<BuildingManager>().RallyPoint,
                    pm.netId, 1, new int[] { pm.baseBuilding.GetComponent<BuildingManager>().netId });
    }
}