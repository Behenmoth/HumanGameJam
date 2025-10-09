using TMPro;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    public static CountDownTimer instance;

    [Header("タイマー")]
    public float limitTimer;
    public float currentTimer;
    public bool isCountDown = false;
    public TMP_Text countDownTimerText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Update()
    {
        //カウントダウン中のみ動く
        if (isCountDown)
        {
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0)
            {
                currentTimer = 0;
                isCountDown = false;

                //カウントが0になるとゲームオーバー
                GameManager.instance.GameOver();
            }
            UpdateTimerUI();
        }
    }

    //タイマーのカウントダウンを開始する
    public void StartCountDownTimer()
    {
        currentTimer = limitTimer;
        isCountDown = true;

        Debug.Log("タイマー開始");
    }

    //タイマーのカウントダウンのリセット
    public void ResetCountDownTimer()
    {
        currentTimer = limitTimer;
        isCountDown = false;
        UpdateTimerUI();

        Debug.Log("タイマーリセット");
    }

    //タイマーのUIを更新
    public void UpdateTimerUI()
    {
        countDownTimerText.text = $"{currentTimer:F2}";
    }
}
