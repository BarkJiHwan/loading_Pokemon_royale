using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    GameObject GameObject()
    {
        if(transform.childCount > 0)
        {
            return transform.GetChild(0).gameObject;
        }
        else
        {
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(GameObject() == null && GameManager.Instance.IsGameStart == true)
        {
            CardDragHandler.begingDragObject.transform.SetParent(transform);
            CardDragHandler.begingDragObject.transform.position = transform.position;
        }
    }
}
