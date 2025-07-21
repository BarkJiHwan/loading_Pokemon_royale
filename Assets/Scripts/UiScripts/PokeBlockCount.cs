using UnityEngine;
using UnityEngine.UI;

public class PokeBlockCount : MonoBehaviour
{
    private bool ispokeBlockUp;

    private float _pokeBlockUpTime; //포케블록 증가 시간
    private float _pokeBlockCoolTime; //포케블록 쿨타임 0시작 +델타타임
    private float _pokeBlockReLoad; //포케블록 1개 회복 시간 2.5초
    private float _pokeBlockReLoadTimer; //포케블록 리로드 + 델타타임
    private int _startMaxpokeBlockCost; //최대 포케블록 코스트 10시작
    private int _maxpokeBlockCost; //20초++ 최대 포케블록 15
    private int _pokeBlockCost; //포케블록 코스트
    private int _enemyPokeBlockCost;
    private GameManager _gameManager;
    private Image _pokeBlockImage;//차오를 포케블록 이미지
    float _currentPokeBlock;//포케 블록 이미지에 담길 현재 코스트
    [SerializeField] Text _pokeBlockCostText;// 차오를 포케블록 텍스트
    private CardManager _cardManager;

    public float PokeBlockReLoad { get => _pokeBlockReLoad; set => _pokeBlockReLoad = value; }
    public int PokeBlockCost { get => _pokeBlockCost; set => _pokeBlockCost = value; }
    public int EnemyPokeBlockCost { get => _enemyPokeBlockCost; set => _enemyPokeBlockCost = value; }
    public int StartMaxpokeBlockCost { get => _startMaxpokeBlockCost; set => _startMaxpokeBlockCost = value; }

    public int PokeBlockMinusCost(Card card)
    {
        PokeBlockCost -= card.Cost;
        return PokeBlockCost;
    }
    public int EnemyPokeBlockMinusCost(Card card)
    {
        EnemyPokeBlockCost -= card.Cost;
        return EnemyPokeBlockCost;
    }
    private void Awake()
    {
        _pokeBlockImage = GetComponent<Image>();
    }
    void Start()
    {
        ispokeBlockUp = false;
        _pokeBlockUpTime = 20f;
        _pokeBlockCoolTime = 0;
        _pokeBlockReLoad = 2.5f; //포케블록 회복 시간
        _pokeBlockReLoadTimer = 0f;
        PokeBlockCost = 0;
        EnemyPokeBlockCost = 0;
        StartMaxpokeBlockCost = 10;
        _maxpokeBlockCost = 15; //20초마다 1씩 증가해서 최대치는 15까지 증가할 예정
        _gameManager = GameManager.Instance;
    }
    private void FixedUpdate()
    {
        if (_gameManager.IsGameStart == true)
        {
            MaxPokeBlockReLoad();
        }

        if (StartMaxpokeBlockCost < _maxpokeBlockCost && ispokeBlockUp == true)
        {
            Debug.Log("포케블록 최대치 1증가");
            StartMaxpokeBlockCost++;
            _pokeBlockCoolTime = 0;
            ispokeBlockUp = false;
        }
    }

    public void MaxPokeBlockReLoad()
    {
        _pokeBlockCoolTime += Time.deltaTime;
        _pokeBlockReLoadTimer += Time.deltaTime;
        if (_pokeBlockCoolTime >= _pokeBlockUpTime && ispokeBlockUp == false)
        {
            ispokeBlockUp = true;
        }
        if (_pokeBlockReLoadTimer >= _pokeBlockReLoad)
        {
            GetpokeBlockCost();
            _cardManager.NoticePokeBlock();
        }
        PokeBlockCostUpDate();
    }
    public void GetpokeBlockCost()
    {
        if (PokeBlockCost < StartMaxpokeBlockCost)
        {
            _pokeBlockReLoadTimer = 0;
            PokeBlockCost++;
            EnemyPokeBlockCost++;
        }
    }

    public void PokeBlockCostUpDate()
    {
        float _currentPokeBlock;//포케 블록 이미지에 담길 현재 코스트
        _currentPokeBlock = (float)_pokeBlockCost / (float)_startMaxpokeBlockCost;
        _pokeBlockImage.fillAmount = _currentPokeBlock;
        if(_pokeBlockCost < 0)
        {
            _pokeBlockCost = 0;
        }
        _pokeBlockCostText.text = _pokeBlockCost.ToString();
    }

    public void SetCardManager(CardManager cardManager)
    {
        _cardManager = cardManager;
    }
}
