using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EUnitType
{
    지상, 공중, 건물
}

[Serializable]
public struct Attackable
{
    public bool attackGround;
    public bool attackAir;
    public bool attackTower;
}

public class Unit : MonoBehaviour
{
    public List<GameObject> Roads { get; set; }
    public GameObject _nextNode;

    // 유닛의 유니크 아이디
    [SerializeField] private int uniqueId;

    // 기본 개념 : 원본의 수치가 UnitObject에 있고, 현재 유닛의 _editable이 붙은 수치는 수정가능한 수치
    // 스탯의 버프와 너프가 있으면 _editableAttackRate를 수정하고, 버프, 너프가 끝나면 0으로 돌린다
    // 몬스터에서 제어하는 정보들 - 공격력, 사정거리, 공격속도, 체력, 이동속도, 유닛 속성, 공격대상
    [SerializeField] private UnitDataObject unitData;

    private int _editableAttackRate;
    private float _editableAttackRange;
    private float _editableAttackSpeed;
    private float _editableMoveSpeed;
    private int _currentHp;
    private int _currentMp;

    // 유닛이 어떤 유형인가
    [SerializeField] private EUnitType _unitType;
    // 지상, 공중, 건물 공격 여부
    [SerializeField] private Attackable attackAble;

    // 카드에서 제어하는 정보들 - 속성, 이름, 코스트, 이미지
    private Card _card;
    // 몬스터 위치 정보
    private Vector3 _pos;

    // 상황에 따라 가변적인 값들 사용가능 스킬, 상태
    [SerializeField] private UnitSkill _unitSkill;
    private IUnitState _currentState;
    [SerializeField] private UnitSpawner _unitSpawner;
    // 내가 현재 공격할 대상을 담아두는 필드
    [SerializeField] private GameObject _enemyTarget;
    private UnitSpawner _enemySpawner;
    // 지금 상호작용 하고있는 유닛들 (떄릴려고 달려오는 애들)
    [SerializeField] private List<GameObject> _followingUnits = new List<GameObject>();
    [SerializeField] private MapManager _mapManager;
    private Animator _animation;
    private HealthUI _healthUI;

    private GameObject _hpBarPref;
    private GameObject _canvas;

    public HealthUI HealthUI
    {
        get => _healthUI;
        set => _healthUI = value;
    }

    public int AttackRate
    {
        get => UnitData.AttackRate + _editableAttackRate;
    }

    public void SetAttackRate(int attackRate)
    {
        _editableAttackRate = attackRate;
    }

    public float AttackRange
    {
        get => UnitData.AttackRate + _editableAttackRange;
    }

    public void SetAttackRange(float attackRange)
    {
        _editableAttackRange = attackRange;
    }

    public float AttackSpeed
    {
        get => UnitData.AttackSpeed + _editableAttackSpeed;
    }

    public void SetAttackSpeed(float attackSpeed)
    {
        _editableAttackSpeed = attackSpeed;
    }

    public int Hp
    {
        get => _currentHp;
        private set => _currentHp = value;
    }

    public int Mp
    {
        get => _currentMp;
        private set => _currentMp = value;
    }

    public void AddManaPoint(int gain)
    {
        Mp += gain;
        if (Mp >= UnitData.MaxMp)
        {
            Mp = UnitData.MaxMp;
        }
    }

    public void ResetManaPoint()
    {
        _currentMp = 0;
    }

    public float MoveSpeed
    {
        get => UnitData.MoveSpeed + _editableMoveSpeed;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        _editableMoveSpeed = moveSpeed;
    }

    public Attackable Attackable
    {
        get => attackAble;
    }

    public UnitSkill Skill
    {
        get => _unitSkill;
        set => _unitSkill = value;
    }

    public IUnitState CurrentState
    {
        get => _currentState;
        private set => _currentState = value;
    }

    public int UniqueId
    {
        get => uniqueId;
        set => uniqueId = value;
    }

    public string GetName()
    {
        return _card.Name;
    }

    public int GetCost()
    {
        return _card.Cost;
    }

    public EPokemonType GetCardType()
    {
        return _card.GetCardType();
    }

    public EPokemonName GetPokemon()
    {
        return _card.Pokemon;
    }

    public String UniqueKey => _card.Name + UniqueId;

    public Vector3 Pos
    {
        get => gameObject.transform.position;
        set => _pos = value;
    }

    public GameObject EnemyTarget
    {
        get => _enemyTarget;
    }

    public void SetSpawner(UnitSpawner spawner)
    {
        _unitSpawner = spawner;
    }

    public void SetCard(Card card)
    {
        _card = card;
    }

    public float CoolTime
    {
        get;
        private set;
    }

    public List<GameObject> FollowingUnits
    {
        get => _followingUnits;
    }

    public MapManager MapManager => _mapManager;

    public EUnitType UnitType => _unitType;

    public UnitDataObject UnitData => unitData;

    private void OnEnable()
    {
        ChangeState(new SearchState());
        _currentHp = UnitData.MaxHp;

        //_healthUI.gameObject.SetActive(true);
        //_healthUI.name = gameObject.name;
        //_healthUI.SetUnit(gameObject);
    }

