using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerAI
{
    PlayerManager pm;
    GameObject enemyBase;
    GameObject barrack;

    float curTime = 0.0f;
    int curWave = 0;

    float[] levelUpTime = { 2 * 60, 4 * 60, 6 * 60, 8 * 60, 10 * 60, 15 * 60 };

    int[,] waves =
    {
        {5, 0, 0 },
        {5, 3, 0 },
        {5, 3, 2 },
        {6, 5, 3 },
        {7, 8, 10 }
    };

    List<GameObject> infantry = new List<GameObject>();
    List<GameObject> cavalry = new List<GameObject>();
    List<GameObject> archer = new List<GameObject>();
    public SimplePlayerAI(PlayerManager _pm, GameObject _barrack, GameObject _enemyBase)
    {
        pm = _pm;
        barrack = _barrack;
        enemyBase = _enemyBase;
        barrack.GetComponent<BuildingManager>().RallyPoint = new Vector3(pm.transform.position.x, 1.0071f, pm.transform.position.z);
    }

    public void Act(float LogicLen)
    {
        curTime += LogicLen;
        if(curWave < waves.Length && curTime >= levelUpTime[curWave + 1])
        {
            SendWave();
            curWave++;
            pm.autoIncomeRate += curWave * 20;
        }
        if (barrack != null && barrack.tag != "Dead")
        {
            if(infantry.Count < waves[curWave,0] && pm.gameSetting.UnitPrice[(int)pm.hero_t, (int)Unit.Infantry] <= pm.gold)
            {
                GameObject newUnit = barrack.GetComponent<BuildingManager>().SpawnUnit(pm, Unit.Infantry);
                infantry.Add(newUnit);
                pm.units.Add(newUnit);
                pm.gold -= pm.gameSetting.UnitPrice[(int)pm.hero_t, (int)Unit.Infantry];
            }
            if (cavalry.Count < waves[curWave, 1] && pm.gameSetting.UnitPrice[(int)pm.hero_t, (int)Unit.Cavalry] <= pm.gold)
            {
                GameObject newUnit = barrack.GetComponent<BuildingManager>().SpawnUnit(pm, Unit.Cavalry);
                cavalry.Add(newUnit);
                pm.units.Add(newUnit);
                pm.gold -= pm.gameSetting.UnitPrice[(int)pm.hero_t, (int)Unit.Cavalry];
            }
            if (archer.Count < waves[curWave, 2] && pm.gameSetting.UnitPrice[(int)pm.hero_t, (int)Unit.Archer] <= pm.gold)
            {
                GameObject newUnit = barrack.GetComponent<BuildingManager>().SpawnUnit(pm, Unit.Archer);
                archer.Add(newUnit);
                pm.units.Add(newUnit);
                pm.gold -= pm.gameSetting.UnitPrice[(int)pm.hero_t, (int)Unit.Archer];
            }
        }
        if(infantry.Count == waves[curWave, 0] && cavalry.Count == waves[curWave, 1] && archer.Count == waves[curWave, 2])
        {
            SendWave();
        }
    }
    void SendWave()
    {
        foreach (GameObject unit in infantry)
        {
            unit.GetComponent<UnitManager>().enemyOnSight.Add(enemyBase);
            unit.GetComponent<UnitManager>().target = enemyBase;
            unit.GetComponent<UnitManager>().autoSelectTarget = true;
            unit.GetComponent<UnitManager>().onGuard = true;
        }
        foreach (GameObject unit in cavalry)
        {
            unit.GetComponent<UnitManager>().enemyOnSight.Add(enemyBase);
            unit.GetComponent<UnitManager>().target = enemyBase;
            unit.GetComponent<UnitManager>().autoSelectTarget = true;
            unit.GetComponent<UnitManager>().onGuard = true;
        }
        foreach (GameObject unit in archer)
        {
            unit.GetComponent<UnitManager>().enemyOnSight.Add(enemyBase);
            unit.GetComponent<UnitManager>().target = enemyBase;
            unit.GetComponent<UnitManager>().autoSelectTarget = true;
            unit.GetComponent<UnitManager>().onGuard = true;
        }
        infantry.Clear();
        cavalry.Clear();
        archer.Clear();
    }
}
