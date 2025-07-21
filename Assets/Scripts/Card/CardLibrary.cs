using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum EPokemonName
{
    이상해씨, 파이리, 꼬부기, 구구, 버터플, 어니부기, 거북왕,
    푸린, 이상해풀, 이상해꽃, 캐터피, 단데기, 푸푸린, 푸크린,
    리자드, 리자몽, 먹고자, 잠만보, 피죤, 피죤투, 파이어, 썬더,
    프리져, 레큐쟈, end
}
public enum ETowerPokemonName { 뮤츠, 루기아, 칠색조, end } //예시 리스트


public class CardLibrary : MonoBehaviour
{
    [SerializeField]
    Transform TowerDeckImage;//타워 덱 표시위치

    [SerializeField]
    Transform AcquisitionDeck;//흭득한 덱 표시위치

    [SerializeField]
    Transform UnacquiredDeck; // 미흭득 덱 표시위치

    [SerializeField]
    Transform DeckImage;//장착중 덱 표시 위치

    [SerializeField]
    public GameObject SelectCard;//선택된 카드 테두리 프리팹

    [SerializeField]
    public GameObject EmptyCard;//빈카드 프리팹

    [SerializeField]
    GameObject SelectZone;//장착시 표기해줄 프리팹


    //유닛
    List<GameObject> cardLibraryAllPrefabs; // 카드들 프리팹들
    List<CardDataObject> _cardDataObjects;//카드들 데이터 프리팹불러올예정

    //타워
    List<GameObject> TowercardLibraryPrefabs; // 카드들 프리팹들
    List<CardDataObject> _ToewerDataObjects;//카드들 데이터 프리팹불러올예정

    private List<Card> cardLibrary;//전체 카드 //대충 32마리
    private List<Card> getCards;//가지고있는 카드

    private static Card[] equipCards;//현재 장착중 카드 12개
    private static Card[] equipTowerCards;//현재 장착중 타워카드 3개- 1개로 3개 사용

    private List<Card>[] tempEnemyCardDeck;
    private Card[] enemyEquipCards; //적이 현재 가지고있을 카드
    private Card[] enemyTowerEquipCards; //적이 현재 가지고있을 타워 카드

    Card[] towerCards = new Card[3];//전체 타워카드 => 플레이어는 활성화 된애를 선택해서 장착할수 있게 할 예정.

    public Card[] EnemyEquipCards
    {
        get => enemyEquipCards;
    }
    public Card[] EnemyTowerEquipCards
    {
        get => enemyTowerEquipCards;
    }

    private int currentStage;



    public List<Card> GetCards
    {
        get => getCards;
    }

    public List<Card> GetCardLibrary
    {
        get => cardLibrary;
    }

    public Card[] AllTowerCard
    {
        get { return towerCards; }
    }

    //전체 타워 카드 프리팹
    GameObject[] towerPrefabs;

    /// <summary>
    /// 카드 정렬시 코스트 기준으로 할것인지 여부. false이면 타입별로 정렬
    /// </summary>
    bool isCostSort = false;

    /// <summary>
    /// 덱뷰 상태 확인용
    /// </summary>
    public bool isDeckMode = false;

    public Card[] EquipCards
    {
        get => equipCards;
    }

    public Card[] TowerCards
    {
        get => towerCards;
    }
    /// <summary>
    /// 현재 선택된 카드 제외하고 모두 다 선택해제
    /// </summary>
    /// <param name="card">선택할 카드</param>
    public void SelectClear(Card card)
    {
        for (int i = 0; i < equipCards.Length; i++)
        {
            if (equipCards[i] != null)
            {

                if (equipCards[i] != card)
                {
                    equipCards[i].IsSelecte = false;
                }
                else
                {
                    equipCards[i].IsSelecte = true;
                }
            }
        }

        for (int i = 0; i < towerCards.Length; i++)
        {
            if (towerCards[i] != card)
            {
                towerCards[i].IsSelecte = false;
            }
            else
            {
                towerCards[i].IsSelecte = true;
            }
        }
    }

    /// <summary>
    /// 카드를 흭득한 카드로 바꿔줄 메서드
    /// </summary>
    /// <param name="name">선택한 카드</param>
    public void GetCard(EPokemonName name)
    {
        for (int i = 0; i < cardLibrary.Count; i++)
        {
            if (cardLibrary[i].Name == name.ToString())
            {
                cardLibrary[i].IsAllCardGet = true;
                cardLibrary[i].CardImage.color = new Color(1f, 1f, 1f);
                getCards.Add(cardLibrary[i]);

                AcquisitionDeckAddImage(cardLibrary[i].Pokemon);

            }
        }
    }

