using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public bool isMoveable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        this.isMoveable = false;
    }
}