    private void Awake()
    {
        _mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        _animation = GetComponent<Animator>();
        _hpBarPref = Resources.Load<GameObject>("Prefabs/HpBar");
        _canvas = GameObject.Find("Canvas");
        Roads = new List<GameObject>();
        //Destroy( _healthUI.gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        CurrentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        CurrentState.FixedUpdateState(this);
    }

    public void ChangeState(IUnitState newState)
    {
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    public void MakeHealthUI()
    {
        _healthUI = Instantiate(_hpBarPref, _canvas.transform).GetComponent<HealthUI>();

    }

    // 갑지된 대상 유닛을 공격 할 수 있는가
    // 상대방 유닛의 EUnitType의 값으로 지상 공격여부, 공중 공격 여부, 건물 공격 여부를 찾을 수 있다
    public bool CanAttack(Unit other)
    {
        if (other.UnitType == EUnitType.지상)
        {
            return Attackable.attackGround;
        }

        if (other.UnitType == EUnitType.공중)
        {
            return Attackable.attackAir;
        }

        if (other.UnitType == EUnitType.건물)
        {
            return Attackable.attackTower;
        }

        Debug.Log("공격 할 수 없는 대상");
        return false;
    }

    // 스킬 사용, MP가 유닛의 최대 MP값이라면 스킬을 쏠 수 있다.
    public bool CanShootSkill()
    {
        if (Mp >= UnitData.MaxMp)
        {
            return true;
        }

        return false;
    }

    // 데미지 입었을 때의 설정
    // 1. 데미지를 입었는데
    // - 지금 내가 설정한 적 타겟이 없다 혹은 적이 내 때리는 사정거리 밖에있다
    // - 떄릴 대상을 바꾼다
    public void TakeDamage(int damage, GameObject from)
    {
        if (_enemyTarget == null || IsEnemyOutOfAttackRange())
        {
            ChangeEnemyTarget(from);
        }

        Hp -= damage;

        if (HealthUI != null)
        {
            HealthUI.HpUpdate();
        }

        if (Hp <= 0)
        {
            Hp = 0;
            Die();
        }
    }

    // 나 죽었을때, 팔로잉 유닛들에게 죽었다고 전파, 초기화 후 풀로 돌아감
    private void Die()
    {
        AnnounceDie();
        _editableAttackRate = 0;
        _editableAttackRange = 0;
        _editableAttackSpeed = 0;
        _editableMoveSpeed = 0;
        _currentMp = 0;
        _currentHp = UnitData.MaxHp;
        _enemyTarget = null;
        _followingUnits.Clear();
        _unitSpawner.ReturnUnit(this);
        HealthUI.HpDestroy();
        destroyTower();
        foreach (GameObject road in Roads)
        {
            Destroy(road);
        }
        Roads.Clear();
    }

    // 죽으면서 내 죽음을 알려라
    private void AnnounceDie()
    {
        foreach (GameObject obj in FollowingUnits)
        {
            obj.GetComponent<Unit>().NoticeDie();
        }
    }

    // 내 상대의 죽음을 알았을 경우
    private void NoticeDie()
    {
        _followingUnits.Remove(_enemyTarget);
        _enemyTarget = null;
    }

    // 위치를 받아서 두개간의 거리를 계산해준다.
    public double CalculateDistance(Vector3 myPos, Vector3 otherPos)
    {
        float xDiff = Math.Abs(myPos.x - otherPos.x);
        float yDiff = Math.Abs(myPos.y - otherPos.y);
        return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
    }

    // 1. 현재 공격 대상으로 찍힌 상대가 있으면 바로 리턴한다.
    // 2. 없다면 적의 딕셔너리에서 탐색한다
    // 3. 공격할수 있는 적인지 판단한다.
    // 4. 공격할수 있는 적이라면 거리를 판단해서 시야에 들어왔는지 확인한다.
    // 5. 공격할수 있는 적 중 최단 거리에 있는 적을 담아둔다.
    // 6. _enemyTarget 에 넣어둔 후 종료한다.
    public void FindAttackableNearestEnemy()
    {
        if (EnemyTarget != null)
        {
            return;
        }

        GameObject nearestTarget = null;
        double tempDistance = 0.0d;
        double nearestDistance = 0.0d;

        if (_enemySpawner == null)
        {
            if (gameObject.tag.Equals("Enemy"))
            {
                _enemySpawner = UnitSpawner.GetSpawner(ESpawner.PlayerSpawner).GetComponent<UnitSpawner>();
            } else
            {
                _enemySpawner = UnitSpawner.GetSpawner(ESpawner.EnemySpawner).GetComponent<UnitSpawner>();
            }
        }

        foreach (String key in _enemySpawner.FieldDict.Keys)
        {
            GameObject other = _enemySpawner.FieldDict[key];

            // 대상을 때릴 수 있는지 확인
            if (!CanAttack(other.GetComponent<Unit>()))
            {
                continue;
            }

            Vector3 otherPos = other.transform.position;
            tempDistance = CalculateDistance(gameObject.transform.position, otherPos);

            if (tempDistance <= UnitData.SightRange)
            {
                if (nearestDistance == 0.0d)
                {
                    nearestDistance = tempDistance;
                }

                if (tempDistance <= nearestDistance)
                {
                    nearestDistance = tempDistance;
                    nearestTarget = other;
                }
            }
        }

        if (nearestTarget != null)
        {
            ChangeEnemyTarget(nearestTarget);
        }
    }

    // 초당 AttackSpeed 만큼 공격함
    public void Attack()
    {
        // 공격중에 적이 사라졌다 (죽었다)
        if (EnemyTarget == null)
        {
            return;
        }

        CoolTime += Time.deltaTime;
        float attackPerSec = 1 / AttackSpeed;

        if (CoolTime >= attackPerSec)
        {
            PlayDefaultWalk();
            PlayAttackAnimator();
            CoolTime = 0;
            Unit enemyUnit = EnemyTarget.GetComponent<Unit>();
            enemyUnit.TakeDamage(UnitData.AttackRate, gameObject);
        }
    }

    // 내가 찍어둔 타겟이 사정거리 밖에있는가
    public bool IsEnemyOutOfAttackRange()
    {
        double distance = CalculateDistance(gameObject.transform.position, _enemyTarget.transform.position);
        if (distance > UnitData.AttackRange)
        {
            return true;
        }

        return false;
    }

    // 벽 무시하고 포지션으로 바로 날아가기 (공중유닛)
    public void moveToFly(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, UnitData.MoveSpeed * 0.002f);
    }

