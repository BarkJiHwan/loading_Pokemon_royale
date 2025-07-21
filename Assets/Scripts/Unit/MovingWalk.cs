using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWalk : MonoBehaviour
{
    //필요한 정보
    //1. 노드
    //2. 이동
    private DrawingMap map;
    //private float MoveSpeed = 1f;
    Vector3 zero = Vector3.zero;
    float timer;
    public GameObject top;
    TileInfo _tileInfo;
    public DrawingMap Map { get => map; set => map = value; }

    private void Start()
    {
        Map = GameObject.Find("Roads").GetComponent<DrawingMap>();
    }
    private void Update()
    {
    }
}