    //적 카드 스테이지별 장착
    public void EnemyStageCardSeting(int stageIndex)
    {

        for (int i = 0; i < enemyEquipCards.Length; i++)
        {
            enemyEquipCards[i] = tempEnemyCardDeck[stageIndex][i];
        }

        //for (int i = 0; i < enemyEquipCards.Length; i++)
        //{
        //    Debug.Log("장착값 : " + enemyEquipCards[i].Name);
        //}
        //Debug.Log("적 : " + EnemyTowerEquipCards[0].Name);
        //Debug.Log("나 : " + equipTowerCards[0].Name);
    }

    //적 카드 세팅
    public void EnemyEquipCard(int stageIndex, EPokemonName name)
    {

        tempEnemyCardDeck[stageIndex].Add(cardLibrary[(int)name]);

        //for (int i = 0; i < tempEnemyCardDeck[stageIndex].Count; i++)
        //{
        //    Debug.Log("장착값 세팅: " + tempEnemyCardDeck[stageIndex][i].Name);
        //}
    }
    //적 타워 장착 - 한개로 3개다 설치
    public void EnemyEquipTowerCard(ETowerPokemonName name)
    {
        //3개 다 통일
        for (int i = 0; i < 3; i++)
        {
            EnemyTowerEquipCards[i] = towerCards[(int)name];
        }
    }

    //카드 장착(장착 OBJ도 반환)
    public GameObject EquipCard(EPokemonName name)
    {
        for (int i = 0; i < equipCards.Length; i++)
        {
            if (equipCards[i] == null)
            {
                equipCards[i] = cardLibrary[(int)name];
                //Debug.Log(equipCards[i].Name);
                var eqcd = Instantiate(cardLibraryAllPrefabs[(int)name], DeckImage.GetComponent<GridLayoutGroup>().transform);
                eqcd.gameObject.name = name.ToString();
                var cd = eqcd.GetComponent<CardImage>().Card = equipCards[i];


                cd.IsUserCardEquip = true;
                return eqcd;
            }
        }

        return null;
    }


    /// <summary>
    /// 장착공간 여부 확인메서드 있으면 true
    /// </summary>
    /// <returns> 장착공간 여부 확인메서드 있으면 true 없으면false</returns>
    public bool EquipCardEmpty()
    {

        foreach (var card in equipCards)
        {
            if (card == null)
            {
                return true;
            }
        }
        return false;
    }


