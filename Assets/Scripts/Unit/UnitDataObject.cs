using UnityEngine;

// 절대 변하지 않을 수치들만 작성할 것 (유닛의 초기화 수치값)
[CreateAssetMenu(fileName = "PokemonData", menuName = "Scriptable Object/Pokemon Data")]
public class UnitDataObject : ScriptableObject
{
    [SerializeField] private int _attackRate; // 공격력
    [SerializeField] private float _attackRange; // 공격 사거리
    [SerializeField] private float _attackSpeed; // 공격 속도
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxMp;
    [SerializeField] private float _moveSpeed; // 이동 속도
    [SerializeField] private float _sightRange = 2.0f; // 시야 범위 일단은 고정    

    // 소환시 필요한 정보들
    [SerializeField] private int _unitQuantity; // 유닛 1번 소환당 소환량
    [SerializeField] private int _unitCoolTime; // 유닛 소환 이후에 다음 카드 낼때까지 걸리는 시간
    [SerializeField] private int _maxSpawn; // 필드에 최대 소환 갯수

    public int AttackRate
    {
        get { return _attackRate; }
    }

    public float AttackRange
    {
        get { return _attackRange; }
    }

    public float AttackSpeed
    {
        get { return _attackSpeed; }
    }

    public int MaxHp
    {
        get { return _maxHp; }
    }

    public int MaxMp
    {
        get { return _maxMp; }
    }

    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }

    public float SightRange
    {
        get { return _sightRange; }
    }

    public int UnitQuantity
    {
        get { return _unitQuantity; }
    }

    public int UnitCoolTime
    {
        get { return _unitCoolTime; }
    }

    public int MaxSpawn
    {
        get { return _maxSpawn; }
    }
}
