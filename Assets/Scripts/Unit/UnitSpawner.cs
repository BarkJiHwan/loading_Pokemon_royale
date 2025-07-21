using System;
using System.Collections.Generic;
using UnityEngine;

public enum ESpawner
{
    PlayerSpawner, EnemySpawner
}

// 유닛 소환하는 스포너 스크립트
public class UnitSpawner : MonoBehaviour
{
    // 선택한 플레이어
    // [SerializeField] private Player _player;
    // 현재 필드에 생성 되어 있는 포켓몬들 딕셔너리
    private Dictionary<string, GameObject> _fieldDict;
    // 재사용 가능한 유닛 풀
    private Dictionary<EPokemonName, List<GameObject>> _unitPoolDict;
    // 유닛들 풀에서 꺼냈을 때 임시로 담아둘 리스트
    private List<GameObject> _tempList;

    public Dictionary<string, GameObject> FieldDict
    {
        get => _fieldDict;
    }

    public Dictionary<EPokemonName, List<GameObject>> UnitPoolDict
    {
        get => _unitPoolDict;
    }

    private void Awake()
    {
        _fieldDict = new Dictionary<string, GameObject>();
        _unitPoolDict = new Dictionary<EPokemonName, List<GameObject>>();
        _tempList = new List<GameObject>();
    }

    private void Start()
    {
        InitObjectPool();
        InitTower();
    }

    public void InitObjectPool()
    {
        // 카드 정보를 가지고 인스턴스를 만드는 방법?
        // - ScriptableObject 외에는 인스턴스화를 하지 않으면 아무것도 접근이 안된다.
        // - 생성자는 monoBehavior로 생성이 안 됨

        Card[] cardPool;
        if (gameObject.tag == "Player")
        {
            cardPool = SceneLoader.Instance.EquipCards;
        }
        else
        {
            cardPool = SceneLoader.Instance.EnemyCards;
        }

        String names = "";

        foreach (Card card in cardPool)
        {
            names += card.Name + ", ";
        }

        Debug.Log(gameObject.tag + " cardNames: " + names);

        foreach (Card card in cardPool)
        {
            EPokemonName pokemon = card.Pokemon;
            GameObject prefab = UnitPrefabLoader.GetPrefab(card.Pokemon);
            UnitDataObject unitData = UnitPrefabLoader.GetUnitDataObject(card.Pokemon);

            int maxSpawn = unitData.MaxSpawn;
            List<GameObject> tempUnitPool = new();

            for (int i = 0; i < maxSpawn; i++)
            {
                // 생성하고 태그 달아주기, 스포너 아래에 생성해주기
                GameObject instantiate = Instantiate(prefab, gameObject.transform);
                instantiate.tag = gameObject.tag;

                Unit unit = instantiate.GetComponent<Unit>();

                // 유닛의 유니크 아이디를 i번으로 설정
                unit.UniqueId = i;
                unit.gameObject.SetActive(false);

                unit.SetCard(card);
                unit.SetSpawner(this);
                // 인스턴스화 안하면 그냥 unit 프리팹 리스트만 담긴다.
                tempUnitPool.Add(instantiate);
            }

            //Debug.Log("pool : " + pokemon);
            //Debug.Log("pool : " + tempUnitPool.Count);
            _unitPoolDict.Add(pokemon, tempUnitPool);
        }
    }

