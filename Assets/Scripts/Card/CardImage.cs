using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardImage : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    GameObject cardLibrary;
    GameObject SelectCardImage;
    private Transform originalParent;
    private Image draggingImage;

    AudioSource _audioSource;


    Card _card;

    public Card Card
    {
        get { return _card; }
        set { _card = value; }
    }

    Image _image;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _image = GetComponent<Image>();

        //var temp = GetComponent<CardImage>();
        //왜 처음 한번만? 아. 이미지셀렉트 오브젝트가 처음 생성될때 정해지고 그 후론 변경이 안되서 그런가봄.
        //근데 포인터 클릭시 계속 갱신해 주는게 맞는거같음. 클릭받을떄마다 가져오니까...
        //만약 이전거 지우고 재생성 하는 방향이였으면 아마 여기다 두는게 맞는거같음.

        cardLibrary = GameObject.Find("CardLibrary");
        SelectCardImage = cardLibrary.GetComponent<CardLibrary>().SelectCard;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if(transform.parent.name == "UseCard")
        {
            return;
        }

        
        _audioSource.Play();
        

        if (Card.IsAllCardGet == true)
        {
            //this.Card.IsSelecte = true;
            cardLibrary.GetComponent<CardLibrary>().SelectClear(this.Card);
        }

        //흭득한 카드인가? 
        if (Card.IsSelecte = true && Card.IsAllCardGet == true)
        {

            SelectCardImage.SetActive(true);

            //선택한 카드 위에 이미지 하나 띄워줌 
            //SelectCard의 이미지를 바꿔줌. use 카드가 실제로 드래그나 대상카드선택으로 선택할 카드고 이건 정보 사용 제거 버튼 달린 오브젝트다.
            var images = SelectCardImage.GetComponentsInChildren<Image>();

            foreach (var image in images)
            {
                if (image.name == "Image")
                {
                    //해당 오브젝트에 카드값 전달                    
                    image.GetComponent<Image>().sprite = Card.CardImage.sprite;
                }
            }

            SelectCardImage.transform.position = transform.position;

            //사용중인 카드 설정
            var temp = GameObject.Find("CardView").transform.Find("UseCard").gameObject;
            temp.SetActive(true);

            var cards = temp.GetComponentsInChildren<Image>();

            foreach (var c in cards)
            {
                if (c.name == "Image")
                {
                    //해당 오브젝트에 카드값 전달
                    c.GetComponent<CardImage>().Card = Card;
                    c.GetComponent<Image>().sprite = Card.CardImage.sprite;
                }
            }
            temp.SetActive(false);
        }


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SelectCardImage.SetActive(false);

        if (this.Card.IsAllCardGet == true)
        {
            cardLibrary.GetComponent<CardLibrary>().SelectClear(this.Card);
        }
        else
        {
            return;
        }


        if (Card.IsSelecte = true && Card.IsAllCardGet == true)
        {
            draggingImage = GetComponent<Image>();


            originalParent = transform.parent;

            transform.SetParent(transform.root); // Canvas 맨 위로 올리기 -오브젝트 묶음중 최상단으로 이동.
            draggingImage.raycastTarget = false; // 드래그 중에는 Raycast 비활성화
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (this.Card.IsAllCardGet == true)
        {
            transform.position = Input.mousePosition; // 마우스 위치에 맞춰 드래그
        }
    }


    public void CardChange()
    {
        cardLibrary.GetComponent<CardLibrary>().EquipCard(Card.Pokemon);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드롭된 위치의 이미지 가져오기
        GameObject dropTarget = eventData.pointerEnter;

        SelectCardImage.SetActive(false);

        Debug.Log(dropTarget.name);

        if (this.Card.IsAllCardGet == true)
        {
            draggingImage.raycastTarget = true; // 드래그 종료 후 Raycast 활성화
                                                //Debug.Log(originalParent.position);
            transform.SetParent(originalParent);

            //여기다가 특정범위 안으로 마우스 포인터가 들어왔을경우 아래꺼 실행해주면 해결.
            //+ 현재카드를 장착덱으로 변경도 필수

            var tempLibrary = cardLibrary.GetComponent<CardLibrary>().EquipCards;
            //포켓몬에 드랍했을때 리턴. = 교환도 넣을라했으나 시간관계로...
            for (int i = 0; i < tempLibrary.Length; i++)
            {
                //Debug.Log(dropTarget.name);
                //Debug.Log(tempLibrary[i].Name);

                if (dropTarget != null && tempLibrary[i] != null && dropTarget.name == tempLibrary[i].Name)
                {
                    //교환할려면 여기넣으면될듯
                    //Debug.Log("여기 왔습니까");
                    transform.position = originalParent.position;

                    return;
                }
            }

            if (dropTarget != null && dropTarget.name == "DeckImage" && dropTarget.GetComponentInChildren<GridLayoutGroup>().name == "DeckImage")
            {

                var tempTrans = dropTarget.GetComponentInChildren<GridLayoutGroup>().transform;

                var rect = tempTrans.GetComponent<RectTransform>();

                //마우스 포인터가 드롭할때 DeckImage의 rect 안에 있을경우 호출되는 메서드-월드좌표랑 UI좌표랑 연동해서 계산후 결과알려줌
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, eventData.enterEventCamera))
                {
                    //장착할 여유가 되는지와 지금 현재 들고있는 카드가 장착중인지 여부 확인
                    if (cardLibrary.GetComponent<CardLibrary>().EquipCardEmpty() && Card.IsUserCardEquip == false)
                    {
                        this.Card.IsUserCardEquip = true;

                        var temp = cardLibrary.GetComponent<CardLibrary>().EquipCard(Card.Pokemon);

                        Image targetImage = temp.GetComponent<Image>();

                        // 이미지 교환
                        var tempSprite = targetImage.sprite;
                        targetImage.sprite = draggingImage.sprite;
                        draggingImage.sprite = tempSprite;

                        cardLibrary.GetComponent<CardLibrary>().EmptyCard.SetActive(true);
                        var empty = Instantiate(cardLibrary.GetComponent<CardLibrary>().EmptyCard, originalParent);
                        empty.name = "Image";//클론명칭 지움
                        cardLibrary.GetComponent<CardLibrary>().EmptyCard.SetActive(false);
                        GameObject.Find("CardView").GetComponent<ViewController>().UseCard.SetActive(false);
                        Destroy(gameObject);
                    }
                }
            }
        }


        if (dropTarget != null)
        {
            transform.position = originalParent.position;
        }

    }
}
