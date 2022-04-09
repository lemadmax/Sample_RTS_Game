using UnityEngine;

/*********************************************************
 *
 * ---------------------- GameSetting --------------------
 * Set prefabs for in-game instantiation.
 *
 *********************************************************/
public class GameSetting : MonoBehaviour
{
    public int PlayerCount;

    [SerializeField] private GameObject MenInfantry;
    [SerializeField] private GameObject UndeadInfantry;
    [SerializeField] private GameObject OrcInfantry;

    [SerializeField] private GameObject OrcArcher;

    [SerializeField] private GameObject OrcCavalry;

    [SerializeField] private GameObject MenBarrack;
    [SerializeField] private GameObject MenDefense;
    [SerializeField] private GameObject MenMage;
    [SerializeField] private GameObject UndeadBarrack;
    [SerializeField] private GameObject OrcBarrack;
    [SerializeField] private GameObject OrcDefense;

    public const int heroCount = 4;
    public const int unitCount = 4;
    public const int buildingCount = 3;

    public GameObject[] heroPrefabs = new GameObject[heroCount];
    public GameObject[,] UnitPrefabs = new GameObject[heroCount, unitCount];
    public GameObject[,] BuildingPrefabs = new GameObject[3, buildingCount];

    public int[,] UnitPrice = new int[heroCount, unitCount];
    public int[,] BuildingPrice = new int[heroCount, buildingCount];
    public float bountyRate = 0.3f;

    void Start()
    {
        UnitPrefabs[(int)Hero.Ashe, (int)Unit.Infantry] = MenInfantry;
        UnitPrice[(int)Hero.Ashe, (int)Unit.Infantry] = 100;

        UnitPrefabs[(int)Hero.Nature, (int)Unit.Infantry] = UndeadInfantry;
        UnitPrice[(int)Hero.Nature, (int)Unit.Infantry] = 100;

        UnitPrefabs[(int)Hero.OrcShaman, (int)Unit.Infantry] = OrcInfantry;
        UnitPrice[(int)Hero.OrcShaman, (int)Unit.Infantry] = 100;

        UnitPrefabs[(int)Hero.OrcShaman, (int)Unit.Cavalry] = OrcCavalry;
        UnitPrice[(int)Hero.OrcShaman, (int)Unit.Cavalry] = 500;

        UnitPrefabs[(int)Hero.OrcShaman, (int)Unit.Archer] = OrcArcher;
        UnitPrice[(int)Hero.OrcShaman, (int)Unit.Archer] = 200;

        BuildingPrefabs[1, (int)Building.Barracks] = MenBarrack;
        BuildingPrice[1, (int)Building.Barracks] = 1000;

        BuildingPrefabs[1, (int)Building.Defense] = MenDefense;
        BuildingPrice[1, (int)Building.Defense] = 1000;

        BuildingPrefabs[1, (int)Building.Mage] = MenMage;
        BuildingPrice[1, (int)Building.Mage] = 1000;

        BuildingPrefabs[0, (int)Building.Barracks] = UndeadBarrack;
        BuildingPrice[0, (int)Building.Barracks] = 1000;

        BuildingPrefabs[2, (int)Building.Barracks] = OrcBarrack;
        BuildingPrice[2, (int)Building.Barracks] = 1000;

        BuildingPrefabs[2, (int)Building.Defense] = OrcDefense;
        BuildingPrice[2, (int)Building.Defense] = 1000;

    }
}