    public void InitTower()
    {
        // Init PlayerTower
        if (gameObject.tag.Equals("Player"))
        {
            // 타워 3개 위치 초기화하고 꼽기 가운데 왕타워, 왼쪽 오른쪽 타워 순
            int count = 0;
            Dictionary<string, Vector3> playerTowerDict = new();
            playerTowerDict.Add("Castle", new Vector3(0, -2, 0));
            playerTowerDict.Add("LeftTower", new Vector3(-1.875f, -1.375f, 0));
            playerTowerDict.Add("RightTower", new Vector3(1.875f, -1.375f, 0));

            foreach (string key in playerTowerDict.Keys)
            {
                ETowerPokemonName towerPokemon;
                Vector3 playerTowerPos = playerTowerDict[key];
                Card card = SceneLoader.Instance.TowerCards[count];

                //Debug.Log(count);

                Enum.TryParse(card.Name, true, out towerPokemon);

                GameObject prefab = UnitPrefabLoader.GetPrefab(towerPokemon);
                GameObject instantiate = Instantiate(prefab, gameObject.transform);

                Unit unit = instantiate.GetComponent<Unit>();
                // 유닛의 유니크 아이디를 i번으로 설정
                unit.UniqueId = count;
                unit.SetCard(card);
                unit.SetSpawner(this);

                unit.MakeHealthUI();
                unit.HealthUI.SetUnit(unit.gameObject);
                unit.HpMove();

                instantiate.name = key;
                instantiate.tag = gameObject.tag;
                instantiate.transform.parent = gameObject.transform;
                instantiate.transform.position = playerTowerPos;

                count++;
                AddFieldDictionary(instantiate);
            }
        }

        // init EnemyTower
        if (gameObject.tag.Equals("Enemy"))
        {
            // 적 타워 3개 위치 초기화하고 꼽기 가운데 왕타워, 왼쪽 오른쪽 타워 순
            int count = 0;
            Dictionary<string, Vector3> enemyTowerDict = new();
            enemyTowerDict.Add("Castle", new Vector3(0, 3.25f, 0));
            enemyTowerDict.Add("LeftTower", new Vector3(-1.875f, 2.625f, 0));
            enemyTowerDict.Add("RightTower", new Vector3(1.875f, 2.625f, 0));

            foreach (string key in enemyTowerDict.Keys)
            {
                Vector3 enemyTowerPos = enemyTowerDict[key];

                Card card = SceneLoader.Instance.EnemyTowerCards[count++];
                ETowerPokemonName towerPokemon;
                Enum.TryParse(card.Name, true, out towerPokemon);

                GameObject prefab = UnitPrefabLoader.GetPrefab(towerPokemon);
                GameObject instantiate = Instantiate(prefab, gameObject.transform);

                Unit unit = instantiate.GetComponent<Unit>();
                // 유닛의 유니크 아이디를 i번으로 설정
                unit.UniqueId = count;
                unit.SetCard(card);
                unit.SetSpawner(this);

                unit.MakeHealthUI();
                unit.HealthUI.SetUnit(unit.gameObject);
                unit.HpMove();

                instantiate.name = key;
                instantiate.tag = gameObject.tag;
                instantiate.transform.parent = gameObject.transform;
                instantiate.transform.position = enemyTowerPos;
                AddFieldDictionary(instantiate);
            }
        }
    }

    public void AddFieldDictionary(GameObject gameObject)
    {
        Unit unit = gameObject.GetComponent<Unit>();
        if (_fieldDict.ContainsKey(unit.UniqueKey))
        {
            Debug.LogError("중복된 키값의 유닛이 있습니다!?");
        }
        _fieldDict.Add(unit.UniqueKey, gameObject);
    }

    public void RemoveFieldDictionary(Unit unit)
    {
        _fieldDict.Remove(unit.UniqueKey);
    }

    // 오브젝트 풀에서 유닛 불러오기
    private void GetUnactiveUnit(Card card, int quantity)
    {
        int count = 0;
        List<GameObject> pokemonPool = _unitPoolDict[card.Pokemon];
        // 풀보다 많은 수량 요청했을 경우, 이런케이스 있으면 안됨
        if (pokemonPool.Count < quantity)
        {
            Debug.LogError("포켓몬 풀 보다 많은 유닛을 생성할 수 없습니다");
            return;
        }

        // 비활성화 되었고, 현재 카운트가 수량보다 작다면 풀에서 꺼낸다.
        for (int i = 0; i < pokemonPool.Count; i++)
        {
            if (count >= quantity)
            {
                break;
            }
            GameObject gameObj = pokemonPool[i];
            if (gameObj.activeInHierarchy == false)
            {
                count++;
                _tempList.Add(gameObj);
            }
        }
    }

