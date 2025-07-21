using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    public GameObject cardLibrary;//전체 카드 라이브러리
    public GameObject DeckView;//덱 뷰 = 화면
    public GameObject CollectionView;//컬렉션 뷰 = 화면
    public GameObject UseCard;//카드 사용 눌렀을때 나올녀석.
    public GameObject SelectCard;//카드 클릭시 나올 이미지
    public GameObject UseButton;//카드 사용버튼
    public GameObject TowerSelect;//타워 선택창 

    bool IsDeckOpen;//이게 true면 덱오픈 false 이면 컬렉션 오픈
    
    //선택된 카드
    //Card selectCard;

    //덱오픈 버튼클릭
    public void DeckOpen()
    {
        SelectCard.SetActive(true);

        UseButton.SetActive(false);

        DeckView.SetActive(true);
        SelectCard.SetActive(false);
        CollectionView.SetActive(false);
        TowerCardEquip(true);


    }

    //타워카드 장착한거 표시해줄 메소드
    void TowerCardEquip(bool setting)
    {
        var count = gameObject.transform.childCount;
        var TCCardImage = gameObject.transform.GetChild(count - 1);
        var TCCardImageBG = gameObject.transform.GetChild(count - 2);
        TCCardImage.gameObject.SetActive(setting);
        TCCardImageBG.gameObject.SetActive(setting);

        if (cardLibrary.GetComponent<CardLibrary>().GetTowerCard()[0] != null)
        {
            TCCardImage.GetComponent<Image>().sprite = cardLibrary.GetComponent<CardLibrary>().GetTowerCard()[0].CardImage.sprite;
        }
    }

    public void DeckMove()
    {
        DeckOpen();
        gameObject.transform.parent.GetComponent<RectTransform>().position += new Vector3(1080f, 0, 0);
    }

    public void DeleteButton()
    {
        
        var eqCard = cardLibrary.GetComponent<CardLibrary>().EquipCards;
        for (int i = 0; i < eqCard.Length; i++)
        {
            if (eqCard[i] != null && eqCard[i].IsSelecte == true)
            {
                var imageArr = DeckView.GetComponentsInChildren<CardImage>();

                foreach (CardImage img in imageArr)
                {
                    if(img.Card.IsSelecte == true)
                    {
                        img.Card.IsUserCardEquip = false;
                        Destroy(img.gameObject);
                    }
                }
                eqCard[i] = null;
                break;
            }
        }
        SelectCard.SetActive(false);

    }

    //카드 사용누른뒤 덱화면에서 확인 눌렀을경우.
    public void UseOkButton()
    {
        //var eqCard = cardLibrary.GetComponent<CardLibrary>().equipCards;

        SelectCard.SetActive(false);
        UseCard.SetActive(false);
    }

    //컬렉션덱 오픈   
    public void CollectionOpen()
    {
        //컬렉션 화면일때 카드선택시 사용으로 켜짐

        DeckView.SetActive(false);
        UseCard.SetActive(false);
        SelectCard.SetActive(false);
        CollectionView.SetActive(true);
        UseButton.SetActive(true);
        TowerCardEquip(false);
        cardLibrary.GetComponent<CardLibrary>().EquipCardCheck();
    }


    //SelectCard의 사용버튼 눌렀을시 넘어옴.
    public void UseCardOpen()
    {
        bool isTower = false;

        //타워의 경우 장착해줌.
        var eqTowerCard = cardLibrary.GetComponent<CardLibrary>();
        var tower = GameObject.Find("TowerDeckImage");

        for (int i = 0; i < eqTowerCard.AllTowerCard.Length; i++)
        {
            if (eqTowerCard.AllTowerCard[i].IsSelecte == true)
            {
                eqTowerCard.SetTowerCard((ETowerPokemonName)i);
                isTower = true;
                cardLibrary.GetComponent<CardLibrary>().EquipCardCheck();

                Debug.Log(eqTowerCard.AllTowerCard[i].Name);
            }
        }

        //장착공간이 있을경우 덱오픈과 UseCard열어줌
        if (cardLibrary.GetComponent<CardLibrary>().EquipCardEmpty() == true && isTower == false)
        {
            UseCard.SetActive(true);
            DeckOpen();
        }
    }

    //첫 시작시 뷰 화면들 초기화
    private void Start()
    {
        CollectionView.SetActive(false);
        UseCard.SetActive(false);
        DeckOpen();
        SelectCard.SetActive(false);
        TowerCardEquip(true);
    }


}
