using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    GameManager gameManager;
    UnitManager um;
    GameObject target;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        um = gameObject.GetComponentInParent<UnitManager>();
    }
    public void DealDamage()
    {
        if (gameManager.NetWorkEnabled() && um.GetPlayerId() != gameManager.id) return;
        if (gameManager.NetWorkEnabled())
        {
            um.SendDamageCmd(um.target, um.autoDamage);
        }
        else
        {
            um.DealDamage(um.target, um.autoDamage);
        }
    }
    public void Loose()
    {
        um.LooseFlyer1();
    }
    public void AttackEnd()
    {
        GetComponent<Animator>().SetInteger("states", 0);
    }
    public void StartSkill(int id)
    {
        um.StartSkill(id);
    }
    public void EndSkill(int id)
    {
        um.EndSkill(id);
    }
}
