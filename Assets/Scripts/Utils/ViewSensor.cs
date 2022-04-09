using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSensor : MonoBehaviour
{
    public float viewRange = 50;

    UnitManager unitManager;
    BuildingManager buildingManager;
    //SphereCollider collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null) return;
        unitManager = GetComponentInParent<UnitManager>();
        buildingManager = GetComponentInParent<BuildingManager>();
        GameObject parent = other.transform.parent.gameObject;
        if(unitManager != null)
        {
            if (parent == unitManager.gameObject) return;
            if (parent.tag == "Unit")
            {
                if(parent.GetComponent<UnitManager>().pm.side != unitManager.pm.side)
                {
                    unitManager.enemyOnSight.Add(parent);
                }
                else
                {
                    unitManager.allyOnSight.Add(parent);
                }
            }
            else if(parent.tag == "Building")
            {
                if (parent.GetComponent<BuildingManager>().side != unitManager.pm.side)
                {
                    unitManager.enemyOnSight.Add(parent);
                }
                else
                {
                    unitManager.allyOnSight.Add(parent);
                }
            }
        }
        else
        {
            if (parent == buildingManager.gameObject) return;
            if(parent.tag == "Unit")
            {
                //Debug.Log("an unit hit" + parent);
                if (parent.GetComponent<UnitManager>().pm.side != buildingManager.side)
                {
                    buildingManager.enemyOnSight.Add(parent);
                }
                else
                {
                    buildingManager.allyOnSight.Add(parent);
                    //Debug.Log("Building find an ally" + parent);
                }
            }
            else if(parent.tag == "Building")
            {
                if(parent.GetComponent<BuildingManager>().side != buildingManager.side)
                {
                    buildingManager.enemyOnSight.Add(parent);
                }
                else
                {
                    buildingManager.allyOnSight.Add(parent);
                }
            }
        }
        
        //Debug.Log(unitManager.gameObject + ": " + unitManager.enemyOnSight.Count);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == null) return;
        unitManager = GetComponentInParent<UnitManager>();
        buildingManager = GetComponentInParent<BuildingManager>();
        GameObject parent = other.transform.parent.gameObject;
        if(unitManager != null)
        {
            if (parent == unitManager.gameObject) return;
            if (parent.tag == "Unit")
            {
                if (parent.GetComponent<UnitManager>().pm.side != unitManager.pm.side)
                {
                    unitManager.enemyOnSight.Remove(parent);
                }
                else
                {
                    unitManager.allyOnSight.Remove(parent);
                }
            }
            else if (parent.tag == "Building")
            {
                if (parent.GetComponent<BuildingManager>().side != unitManager.pm.side)
                {
                    unitManager.enemyOnSight.Remove(parent);
                }
                else
                {
                    unitManager.allyOnSight.Remove(parent);
                }
            }
        }
        else
        {
            if (parent == buildingManager.gameObject) return;
            if(parent.tag == "Unit")
            {
                if(parent.GetComponent<UnitManager>().pm.side != buildingManager.side)
                {
                    buildingManager.enemyOnSight.Remove(parent);
                }
                else
                {
                    buildingManager.allyOnSight.Remove(parent);
                    //Debug.Log("building lose an ally");
                }
            }
            else if(parent.tag == "Building")
            {
                if(parent.GetComponent<BuildingManager>().side != buildingManager.side)
                {
                    buildingManager.enemyOnSight.Remove(parent);
                }
                else
                {
                    buildingManager.allyOnSight.Remove(parent);
                }
            }
        }
        //Debug.Log(unitManager.gameObject + ": " + unitManager.enemyOnSight.Count);
    }
}
