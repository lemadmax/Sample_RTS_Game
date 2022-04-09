using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject heroName;
    public GameObject sideName;
    public GameObject heroAvatar;
    public GameObject baseAvatar;
    public GameObject skillBtn;
    public GameObject infantryBtn;
    public GameObject heroHealthBar;
    public GameObject baseHealthBar;
    public GameObject gameSettingsUI;
    public GameObject winUI;
    public GameObject loseUI;

    private UnitManager playerHero;
    private BuildingManager playerBase;

    void Start()
    {
        TextMeshProUGUI textMeshHeroName = heroName.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textMeshSideName = sideName.GetComponent<TextMeshProUGUI>();
        GameManager manager = gameManager.GetComponent<GameManager>();
        PlayerManager pm = manager.player.GetComponent<PlayerManager>();
        Hero curHero = pm.hero_t;
        playerHero = pm.hero.GetComponent<UnitManager>();
        playerBase = pm.baseBuilding.GetComponent<BuildingManager>();
        int side = pm.side;
        Image heroAvatarImg = heroAvatar.GetComponent<Image>();
        Image baseAvatarImg = baseAvatar.GetComponent<Image>();
        Image skillImg = skillBtn.GetComponent<Image>();
        Image infantryImg = infantryBtn.GetComponent<Image>();

        if (curHero == Hero.Ashe)
        {
            string text = "Ashe";
            textMeshHeroName.text = text;
            heroAvatarImg.sprite = Resources.Load<Sprite>("Ashe/Ashe");
            skillImg.sprite = Resources.Load<Sprite>("Ashe/Ashe_skill");
            skillImg.color = Color.white;
        }
        else if (curHero == Hero.BlackWidow)
        {
            string text = "Black Widow";
            textMeshHeroName.text = text;
            heroAvatarImg.sprite = Resources.Load<Sprite>("Widow/Widow");
        }
        else if (curHero == Hero.OrcShaman)
        {
            string text = "Orc Shaman";
            textMeshHeroName.text = text;
            heroAvatarImg.sprite = Resources.Load<Sprite>("Shaman/Shaman");
            skillImg.sprite = Resources.Load<Sprite>("Shaman/Shaman_skill");
            skillImg.color = Color.white;
        }

        if (side == 1)
        {
            string text = "Human";
            textMeshSideName.text = text;
            baseAvatarImg.sprite = Resources.Load<Sprite>("Human/Base");
            infantryImg.sprite = Resources.Load<Sprite>("Human/Infantry");
            infantryImg.color = Color.white;
        }
        else if (side == 2)
        {
            string text = "Orc";
            textMeshSideName.text = text;
            baseAvatarImg.sprite = Resources.Load<Sprite>("Orc/Base");
            infantryImg.sprite = Resources.Load<Sprite>("Orc/Infantry");
            infantryImg.color = Color.white;
        }

        gameSettingsUI.SetActive(false);
        winUI.SetActive(false);
        loseUI.SetActive(false);
    }

    void Update()
    {
        heroHealthBar.GetComponent<HealthBarManager>().SetBar(new Vector3(playerHero.health / playerHero.maxHealth, 1.0f, 1.0f));
        baseHealthBar.GetComponent<HealthBarManager>().SetBar(new Vector3(playerBase.health / playerBase.maxHealth, 1.0f, 1.0f));
        if(gameManager.GetComponent<GameManager>().winStatus == 0)
        {
            loseUI.SetActive(true);
        }
        else if (gameManager.GetComponent<GameManager>().winStatus == 1)
        {
            winUI.SetActive(true);
        }
    }
}