    // UnitQuantity 갯수만큼 한번에 소환할 유닛 리스트를 찾아서 반환한다.
    // 유닛의 Max Spawn 범위를 벗어나면? 빈 리스트 반환,
    private List<GameObject> GetUnits(Card card)
    {
        UnitDataObject unitData = UnitPrefabLoader.GetUnitDataObject(card.Pokemon);
        GetUnactiveUnit(card, unitData.UnitQuantity);

        // 스폰 갯수보다 작을경우 방어코드 -> 빈 리스트 반환
        if (_tempList.Count < unitData.UnitQuantity)
        {
            _tempList.Clear();
            return _tempList;
        }

        return _tempList;
    }

    // 유닛 실제 객체로 소환
    public void SpawnUnit(Card card, Vector3 pos)
    {
        List<GameObject> units = this.GetUnits(card);
        foreach (GameObject gameObj in units)
        {
            gameObj.SetActive(true);
            gameObj.GetComponent<Unit>().SetSpawner(this);
            gameObj.transform.position = new Vector3(pos.x, pos.y, 0);

            gameObj.GetComponent<Unit>().MakeHealthUI();
            gameObj.GetComponent<Unit> ().HealthUI.SetUnit(gameObj);
            gameObj.GetComponent<Unit>().HpMove();

            if (gameObj.tag == "Enemy")
            {
                if (SceneLoader.LevelValue == 0)
                {
                    //칠색조
                    gameObj.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f);
                }
                else
                {
                    Debug.Log("여긴 루기아");
                    //루기아
                    gameObj.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 1f, 1f);
                }
            }
            else
            {
                gameObj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            }
            AddFieldDictionary(gameObj);
        }

        // 소환 후 임시 리스트 클리어
        _tempList.Clear();
    }

    // 한마리 죽을때마다 풀로 반환
    public void ReturnUnit(Unit unit)
    {
        unit.gameObject.SetActive(false);
        RemoveFieldDictionary(unit);
    }

    // 스포너에서 살아있는 해당 포켓몬 종류가 몇마리인지?
    public int CountAlive(Card card)
    {
        int count = 0;
        List<GameObject> unitPool = _unitPoolDict[card.Pokemon];
        foreach (GameObject gameObject in unitPool)
        {
            if (gameObject.activeInHierarchy)
            {
                count++;
            }
        }
        return count;
    }

    // 카드를 뽑을수 있는지 기준?
    // 최대 스폰 갯수 = 필드 돌아다니는 유닛 갯수 + 카드 스폰시 생성되는 양
    public bool IsDrawable(Card card)
    {
        UnitDataObject unitData = UnitPrefabLoader.GetUnitDataObject(card.Pokemon);
        int alive = this.CountAlive(card);

        return unitData.MaxSpawn >= unitData.UnitQuantity + alive;
    }

    public static GameObject GetSpawner(ESpawner spawner)
    {
        return GameObject.Find(spawner.ToString());
    }

    public static GameObject GetEnemySpawner(string tag)
    {
        if (tag.Equals("Player"))
        {
            return GameObject.Find(ESpawner.EnemySpawner.ToString());
        }
        else
        {
            return GameObject.Find(ESpawner.PlayerSpawner.ToString());
        }
    }

    public GameObject GetLeftTower()
    {
        return transform.Find("LeftTower").gameObject;
    }

    public GameObject GetRightTower()
    {
        return transform.Find("RightTower").gameObject;
    }

    public GameObject GetCastle()
    {
        return transform.Find("Castle").gameObject;
    }
}
