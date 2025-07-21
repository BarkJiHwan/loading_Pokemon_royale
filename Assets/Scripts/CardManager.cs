using System.Collections.Generic;
using UnityEngine;
public class CardManager : MonoBehaviour
{
    //인스턴스화
    private static CardManager instance;

    //현재 가지고 있는 카드 덱
    [SerializeField] List<Card> PlayerCardDeck;

    //현재 장착중인 유닛 덱
    [SerializeField] Card[] UseCard; // <=여기다가 넣어주면됨.장착중인것

    //손에 들고 있는 유닛
    [SerializeField] List<Card> HandCard;

    [SerializeField] List<Card> EnemyCard;

    //손에 들고 있는 유닛 최대치
    int _maxCardCount;

    //장착 중인 유닛 리스트화
    List<Card> CardBuffer;

    //현재 가지고 있는 타워 덱
    List<Card> PlayerTowerDeck; //<= 여기다가 타워 넣어주면됨. 수정해도 된다함.

    //장작 중인 타워
    Card[] UseTower;

    [SerializeField] GameObject CardPrefab;

    [SerializeField] Transform CardParent;

    [SerializeField] Transform NextCardParent;

    [SerializeField] GameObject NextCardPrefab;

    //다음카드
    Card nextCard;

    //다음 오브젝트
    GameObject nextObject;

    private UnitSpawner _unitSpawner;

    private UnitSpawner _enemyUnitSpawner;

    public PokeBlockCount _pokeBlockCount;

