using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private bool _isGameOver; // 패배
    private bool _isGameClear; // 승리
    private bool _isGameStart; // 스테이지 진입
    private bool _isextralTime; // 연장전
    private bool _isSuddenDeath; // 서든데스
    private bool _isSuddenExtra; // 서든데스 연장전
    private bool _istimeOver; //서든데스도 종료


    //추가해야될 내용
    //상대 포탑이 파괴 되었을 경우 조건 처리(승리 했을 때의 처리)
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public bool IsGameOver { get => _isGameOver; set => _isGameOver = value; }
    public bool IsGameClear { get => _isGameClear; set => _isGameClear = value; }
    public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }
    public bool IsextralTime { get => _isextralTime; set => _isextralTime = value; }
    public bool IsSuddenDeath { get => _isSuddenDeath; set => _isSuddenDeath = value; }
    public bool IsSuddenExtra { get => _isSuddenExtra; set => _isSuddenExtra = value; }
    public bool IstimeOver { get => _istimeOver; set => _istimeOver = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void EnterStage()
    {//스테이지 진입 확인 매서드
        IsGameStart = true;
    }
    public void GameClear()
    {//승리 했는지?
        SceneLoader.Instance.LevelUp();

        AllTimeFalse();
        IsGameClear = true;
    }
    public void OnExtraTimer()
    {
        IsextralTime = true; //연장전 시작
    }
    public void OffExtraTimer()
    {
        IsextralTime = false; //연장전 종료
    }
    public void OnSuddenDeath() //서든데스 시작
    {
        IsSuddenDeath = true;
    }
    public void OffSuddenDeath() //서든데스 종료
    {
        IsSuddenDeath = false;
    }
    public void OnSuddenExtra() //서든데스 엑스트라 시작
    {
        IsSuddenExtra = true;
    }
    public void OffSuddenExtra() //서든데스 엑스트라 종료
    {
        IsSuddenExtra = false;
    }
    public void GameOver()//게임 종료 또는 무승부 일때 실행
    {
        IsGameOver = true;
        AllTimeFalse();
    }
    public void AllTimeFalse()
    {
        _isGameOver = false;
        _isGameClear = false;
        _isGameStart = false;
        _isextralTime = false;
        _isSuddenDeath = false;
        _isSuddenExtra = false;
        _istimeOver = false;
    }
    void Start()
    {
        _isGameOver = false;
        _isGameClear = false;
        _isGameStart = false;
        _isextralTime = false;
        _isSuddenDeath = false;
        _isSuddenExtra = false;
        _istimeOver = false;
    }

    void Update()
    {

    }
}
