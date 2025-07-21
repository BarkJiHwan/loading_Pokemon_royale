using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
    private CardLibrary _cardLibrary;
    private Card[] _equipCards;
    private Card[] _towerCards;
    private Card[] _enemyCards;
    private Card[] _enemyTowerCards;
    private static int _LevelValue = 0;

    public static int LevelValue
    {
        get => _LevelValue;
    }

    public static SceneLoader Instance => instance;
    public Card[] EquipCards => _equipCards;
    public Card[] TowerCards => _towerCards;
    public Card[] EnemyCards => _enemyCards;
    public Card[] EnemyTowerCards => _enemyTowerCards;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LevelUp()
    {
        _LevelValue++;
    }
    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadStageOne()
    {
        GetEquipCards();
        SceneManager.LoadScene("FireBattleStage");
    }

    public void LoadStageTwe()
    {
        GetEquipCards();
        SceneManager.LoadScene("WaterBattleStage");
    }

    public void LoadStageThree()
    {
        GetEquipCards();
        SceneManager.LoadScene("GrassBattleStage");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Content 찾아오기
        if (scene.name == "MainScene")
        {
            Debug.Log("카드 라이브러리 찾기 : " + GameObject.Find("CardLibrary"));
            GameObject tempObj = GameObject.Find("CardLibrary");
            _cardLibrary = tempObj.GetComponent<CardLibrary>();
        }

        if (scene.name.Contains("BattleStage"))
        {
            Debug.Log("전투 씬 입장 : " + scene.name);
        }

        if (_equipCards != null && _equipCards[0] != null)
        {
            Debug.Log("0번 장착 : " + _equipCards[0]?.Name);
        }
        else
        {
            GetEquipCards();
        }

        Debug.Log("나와주세요~");
    }

    /* 씬 로드시 카드 장착해주는 메서드들 */
    private void GetEquipCards()
    {
        // 테스트 코드용임
        if (_cardLibrary == null && _equipCards == null)
        {
            _equipCards = MakeEquipCards();
            _towerCards = MakeTowerCards(ETowerPokemonName.뮤츠);
            _enemyCards = MakeEquipCards();
            _enemyTowerCards = MakeTowerCards(ETowerPokemonName.루기아);
            return;
        }

        _equipCards = _cardLibrary.EquipCards;
        _towerCards = _cardLibrary.GetTowerCard();
        _enemyCards = _cardLibrary.EnemyEquipCards;
        _enemyTowerCards = _cardLibrary.EnemyTowerEquipCards;

        if (_cardLibrary.EquipCards == null || _cardLibrary.EquipCards[0] == null)
        {
            Debug.Log("장착 카드가 없습니다");
            _equipCards = MakeEquipCards();
        }

        if (_cardLibrary.TowerCards == null || _cardLibrary.TowerCards[0] == null)
        {
            Debug.Log("장착 타워 카드가 없습니다");
            _towerCards = MakeTowerCards(ETowerPokemonName.뮤츠);
        }

        if (_cardLibrary.EnemyEquipCards == null || _cardLibrary.EnemyEquipCards[0] == null)
        {
            Debug.Log("적의 장착 카드가 없습니다");
            _enemyCards = MakeEquipCards();
        }

        if (_cardLibrary.EnemyTowerEquipCards == null || _cardLibrary.EnemyTowerEquipCards[0] == null)
        {
            Debug.Log("적의 장착 타워 카드가 없습니다");
            _towerCards = MakeTowerCards(ETowerPokemonName.루기아);
        }
    }

    // 완전 임시용입니당, 디버깅용 핸드 만들어주기
    public static Card MakeTempCards(EPokemonName pokeName)
    {
        GameObject gobj = Resources.Load<GameObject>("Prefabs/CardPrefabs/" + pokeName);
        CardDataObject cdObj = Resources.Load<CardDataObject>("ScriptableObject/Card/" + pokeName);
        return new Card(cdObj.EUnitType, cdObj.Type, cdObj.Cost, cdObj.Pokemon, gobj.GetComponent<Image>());
    }

    // 완전 임시용입니당, 디버깅용 핸드 만들어주기
    public static Card MakeTempTowerCards(ETowerPokemonName towerPokemonName)
    {
        GameObject gobj = Resources.Load<GameObject>("Prefabs/CardPrefabs/" + towerPokemonName);
        CardDataObject cdObj = Resources.Load<CardDataObject>("ScriptableObject/Card/" + towerPokemonName);
        return new Card(cdObj.Type, towerPokemonName, gobj.GetComponent<Image>());
    }

    public static Card[] MakeEquipCards()
    {
        Card[] cards = new Card[12];
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = MakeTempCards((EPokemonName)i);
        }
        return cards;
    }

    public static Card[] MakeTowerCards(ETowerPokemonName towerPokemonName)
    {
        Card[] cards = new Card[3];
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = MakeTempTowerCards(towerPokemonName);
        }
        return cards;
    }
}
