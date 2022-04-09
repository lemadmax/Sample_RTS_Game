using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDict
{
    public SelectType sType;
    public GameObject building;
    public Dictionary<int, GameObject> selectDic = new Dictionary<int, GameObject>();

    public bool IsEmpty()
    {
        return selectDic.Count == 0 && building == null;
    }
    public void Select(GameObject ob, SelectType type)
    {
        if (type == SelectType.Unit && ob.GetComponent<UnitManager>().IsAlive())
        {
            if(ob.GetComponent<UnitManager>().type != Unit.Hero)
            {
                selectDic.Add(ob.GetInstanceID(), ob);
                ob.GetComponent<UnitManager>().Select();
            }
        }
        if(type == SelectType.Building && ob.GetComponent<BuildingManager>().IsAlive)
        {
            building = ob;
            ob.GetComponent<BuildingManager>().Select();
        }
        sType = type;
    }

    public void Deselect(GameObject ob)
    {
        if (ob == null) return;
        if (sType == SelectType.Unit)
        {
            selectDic.Remove(ob.GetInstanceID());
            ob.GetComponent<UnitManager>().UnSelect();
        }
        else if (sType == SelectType.Building)
        {
            building = null;
            ob.GetComponent<BuildingManager>().UnSelect();
        }
    }

    public void DeselectAll()
    {
        foreach(var ob in selectDic)
        {
            ob.Value.GetComponent<UnitManager>().UnSelect();
        }
        if(building != null)
        {
            building.GetComponent<BuildingManager>().UnSelect();
            building = null;
        }
        selectDic.Clear();
    }

    public GameObject[] GetUnitArray()
    {
        GameObject[] units = new GameObject[selectDic.Count];
        int index = 0;
        foreach (KeyValuePair<int, GameObject> pair in selectDic)
        {
            units[index++] = pair.Value;
        }
        return units;
    }
}