    // 대상 바꿀때 해줘야하는 행동들
    public void ChangeEnemyTarget(GameObject enemy)
    {
        // 마침 적 대상을 바꾸는 도중에 죽어버렸다
        if (enemy.activeInHierarchy == false)
        {
            return;
        }

        if (_enemyTarget != null)
        {
            _enemyTarget.GetComponent<Unit>().FollowingUnits.Remove(gameObject);
        }

        _enemyTarget = enemy;
        enemy.GetComponent<Unit>().FollowingUnits.Add(gameObject);
    }

    // 공격 애니메이터 재생하기
    public void PlayAttackAnimator()
    {
        if (_animation == null)
        {
            return;
        }

        float diffX = _enemyTarget.transform.position.x - gameObject.transform.position.x;
        float diffY = _enemyTarget.transform.position.y - gameObject.transform.position.y;

        // X축 위치가 더 많이 벌어져있다면 왼쪽, 오른쪽 공격모션
        // Y축 위치가 더 많이 벌어져 있다면 위쪽, 아래쪽 공격모션
        if (Math.Abs(diffX) > Math.Abs(diffY))
        {
            if (_enemyTarget.transform.position.x < gameObject.transform.position.x)
            {
                _animation.SetTrigger("IsLeftAttack");
            }
            else if (_enemyTarget.transform.position.x > gameObject.transform.position.x)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
                _animation.SetTrigger("IsRightAttack");
            }
        }
        else
        {
            if (_enemyTarget.transform.position.y > gameObject.transform.position.y)
            {
                _animation.SetTrigger("IsBackAttack");
            }
            else if (_enemyTarget.transform.position.y < gameObject.transform.position.y)
            {
                _animation.SetTrigger("IsFrontAttack");
            }
        }
    }

    public void DecideWalkParameter(GameObject me, GameObject target)
    {
        if (me.transform.position.y < target.transform.position.y)
        {
            PlayDefaultWalk();
        }
        else
        {
            PlayFrontWalk();
        }
    }

    public void PlayDefaultWalk()
    {
        if (_animation == null)
        {
            return;
        }
        _animation.SetBool("IsFrontMove", false);
    }

    public void PlayFrontWalk()
    {
        if (_animation == null)
        {
            return;
        }
        _animation.SetBool("IsFrontMove", true);
    }

    // HP바 따라다니기
    public void HpMove()
    {
        if (HealthUI != null)
        {
            HealthUI.HpMove();
        }
    }
    public void MyMapPing(GameObject unit)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector3 tempPos = new Vector3((unit.transform.position.x + 0.25f) - (i*0.25f), (unit.transform.position.y + 0.25f) - (j*0.25f));
                if (HasRoad(unit, tempPos))
                {
                    continue;
                }
                var tempRoad = Instantiate(GameObject.Find("Roads").GetComponent<DrawingMap>()._road, tempPos, transform.rotation, GameObject.Find("Roads").transform);
                if (i == 1 && j == 1)
                {
                    tempRoad.GetComponent<TileInfo>().isMoveable = false;
                }
                Roads.Add(tempRoad);
            }
        }
    }

    public bool HasRoad(GameObject unit, Vector3 tempPos)
    {
        for (int i = 0; i < unit.GetComponent<Unit>().Roads.Count; i++)
        {
            if (Roads[i].transform.position == tempPos)
            {
                return true;
            }
        }
        return false;
    }
    public void destroyTower()
    {
        Debug.Log("들어옴");
        if (gameObject.name == "Castle")
        {
            if (gameObject.transform.tag == "Enemy")
            {
                GameManager.Instance.GameClear();
            }
            else if (gameObject.transform.tag == "Player")
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}
