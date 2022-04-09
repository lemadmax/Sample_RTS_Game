using UnityEngine;

public class AsheSkill0 : MonoBehaviour
{
    public float damage = 50;
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.GetComponentInParent<UnitManager>().pm.playerId != GameObject.Find("GameManager").GetComponent<GameManager>().id)
            return;
        if (other.gameObject.transform.parent == gameObject.transform.parent) return;
        if(other.gameObject.tag == "UnitBody" && gameObject.GetComponentInParent<UnitManager>().pm.side != other.gameObject.GetComponentInParent<UnitManager>().pm.side)
        {
            gameObject.transform.parent.gameObject.GetComponent<UnitManager>().SendDamageCmd(other.gameObject.transform.parent.gameObject, damage);
        }
    }
}
