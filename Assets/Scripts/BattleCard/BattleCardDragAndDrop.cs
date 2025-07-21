using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleCardDragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //드래그 될 때 이동될 아이콘
    public static GameObject begingDragObject;

    //슬롯이 아닌 곳에 드래그될 경우 원복될 좌표
    Vector3 startPos;

    //부모 레이어 저장 변수
    [SerializeField] Transform onDragParent;

    //슬롯이 아닌 곳에 드래그될 경우 원복될 레이어 저장 변수
    [HideInInspector] public Transform startParent;


    public void OnBeginDrag(PointerEventData eventData)
    {
        begingDragObject = gameObject;

        startPos = transform.position;
        startParent = transform.parent;

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        transform.SetParent(onDragParent);
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        begingDragObject = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;


        if (transform.parent == onDragParent)
        {
            transform.position = startPos;
            transform.SetParent(startParent);
        }
    }
}
