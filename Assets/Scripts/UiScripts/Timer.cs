using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float extraTimer; // 연장전 게임 타이머 120초 부터
    private float suddenDeathTimer; // 서든데스 타이머 180초 부터
    private float suddenDeathExtra; // 서든데스 연장전 서든데스에서 60초 후 부터
    private float playTime; //게임시간 3분, 전체 게임 시간 5분
    private float temppokeBlockReLoad; //pokeBlockReLoad정보를 담을 변수
    private float timeOver; //서든데스 종료점

    [SerializeField] private static float gameTimer; // 게임 타이머

    private Text timerText;
    private float _sec;
    private int _min;
    private PokeBlockCount _pokeBlockCount;
    private PokeBlockBurning _pokeBlockBurning;
    private Image[] _ButtonConttroler = new Image[2];
    private void Awake()
    {
        _pokeBlockCount = GameObject.Find("Canvas").transform.GetChild(0).GetChild(4).GetComponent<PokeBlockCount>();
        _pokeBlockBurning = GameObject.Find("Sudden Death").GetComponent<PokeBlockBurning>();
        _ButtonConttroler[0] = GameObject.Find("Win").GetComponent<Image>();
        _ButtonConttroler[1] = GameObject.Find("Defeat").GetComponent<Image>();
        _pokeBlockBurning.SetSuddenDeathBurningOver();
    }
    void Start()
    {
        _ButtonConttroler[0].GameObject().SetActive(false);
        _ButtonConttroler[1].GameObject().SetActive(false);
        gameTimer = 0f;
        extraTimer = 120f;
        suddenDeathTimer = 120f;
        suddenDeathExtra = 240f;
        playTime = 180f;
        timeOver = 300f;
        //temppokeBlockReLoad = _pokeBlockCount.PokeBlockReLoad;
        timerText = GetComponent<Text>();
        _min = 3; //min시간 절대값 절대 건들지마세요. 3분시작
        _sec = 0; //sec시간 절대값 절대 건들지마세요. 0초시작
        Time.timeScale = 1;
    }
    public void EnmeyTowerDestroy()
    {//유저 승리 에네미타워 터짐
        if (GameObject.Find("EnemySpawner").GetComponent<UnitSpawner>().GetCastle().activeInHierarchy == false)
        {
            Time.timeScale = 0;
            _ButtonConttroler[0].GameObject().SetActive(true);
        }
    }
    public void PlayerTowerDestroy()
    {//유저 패배 유저타워 터짐
        if (GameObject.Find("PlayerSpawner").GetComponent<UnitSpawner>().GetCastle().activeInHierarchy == false)
        {
            Time.timeScale = 0;
            _ButtonConttroler[1].GameObject().SetActive(true);
        }
    }
    public void TimerStart()
    {
        if (GameManager.Instance.IsGameStart == true && GameManager.Instance.IsGameOver == false)
        {
            gameTimer += Time.deltaTime;
            onTimer();
            if (gameTimer > extraTimer && GameManager.Instance.IsextralTime == false)
            {//연장전 시작(스테이지 진입 후 2분 뒤 시작)
                //▼포케블록 회복량 2배
                _pokeBlockBurning.SetSuddenDeathBurningStart();
                _pokeBlockCount.PokeBlockReLoad /= 2;
                GameManager.Instance.OnExtraTimer();
            }
            if (gameTimer > playTime && GameManager.Instance.IsextralTime == true)
            {//서든데스 시작(연장전 끝나고 시작)
                GameManager.Instance.OnSuddenDeath();//서든데스 시작 메소드
            }
            if (gameTimer > suddenDeathExtra && GameManager.Instance.IsSuddenDeath == true)
            {//서든데스 시작 후 60초 뒤(서든데스 연장전 시작)
                //▼포케블록 회복량 3배 기존값 2.5의 3배
                GameManager.Instance.OffExtraTimer();
                GameManager.Instance.OffSuddenDeath();
                _pokeBlockCount.PokeBlockReLoad = temppokeBlockReLoad / 3;
                GameManager.Instance.OnSuddenExtra();
            }
            if (gameTimer >= timeOver && GameManager.Instance.IsSuddenExtra == true)
            {
                GameManager.Instance.OffSuddenExtra();
                GameManager.Instance.GameOver();
                GameManager.Instance.IsGameStart = false; //게임 종료
                Time.timeScale = 0;
                gameTimer = 0f;// 시간 멈춤
                //후처리 서든데스 까지 끝났지만 여전히 상대와 동일한 타워수를 보유한 경우 게임 패배로 간주
                //상대가 AI이기 때문에 정한 설정
            }
        }
    }
    void onTimer()
    {//오른쪽 상단 분:초 를 표기해 주는 메소드
        _sec -= Time.deltaTime;

        timerText.text = string.Format("{0:D2}:{1:D2}", _min, (int)_sec);

        if (_sec < 0)
        {
            _sec = 60;
            _min--;
        }
        else if (_min < 0)
        {
            _min += 3;
            _sec = 0;
        }
    }
    void Update()
    {
        TimerStart();


        EnmeyTowerDestroy();
        PlayerTowerDestroy();
    }
}