    public void EquipCardCheck()
    {
        var temp = AcquisitionDeck.gameObject.GetComponentsInChildren<CardImage>();

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].Card.IsUserCardEquip == true)
            {
                if (temp[i].transform.childCount < 1)
                {
                    Instantiate(SelectZone, temp[i].transform);
                }
            }
            else
            {
                foreach (Transform child in temp[i].transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }


        var tempTower = TowerDeckImage.gameObject.GetComponentsInChildren<CardImage>();

        for (int i = 0; i < tempTower.Length; i++)
        {
            if (tempTower[i].Card.IsUserCardEquip == true)
            {
                //Debug.Log("여기 오긴했는가.");
                if (tempTower[i].transform.childCount < 1)
                {
                    Instantiate(SelectZone, tempTower[i].transform);
                }
            }
            else
            {
                foreach (Transform child in tempTower[i].transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    //흭득 카드 이미지 컬렉션 화면에 뿌려줌
    public void AcquisitionDeckAddImage(EPokemonName name)
    {
        //미흭득카드에서 흭득 카드로 변경될때 원상복구
        var targetCard = Instantiate(cardLibraryAllPrefabs[(int)name], AcquisitionDeck);
        targetCard.gameObject.name = name.ToString();
        targetCard.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        var tgCd = targetCard.GetComponent<CardImage>().Card = cardLibrary[(int)name];



    }

    //미흭득카드 이미지 컬렉션 화면에 뿌려줌
    public void UnacquiredDeckAddImage(EPokemonName name)
    {
        //미흭득 카드로 분류됬을때 어둡게 변경
        var targetCard = Instantiate(cardLibraryAllPrefabs[(int)name], UnacquiredDeck);
        targetCard.gameObject.name = name.ToString();
        targetCard.GetComponent<CardImage>().Card = cardLibrary[(int)name];
        targetCard.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
    }

    /// <summary>
    /// 코스트 별로 정렬 메소드
    /// </summary>
    public void CostSort(List<Card> cards)
    {
        List<Card> tempCards = new List<Card>();
        //int MaxCost = 15;// 15코스트는 전설 카드(트리거) => 사용 예정이였으나... 모르겠음.
        isCostSort = true;

        //병합 정렬 구현

        List<Card> temp = new List<Card>();
        for (int i = 0; i < cards.Count; i++)
        {
            temp.Add(cards[i]);
        }

        MergeSort(cards, temp, 0, cards.Count);

    }

    void MergeSort(List<Card> cards, List<Card> temp, int start, int end)
    {
        int right = end;
        int left = start;

        if (left < right)
        {
            int mid = (start + right) / 2;
            MergeSort(cards, temp, start, mid);
            MergeSort(cards, temp, mid + 1, right);
            Merge(cards, temp, left, mid, right);
        }

    }

    void Merge(List<Card> cards, List<Card> temp, int left, int mid, int right)
    {
        int i = left;
        int j = mid + 1;
        int k = left;
        int l;

        while (i <= mid && j <= right)
        {
            if (isCostSort == true)
            {
                if (cards[i].Cost < temp[j].Cost)
                {
                    temp[k] = cards[i];
                    k++;
                    i++;
                }
                else
                {
                    temp[k] = cards[j];
                    k++;
                    j++;
                }
            }
            else
            {
                if (cards[i].Type < temp[j].Type)
                {
                    temp[k] = cards[i];
                    k++;
                    i++;
                }
                else
                {
                    temp[k] = cards[j];
                    k++;
                    j++;
                }
            }

        }

        if (i > mid)
        {
            for (l = j; l <= right; l++)
            {
                temp[k] = cards[l];
                k++;
            }
        }
        else
        {
            for (l = i; l <= mid; l++)
            {
                temp[k] = cards[l];
                k++;
            }
        }

        for (l = left; l <= right; l++)
        {
            cards[l] = temp[l];
        }
    }

    /// <summary>
    /// 타입 별로 정렬 메소드
    /// </summary>
    public void TypeSort(List<Card> cards)
    {
        isCostSort = false;
        List<Card> tempCards = new List<Card>();

        MergeSort(cards, tempCards, 0, cards.Count);
    }
    //전체라이브러리에 카드 추가
    public void AddLibrary(CardDataObject cdObj)
    {
        cardLibrary.Add(new Card(cdObj.EUnitType, cdObj.Type, cdObj.Cost, cdObj.Pokemon, cardLibraryAllPrefabs[(int)cdObj.Pokemon].GetComponent<Image>()));
    }

    //전체 타워카드에 카드 추가
    public void AddTowerCard(CardDataObject cdObj)
    {
        towerCards[(int)cdObj.TowerNmae] = new Card(cdObj.Type, cdObj.TowerNmae, TowercardLibraryPrefabs[(int)cdObj.TowerNmae].GetComponent<Image>());
    }

    /// <summary>
    /// 타워를 흭득해했을때 카드로 바꿔줄 메서드
    /// </summary>
    /// <param name="name"></param>
    public void GetTowerCard(ETowerPokemonName name)
    {
        for (int i = 0; i < towerCards.Length; i++)
        {
            if (towerCards[i].Name == name.ToString())
            {
                towerCards[i].IsAllCardGet = true;
                towerCards[i].CardImage.color = new Color(1f, 1f, 1f);
            }
        }
    }

    //장착된 타워카드로 바꿔줄 메소드
    public void SetTowerCard(ETowerPokemonName name)
    {
        for (int i = 0; i < towerCards.Length; i++)
        {
            if (towerCards[i].Name == name.ToString())
            {
                towerCards[i].IsUserCardEquip = true;
                //장착으로 바꿔준걸로 전부세팅
                for (int j = 0; j < equipTowerCards.Length; j++)
                {
                    equipTowerCards[j] = towerCards[i];

                }
            }
            else
            {
                towerCards[i].IsUserCardEquip = false;
            }
        }
    }
    /// <summary>
    /// 플레이어 카드 정보가져갈 메서드
    /// </summary>
    /// <returns></returns>
    public Card[] GetTowerCard()
    {
        return equipTowerCards;
    }

    //타워카드 이미지 컬렉션 화면에 뿌려줌
    public void TowerDeckAddImage(ETowerPokemonName name)
    {
        //TowerDeckImage 부모 오브젝트의 트랜스폼 = 오브젝트
        //Instantiate(프리팹,TowerDeckImage의 자식으로);
        //프리팹을 TowerDeckImage의 자식 오브젝트로 생성하겠다.

        //타워카드 아직 미생성
        var targetCard = Instantiate(towerCards[(int)name].CardImage, TowerDeckImage);
        targetCard.gameObject.name = name.ToString();
        targetCard.GetComponent<CardImage>().Card = towerCards[(int)name];
        targetCard.GetComponent<Image>().color = towerCards[(int)name].CardImage.color;
    }


    public void StageSet(int index)
    {
        switch (index)
        {
            case 0:
                //적 타워카드 설정
                EnemyEquipTowerCard(ETowerPokemonName.칠색조);
                //적 소환카드 장착
                EnemyStageCardSeting(index);

                break;

            case 1:
                //적 타워카드 설정
                EnemyEquipTowerCard(ETowerPokemonName.루기아);
                //적 소환카드 장착
                EnemyStageCardSeting(index);

                break;

        }

    }

    private void Awake()
    {
        towerPrefabs = new GameObject[3];//타워 프리팹
        cardLibraryAllPrefabs = new List<GameObject>();// 카드들 프리팹들
        _cardDataObjects = new List<CardDataObject>();//카드들 데이터 프리팹불러올예정

        TowercardLibraryPrefabs = new List<GameObject>(); // 타워들 프리팹들
        _ToewerDataObjects = new List<CardDataObject>();//타워들 데이터 프리팹불러올예정

        //프리팹 초기화 -유닛
        for (int i = 0; i < (int)EPokemonName.end; i++)
        {
            cardLibraryAllPrefabs.Add(Resources.Load<GameObject>("Prefabs/CardPrefabs/" + ((EPokemonName)i).ToString() ));
            //Debug.Log(cardLibraryAllPrefabs[i].GetComponent<Image>().ToString());
        }
        for (int i = 0; i < (int)EPokemonName.end; i++)
        {
            //Debug.Log(AssetDatabase.LoadAssetAtPath<CardDataObject>("Assets/ScriptableObject/Card/" + ((EPokemonName)i).ToString() + ".asset"));
            _cardDataObjects.Add(Resources.Load<CardDataObject>("ScriptableObject/Card/" + ((EPokemonName)i).ToString()));
        }

        //프리팹 초기화 -타워
        for (int i = 0; i < (int)ETowerPokemonName.end; i++)
        {
            TowercardLibraryPrefabs.Add(Resources.Load<GameObject>("Prefabs/CardPrefabs/" + ((ETowerPokemonName)i).ToString()));
            towerPrefabs[i] = TowercardLibraryPrefabs[i];
        }
        for (int i = 0; i < (int)ETowerPokemonName.end; i++)
        {
            _ToewerDataObjects.Add(Resources.Load<CardDataObject>("ScriptableObject/Card/" + ((ETowerPokemonName)i).ToString()));
        }

        SelectCard.SetActive(false);
        //카드 설정초기화

        //적 카드 설정
        enemyEquipCards = new Card[12];
        enemyTowerEquipCards = new Card[3];
        tempEnemyCardDeck = new List<Card>[2];//스테이지 추가시 늘려줘야함.
        for (int i = 0; i < tempEnemyCardDeck.Length; i++)
        {
            tempEnemyCardDeck[i] = new List<Card>(); // 각 리스트 초기화
        }
        //플레이어 카드 설정
        equipCards = new Card[12];
        equipTowerCards = new Card[3];
        getCards = new List<Card>();
        cardLibrary = new List<Card>();

        //전체라이브러리에 카드 추가
        for (int i = 0; i < (int)EPokemonName.end; i++)
        {
            AddLibrary(_cardDataObjects[i]);
        }

        //타워카드 기본값 추가
        for (int i = 0; i < (int)ETowerPokemonName.end; i++)
        {
            AddTowerCard(_ToewerDataObjects[i]);
        }

        //스테이지 1 적 덱 세팅
        EnemyEquipCard(0, EPokemonName.파이리);
        EnemyEquipCard(0, EPokemonName.리자드);
        EnemyEquipCard(0, EPokemonName.리자몽);
        EnemyEquipCard(0, EPokemonName.파이어);
        EnemyEquipCard(0, EPokemonName.이상해꽃);
        EnemyEquipCard(0, EPokemonName.버터플);
        EnemyEquipCard(0, EPokemonName.피죤투);
        EnemyEquipCard(0, EPokemonName.썬더);
        EnemyEquipCard(0, EPokemonName.이상해풀);
        EnemyEquipCard(0, EPokemonName.피죤);
        EnemyEquipCard(0, EPokemonName.잠만보);
        EnemyEquipCard(0, EPokemonName.푸크린);

        //스테이지 2 적 덱 세팅
        EnemyEquipCard(1, EPokemonName.꼬부기);
        EnemyEquipCard(1, EPokemonName.어니부기);
        EnemyEquipCard(1, EPokemonName.거북왕);
        EnemyEquipCard(1, EPokemonName.프리져);
        EnemyEquipCard(1, EPokemonName.이상해풀);
        EnemyEquipCard(1, EPokemonName.이상해꽃);
        EnemyEquipCard(1, EPokemonName.버터플);
        EnemyEquipCard(1, EPokemonName.피죤투);
        EnemyEquipCard(1, EPokemonName.썬더);
        EnemyEquipCard(1, EPokemonName.레큐쟈);
        EnemyEquipCard(1, EPokemonName.리자드);
        EnemyEquipCard(1, EPokemonName.리자몽);




        //스테이지별로 얻는카드 추가될 예정.
        currentStage = SceneLoader.LevelValue;//게임매니저가 여기다가 스테이지값 넣어줘야함.

        Debug.Log("현재 클리어 스테이지 : " + currentStage);

        //전체라이브러리중 클리어로 얻은 카드 추가
        if (currentStage >= 0)
        {
            //흭득카드
            GetCard(EPokemonName.구구);
            GetCard(EPokemonName.피죤);
            GetCard(EPokemonName.푸푸린);
            GetCard(EPokemonName.푸린);
            GetCard(EPokemonName.푸크린);
            GetCard(EPokemonName.먹고자);
            GetCard(EPokemonName.잠만보);
            GetCard(EPokemonName.꼬부기);
            GetCard(EPokemonName.이상해씨);
            GetCard(EPokemonName.이상해풀);
            GetCard(EPokemonName.캐터피);
            GetCard(EPokemonName.단데기);//12

            //타워 얻음
            GetTowerCard(ETowerPokemonName.뮤츠);


        }
        if (currentStage >= 1)
        {
            //흭득카드
            GetCard(EPokemonName.이상해꽃);
            GetCard(EPokemonName.버터플);
            GetCard(EPokemonName.피죤투);
            GetCard(EPokemonName.레큐쟈);
            GetCard(EPokemonName.썬더);


            //타워 얻음
            GetTowerCard(ETowerPokemonName.칠색조);


        }
        if (currentStage >= 2)
        {
            //흭득카드
            GetCard(EPokemonName.어니부기);
            GetCard(EPokemonName.거북왕);
            GetCard(EPokemonName.프리져);
            //타워 얻음
            GetTowerCard(ETowerPokemonName.루기아);
        }


        ////테스트용 장착용덱에 카드 추가.
        for (int i = 0; i < cardLibrary.Count; i++)
        {
            if (cardLibrary[i].IsAllCardGet == true)
            {
                EquipCard((EPokemonName)i);
            }

        }
        Debug.Log("stage : "  + currentStage);

        //전체덱중 사용 가능한 덱 제외하고 보여줄 메서드
        //미흭득카드 란에 추가
        foreach (var c in cardLibrary)
        {
            if (c.IsAllCardGet == false)
            {
                //Debug.Log(c.Pokemon);
                UnacquiredDeckAddImage(c.Pokemon);
            }
        }



        //타워 미흭득기준 컬러 추가 (현재는 뮤츠만있어서 적용안되는중)
        foreach (var t in TowerCards)
        {
            if (t.IsAllCardGet == false)
            {
                t.CardImage.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }



        //전체 타워 컬렉션에 보이게 추가
        for (int i = 0; i < towerCards.Length; i++)
        {
            TowerDeckAddImage((ETowerPokemonName)i);
        }


        SetTowerCard(ETowerPokemonName.뮤츠);

    }


}