    //적이 고르려고 선택한 카드
    Card EnemySeletCard;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        PlayerCardDeck = new List<Card>();
        PlayerTowerDeck = new List<Card>();
        HandCard = new List<Card>();
        EnemyCard = new List<Card>();
        _maxCardCount = 5;
        _pokeBlockCount = GameObject.Find("Canvas").transform.GetChild(0).GetChild(4).GetComponent<PokeBlockCount>();
    }

    public static CardManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public UnitSpawner Spawner
    {
        get => _unitSpawner;
        set => _unitSpawner = value;
    }

    public UnitSpawner EnemySpawner
    {
        get => _enemyUnitSpawner;
        set => _enemyUnitSpawner = value;
    }

    void Start()
    {
        // 씬 로더에서 가져오기
        GetPlayerCardsFromScene();
        GetEnemyCardsFromScene();

        // 스포너 연동하기
        GetPlayerSpawner();
        GetEnemySpawner();
        SetCardBuffer();
        CardBufferCardShuffle();

        SelectEnemyCard();
        _pokeBlockCount.SetCardManager(this);
    }

    void Update()
    {
        // 초기 핸드 세팅, 안드로이드 버전에서 깨짐
        if (GameManager.Instance.IsGameStart == false && HandCard.Count < _maxCardCount)
        {
            DrawCard();
        }
    }

    //장착 중인 카드 리스트화 시키고 초기화 시키는 함수
    public void SetCardBuffer()
    {
        CardBuffer = new List<Card>();
        for(int i = 0; i < UseCard.Length; i++)
        {
            Card card = UseCard[i];
            CardBuffer.Add(card);
        }
    }

    //장착된 유닛 셔플
    public void CardBufferCardShuffle()
    {
        for (int i = 0; i < CardBuffer.Count; i++)
        {
            int ran = Random.Range(0, CardBuffer.Count);
            Card tempCard = CardBuffer[ran];
            CardBuffer[ran] = CardBuffer[i];
            CardBuffer[i] = tempCard;
        }
    }

    //뽑기 가능한 유닛 드로우하면 덱에서 제거하는 함수
    public Card PopCard()
    {
        //int ranNum = Random.Range(0, CardBuffer.Count);
        Card card = CardBuffer[0];
        CardBuffer.RemoveAt(0);
        return card;
    }

    //카드 드로우
    public void DrawCard()
    {
        int maxLoopDraw = 100;
        Card tempCard = PopCard();

        // 뽑을 수 있는 카드 뽑을 때 까지 다시 카드 뽑기
        // 무한루프 방지 조건 설정
        while (!_unitSpawner.IsDrawable(tempCard) && maxLoopDraw > 0)
        {
            CardBuffer.Add(tempCard);
            tempCard = PopCard();
            maxLoopDraw--;
        }

        // 100번 루프 끝나면 디버거에 에러로 표시
        if (maxLoopDraw == 0)
        {
            Debug.LogError("카드를 뽑을 수 없는 상황입니다");
            return;
        }

        HandCard.Add(tempCard);
        var cardObject = Instantiate(CardPrefab, CardParent);
        var card = cardObject.GetComponent<BattleCardInfo>();
        card.Setup(tempCard);
        Destroy(nextObject);
        CardBufferCardShuffle();
        ShowNextCard();
    }

    //다음 카드 보여줌
    public void ShowNextCard()
    {
        nextCard = CardBuffer[0];
        nextObject = Instantiate(NextCardPrefab, NextCardParent);
        var NextCard = nextObject.GetComponent<NextCardImage>();
        NextCard.Setup(nextCard);
    }

    //유닛 내면 손패 제거
    //드래그 앤 드롭
    public void ReMoveHand(GameObject gameObject)
    {
        Card tempCard = gameObject.GetComponent<BattleCardInfo>().Card;
        HandCard.Remove(tempCard);
        CardBuffer.Add(tempCard);
        DrawCard();
    }

    //드로우 할 수 있는 카드 목록 작성
    public void SetCard()
    {
        for (int i = 0; i < UseCard.Length; i++)
        {
            if (UseCard[i].IsHandCard == false && UseCard[i].IsBattleCardUse == true)
            {
                //canDrawCards.Add(UseCard[i]);
            }
        }
    }

    /// <summary>
    /// 현재 가지고 있는 카드 덱에 카드 추가
    /// </summary>
    /// <param name="card">추가할 카드 넣어주세요</param>
    public void AddPlayerCardList(Card card)
    {
        card.IsAllCardGet = true;//카드 클래스에서 수정한거 때문에 에러나서 수정해드렸습니다.
        card.IsBattleCardUse = true;
        card.IsUserCardEquip = false;
        PlayerCardDeck.Add(card);
    }

    public void Delivery(List<Card> list)
    {
        UseCard = list.ToArray();
    }

    public void AddCanDrawCard(Card card)
    {
        //canDrawCards.Add(card);
    }

    public void RemoveHandCard(int i)
    {
        HandCard[i] = null;
    }

    /// <summary>
    /// 카드 장착하기
    /// </summary>
    /// <param name="CardNum">장착할 카드 넘버</param>
    /// <param name="PutNum">장착할 칸</param>
    public void AddUseCard(int CardNum, int PutNum)
    {
        if(PlayerCardDeck[CardNum].IsAllCardGet == true)
        {
            if (PlayerCardDeck[CardNum].IsUserCardEquip == false)
            {
                if (UseCard[PutNum] == null)
                {
                    UseCard[PutNum] = PlayerCardDeck[CardNum];
                    PlayerCardDeck[CardNum].IsUserCardEquip = true;
                    PlayerCardDeck.Remove(PlayerCardDeck[CardNum]);
                }
                else
                {
                    RemoveUseCard(PutNum);
                    UseCard[PutNum] = PlayerCardDeck[CardNum];
                    PlayerCardDeck[CardNum].IsUserCardEquip = true;
                    PlayerCardDeck.Remove(PlayerCardDeck[CardNum]);
                }
            }
        }
    }

    /// <summary>
    /// //장착된 유닛 해체
    /// </summary>
    /// <param name="CardNum">해체될 카드가 있는 칸</param>
    public void RemoveUseCard(int CardNum)
    {
        if (UseCard[CardNum] != null)
        {
            UseCard[CardNum].IsUserCardEquip = false;
            PlayerCardDeck.Add(UseCard[CardNum]);
            UseCard[CardNum] = null;
        }
    }

    /// <summary>
    /// //타워덱에 추가
    /// </summary>
    /// <param name="card">추가할 카드</param>
    public void AddPlayerTowerDeck(Card card)
    {
        card.IsAllCardGet = true;
        card.IsUserCardEquip = false;
        PlayerTowerDeck.Add(card);
    }

    /// <summary>
    /// //타워 장착하기
    /// </summary>
    /// <param name="towerNum">장착할 타워 넘버</param>
    /// <param name="putNum">장착할 칸</param>
    public void AddUseTower(int towerNum, int putNum)
    {
        if (PlayerTowerDeck[towerNum].IsAllCardGet == true)
        {
            if (PlayerTowerDeck[towerNum].IsUserCardEquip ==false)
            {
                if (UseTower[putNum] == null)
                {
                    UseTower[putNum] = PlayerTowerDeck[towerNum];
                    PlayerTowerDeck[towerNum].IsUserCardEquip = true;
                    PlayerTowerDeck.Remove(PlayerTowerDeck[towerNum]);
                }
                else
                {
                    RemoveUseTower(putNum);
                    UseTower[putNum] = PlayerTowerDeck[towerNum];
                    PlayerTowerDeck[towerNum].IsUserCardEquip = true;
                    PlayerTowerDeck.Remove(PlayerTowerDeck[towerNum]);
                }
            }
        }
    }

    /// <summary>
    /// //타워 장착 카드 제거
    /// </summary>
    /// <param name="towerNum">제거할 타워 칸</param>
    public void RemoveUseTower(int towerNum)
    {
        if (UseTower[towerNum] != null)
        {
            UseTower[towerNum].IsUserCardEquip = false;
            PlayerTowerDeck.Add(UseTower[towerNum]);
            UseTower[towerNum] = null;
        }
    }

    //인게임 덱정보 보기
    public Card[] GetUseCard()
    {
        return UseCard;
    }

    //타워 덱정보 보기
    public Card[] GetTowerCard()
    {
        return UseTower;
    }

    // 씬에서 장착 카드 받아서 카드 매니저에 세팅
    public void GetPlayerCardsFromScene()
    {
        SceneLoader loader = SceneLoader.Instance;
        UseCard = loader.EquipCards;

        // 씬에서 직접 실행할때 디버깅용 초기화
        if (UseCard == null || UseCard[0] == null)
        {
            Debug.Log("장착 카드가 없습니다 확인해주세요");
            UseCard = SceneLoader.MakeEquipCards();
        }
    }

    // 플레이어 스포너 설정
    public void GetPlayerSpawner()
    {
        GameObject pSpawnerObj = UnitSpawner.GetSpawner(ESpawner.PlayerSpawner);
        Spawner = pSpawnerObj.GetComponent<UnitSpawner>();
    }

    //플레이어가 카드 내면 코스트 감소
    public void PlayerCostMin(Card card)
    {
        _pokeBlockCount.PokeBlockCost -= card.Cost;
    }

    //씬에서 적 장착 카드 받아서 카드 매니저에 세팅
    public void GetEnemyCardsFromScene()
    {
        SceneLoader loader = SceneLoader.Instance;
        for(int i = 0; i < loader.EnemyCards.Length; i++)
        {
            EnemyCard.Add(loader.EnemyCards[i]);
        }
    }

    //적 스포너 설정
    public void GetEnemySpawner()
    {
        UnitSpawner spawner = GameObject.Find("EnemySpawner").GetComponent<UnitSpawner>();
        EnemySpawner = spawner;
    }

    //적 카드 중에 랜덤 소환
    public void SpawnEnemyUnit(Card card)
    {
        int max = 50;
        while (true && max > 0)
        {
            if (!EnemySpawner.IsDrawable(card) || card.Cost > _pokeBlockCount.StartMaxpokeBlockCost)
            {
                Debug.Log(card.Name + "다시");
                SelectEnemyCard();
            }
            //Debug.Log(card.Name);
            if (card.Cost == _pokeBlockCount.EnemyPokeBlockCost)
            {
                GameObject temp = GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).gameObject;
                Vector2 rect = GetRandomEnemyPos(temp);
                Vector2 transform = Camera.main.ScreenToWorldPoint(rect);

                EnemySpawner.SpawnUnit(card, transform);
                _pokeBlockCount.EnemyPokeBlockMinusCost(card);
                SelectEnemyCard();
                break;
            }
        max--;
        }
        if (max == 0)
        {
            return;
        }
    }

    public void SelectEnemyCard()
    {
        //if(EnemyCard != null && )
        int ran = Random.Range(0, EnemyCard.Count);
        EnemySeletCard = EnemyCard[ran];
    }

    public Vector2 GetRandomEnemyPos(GameObject gameObject)
    {
        float minWidth = gameObject.GetComponent<RectTransform>().position.x + gameObject.GetComponent<RectTransform>().rect.min.x;
        float maxWidth = gameObject.GetComponent<RectTransform>().position.x + gameObject.GetComponent<RectTransform>().rect.max.x;
        float minHeight = gameObject.GetComponent<RectTransform>().position.y + gameObject.GetComponent<RectTransform>().rect.min.y;
        float maxHeight = gameObject.GetComponent<RectTransform>().position.y + gameObject.GetComponent<RectTransform>().rect.max.y;

        float ranX = Random.Range(minWidth, maxWidth);
        float ranY = Random.Range(minHeight, maxHeight);

        Vector2 ran = new Vector2(ranX, ranY);

        return ran;
    }

    public void NoticePokeBlock()
    {
        SpawnEnemyUnit(EnemySeletCard);
    }
}
