using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BombManager : MonoBehaviour
{
    public static BombManager instance;

    [Header("爆弾のカウント数")]
    public int bombCount;
    public int currentBombCount = 0;

    [Header("爆弾を叩いたか")]
    public bool bombClicked = false;

    [Header("爆弾を叩く上限")]
    public int maxBombClickCount;
    public int currentBombClickCount;

    [Header("爆弾を叩くボタン")]
    public Button bombClickButton;

    [Header("テキストUI")]
    public TMP_Text bombCountText;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //爆弾のカウント数をランダムで決める
    public void StartBombCount()
    {
        bombCount = UnityEngine.Random.Range(20, 41);
        currentBombCount = bombCount;

        UpdateBombCount();
    }

    //爆弾のカウントを減らす
    public void BombClick()
    {
        currentBombClickCount++;
        //爆弾を叩ける回数に上限を設ける
        if (currentBombClickCount == maxBombClickCount)
        {
            bombClickButton.interactable = false;
            Debug.Log("これ以上爆弾は叩けない");
        }

        bombClicked = true;
        currentBombCount--;
        Debug.Log("現在のカウント数は" + currentBombCount);

        //カウントが0以下になれば爆発する
        if (currentBombCount <= 0)
        {
            currentBombCount = 0;
            Debug.Log("爆弾が爆発した");

            GameManager.instance.GameOver();
        }

        UpdateBombCount();
    }

    //現在の爆弾のカウント数を表示する
    private void UpdateBombCount()
    {
        bombCountText.text = $"{currentBombCount}";
    }

    //ターン終了時に叩いたカウントをリセットする
    public void ResetTrunBombClick()
    {
        currentBombClickCount = 0;
        bombClickButton.interactable = true;
        bombClicked = false;
        Debug.Log("爆弾を叩いた数をリセットしました");
    }

    //田中作
    public void SetLimitedClicks(int max)
    {
        Debug.Log($"叩ける回数を {max} 回に制限");
        // 叩く回数を制限するロジックをここに
    }

    public void AddBombCount(int add)
    {
        bombCount += add;
        Debug.Log($"爆弾カウントを +{add} しました。現在: {bombCount}");
    }

    public void HideBombCountForOpponent()
    {
        Debug.Log("相手から爆弾カウントを隠しました");
        // UI上で相手に表示しないなどの処理を実装
    }

}
