using System.Collections.Generic;
using UnityEngine;
/*********************************************************
 *
 * -------------------- CrowdArrange --------------------
 * This class handles the crowd controll (when the player
 * give commands to a group of units).
 *
 *********************************************************/
public class CrowdArrange : MonoBehaviour
{
    [SerializeField] private GameSetting gameSetting;

    // Get the object array of the units that are following the hero.
    public GameObject[] GetFollowingUnits(List<GameObject> units)
    {
        List<GameObject> followingUnits = new List<GameObject>();
        foreach (GameObject unit in units)
        {
            if (unit.GetComponent<UnitManager>().followHero) followingUnits.Add(unit);
        }
        GameObject[] unitArray = new GameObject[followingUnits.Count];
        int index = 0;
        foreach (GameObject unit in followingUnits)
        {
            unitArray[index++] = unit;
        }
        return unitArray;
    }

    // Get the dictionary that map a unit's index in the array to the assigned target position.
    public Dictionary<int, Vector3> GetTargetPos(GameObject hero, List<GameObject> units, Vector3 target)
    {
        return GetTargetPos(hero, GetFollowingUnits(units), target);
    }
    public Dictionary<int, Vector3> GetTargetPos(GameObject hero, GameObject[] units, Vector3 target)
    {
        Dictionary<int, Vector3> res = new Dictionary<int, Vector3>();

        GameObject[,] typedObjects = new GameObject[GameSetting.unitCount - 1, units.Length];
        int[] indices = new int[GameSetting.unitCount - 1];
        for(int i = 0; i < units.Length; i++)
        {
            int type = (int)units[i].GetComponent<UnitManager>().type;
            typedObjects[type, indices[type]++] = units[i];
        }
        Vector3 currentPos;
        if(hero != null)
        {
            currentPos = hero.transform.position;
        }
        else
        {
            currentPos = units[0].transform.position;
        }
        Vector3 direction = Vector3.Normalize(target - currentPos);
        Vector3 left = new Vector3(direction.z, direction.y, -direction.x);
        float offset = 0.0f;
        if (hero != null) offset = 5.0f;
        for(int i = 0; i < GameSetting.unitCount - 1; i++)
        {
            if (indices[i] == 0) continue;
            int l = (int)Mathf.Sqrt(indices[i] / 2);
            if (indices[i] > l * l) l++;
            int w = l * 2;
            //Debug.Log(w + ", " + l);
            Vector3 startPoint;
            if(w % 2 == 0)
            {
                startPoint = target - direction * offset - (5.0f * (w / 2) - 2.5f) * left;
            }
            else
            {
                startPoint = target - direction * offset - 5.0f * (w / 2) * left;
            }
            for(int j = 0; j < indices[i]; j++)
            {
                int x = j / w;
                int y = j % w;
                Vector3 pos = startPoint + y * 5.0f * left - x * 5.0f * direction;
                res.Add(typedObjects[i, j].GetInstanceID(), pos);
            }
            offset += 5.0f * l;
        }

        return res;
    }

}
