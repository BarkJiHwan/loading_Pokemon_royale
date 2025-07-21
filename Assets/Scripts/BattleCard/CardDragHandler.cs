using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //드래그 될 때 이동될 아이콘
    public static GameObject begingDragObject;

    //슬롯이 아닌 곳에 드래그될 경우 원복될 좌표
    Vector3 startPos;

    //부모 레이어 저장 변수
    [SerializeField] Transform onDragParent;

    //슬롯이 아닌 곳에 드래그될 경우 원복될 레이어 저장 변수
    [HideInInspector] public Transform startParent;

    PokeBlockCount _pokeBlockCount;



    private void Awake()
    {
        onDragParent = GameObject.Find("CardUI").transform;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameStart == true)
        {
            begingDragObject = gameObject;

        startPos = transform.position;
        startParent = transform.parent;

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        transform.SetParent(onDragParent);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameStart == true)
        {
            transform.position = Input.mousePosition;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameStart == true)
        {
            _pokeBlockCount = GameObject.Find("Canvas").transform.GetChild(0).GetChild(4).GetComponent<PokeBlockCount>();
            Card card = gameObject.GetComponent<BattleCardInfo>().Card;
            GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (transform.parent == onDragParent)
            {
                transform.position = startPos;
                transform.SetParent(startParent);
            }
            else if(card.Cost <=_pokeBlockCount.PokeBlockCost)
            {

                // 캔버스에서 플레이어 필드 찾기
                RectTransform rect = GameObject.Find("Canvas").transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);

                if (RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position, eventData.enterEventCamera))
                {
                    CardManager.Instance.Spawner.SpawnUnit(card, worldPosition);
                    CardManager.Instance.ReMoveHand(begingDragObject);
                    Destroy(begingDragObject);
                    _pokeBlockCount.PokeBlockMinusCost(card);
                }
            }
            else
            {
                transform.position = startPos;
                transform.SetParent(startParent);
            }
        }
    }
}
