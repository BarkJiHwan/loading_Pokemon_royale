using UnityEngine;

public class MapManager : MonoBehaviour
{
    public void MovingWalk(Unit unit, GameObject target)
    {
        if (unit == null || target == null || unit.Roads == null || unit.Roads.Count == 0)
        {
            return;
        }
        Vector3 destination = target.transform.position;
        Vector3 myCoordinate = unit.transform.position;
        float roadDistance = 100;
        GameObject nextNode = null;

        for (int i = 0; i < unit.Roads.Count; i++)
        {
            var node = unit.Roads[i];
            if (node == null)
            {
                continue;
            }

            var tileInfo = node.gameObject.GetComponent<TileInfo>();
            if (tileInfo == null || !tileInfo.isMoveable)
            {
                continue;
            }

            float destinationDistance = Vector3.Distance(node.transform.position, destination);
            if (roadDistance > destinationDistance)
            {
                roadDistance = destinationDistance;
                myCoordinate = node.transform.position;
                nextNode = node;
            }
        }

        if (nextNode != null)
        {
            unit._nextNode = nextNode;
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, myCoordinate, unit.MoveSpeed * 0.002f);
        }
    }
}
