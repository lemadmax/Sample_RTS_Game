using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanSkill0 : MonoBehaviour
{
    public int damage = 50;
    public float speed = 100;
    public float range = 30;

    public GameObject spell;
    private void Start()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            Vector3 position = transform.position;
            GameObject flyer = Instantiate(spell, position, transform.rotation);
            FlyerController fc = flyer.GetComponent<FlyerController>();
            fc.pm = GetComponentInParent<UnitManager>().pm;
            fc.nm = GameObject.Find("Network").GetComponent<NetworkManager>();
            fc.owner = gameObject.transform.parent.gameObject;
            fc.target = null;
            fc.speed = speed;
            fc.damage = damage;
            fc.directional = false;
            fc.startPos = position;
            fc.targetPos = position + transform.forward * range;
            GetComponentInParent<UnitManager>().EndSkill(0);
        }
    }
}
