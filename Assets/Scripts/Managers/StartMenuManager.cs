using UnityEngine;
using TMPro;
/*********************************************************
 *
 * ---------------- StartMenuManager --------------------
 * This class manage the interactive fields in the start 
 * scene. Update and record the player's selections and
 * inputs.
 *
 *********************************************************/
public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject nickName;
    [SerializeField] private GameObject ipAddr;
    [SerializeField] private GameObject campSelect;
    [SerializeField] private GameObject heroSelect;
    [SerializeField] private GameObject connection;
    [SerializeField] private GameObject[] players;
    [SerializeField] private PlayerInitSetting pis;
    [SerializeField] private GameObject spinner;
    [SerializeField] private GameObject[] heros;

    void Start()
    {
        spinner.SetActive(false);

        // Initialize the game setting to the default values.
        OnNameChanged();
        OnSideSelected();
        OnHeroSelected();
    }

    
    void Update()
    {
        if(pis.playerId == 10)
        {
            connection.GetComponent<TMP_Text>().text = "";
            spinner.SetActive(true);
        }
        else if (pis.playerId > 0)
        {
            spinner.SetActive(false);
            connection.GetComponent<TMP_Text>().text = "Connected!";
            for (int i = 0; i < 8; i++)
            {
                players[i].transform.Find("Nickname").GetComponent<TMP_Text>().text = pis.allPlayers[i].name + pis.allPlayers[i].status;
                if (pis.allPlayers[i].heroType == Hero.Nature)
                {
                    players[i].transform.Find("Status").GetComponent<TMP_Text>().text = "";
                }
                else
                {
                    players[i].transform.Find("Status").GetComponent<TMP_Text>().text = heroSelect.GetComponent<TMP_Dropdown>().options[(int)pis.allPlayers[i].heroType - 1].text;
                }
            }
        }
    }
    public void OnSideSelected()
    {
        TMP_Dropdown dpn = campSelect.GetComponent<TMP_Dropdown>();
        pis.side = dpn.value + 1;
    }
    public void OnHeroSelected()
    {
        TMP_Dropdown heroDpn = heroSelect.GetComponent<TMP_Dropdown>();
        pis.heroType = (Hero)(heroDpn.value + 1);
        pis.hero = heros[heroDpn.value];
    }
    public void OnNameChanged()
    {
        pis.nickName = nickName.GetComponent<TMP_InputField>().text;
    }
}
