using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountDownUI : MonoBehaviour
{//스테이지 진입 후 5초 카운트 다운
    int startCountDown = 5;
    Text _text;
    void Start()
    {
        _text = GetComponent<Text>();
        GameManager.Instance.IsGameStart = false;
        StartCoroutine(CountDownStart());
    }

    IEnumerator CountDownStart()
    {//코루틴을 활용
        while (startCountDown > 0)
        {
            _text.text = startCountDown.ToString();
            yield return new WaitForSeconds(1);

            startCountDown--;
        }
        _text.text = "Game Start";
        GameManager.Instance.IsGameStart = true;
        yield return new WaitForSeconds(1);
        _text.text = "";
    }
    public int SetOnGameStartTimer()
    {
        return startCountDown;
    }
}
