using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("プレイヤーの勝利数")]
    public int player1WinCount;
    public int player2WinCount;

    [Header("アイテム使用の可否")]
    public bool canUseItems = false;

    [Header("プレイヤーの名前")]
    string player1name = "Player1";
    string player2name = "Player2";

    //爆弾の所持状況
    public enum BombHolder {None,Player1,Player2}

    //プレイヤーのターン
    public enum PlayerTurn {None,Player1,Player2}

    [Header("爆弾保持")]
    public BombHolder currentBombholder = BombHolder.None;

    [Header("ターン")]
    public PlayerTurn currentPlayerTurn = PlayerTurn.None;
    public bool isPlayer1 = false;

    [Header("ラウンド数")]
    public int roundCount;
    public int currentRoundCount;
    public int winCount;

    [Header("ボタン")]
    public Button nextTurnButton;

    [Header("テキストUI")]
    public TMP_Text roundText;
    public TMP_Text turnText;

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

        currentRoundCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TurnManager();
    }

    //ラウンドを管理する処理
    public void RoundManager()
    {
        if (currentRoundCount >= roundCount)
        {
            Debug.Log("全ラウンド終了");
            return;
        }

        if (player1WinCount >= winCount)
        {
            Debug.Log($"{player1name}");
        }

        if (player2WinCount >= winCount)
        {
            Debug.Log($"{player1name}");
        }

        currentRoundCount++;

        if (roundText != null) 
        {
            roundText.text = $"Round {currentRoundCount}";
        }


        //アイテムを各プレイヤーに配る
        if (ItemDistributor.instance != null)
        {
            ItemDistributor.instance.DistributeItems();
        }
        else
        {
            Debug.LogWarning("ItemDistributor がシーンに存在しません");
        }

        //どちらかのプレイヤーに爆弾を渡す
        GiveBombs();

        if (BombManager.instance != null)
        {
            //爆弾のカウントをランダムで決める
            BombManager.instance.StartBombCount();
            //叩いたカウントをリセットする
            BombManager.instance.ResetTrunBombClick();
        }
        else
        {
            Debug.LogWarning("BombManager がシーンに存在しません");
        }

        //現在のターンを表示
        UpdateTurnUI();

        if (CountDownTimer.instance != null)
        {
            //タイマーをリセットする
            CountDownTimer.instance.ResetCountDownTimer();
            //タイマーのカウントダウン開始
            CountDownTimer.instance.StartCountDownTimer();
        }
        else
        {
            Debug.LogWarning("CountDownTimer がシーンに存在しません");
        }

    }

    //どちらかのプレイヤーに爆弾を渡す処理
    private void GiveBombs()
    {
        int randomPlayer = UnityEngine.Random.Range(1, 3);

        if (randomPlayer == 1)
        {
            //プレイヤー1に爆弾を持たせる
            currentBombholder = BombHolder.Player1;
            isPlayer1 = true;

            currentPlayerTurn = PlayerTurn.Player1;
            Debug.Log($"最初は{player1name}");
        }
        else
        {
            //プレイヤー2に爆弾を持たせる
            currentBombholder = BombHolder.Player2;
            isPlayer1 = false;

            currentPlayerTurn = PlayerTurn.Player2;
            Debug.Log($"最初は{player2name}");
        }

        UpdateTurnUI();
    }

    //爆弾を渡す処理
    public void PassBomb()
    {
        if (currentBombholder == BombHolder.Player1)
        {
            currentBombholder = BombHolder.Player2;
            Debug.Log($"{player1name}から{player2name}へ爆弾を渡した");
        }
        else if (currentBombholder == BombHolder.Player2) 
        {
            currentBombholder = BombHolder.Player1;
            Debug.Log($"{player2name}から{player1name}へ爆弾を渡した");
        }
    }

    //各プレイヤーのターン処理
    private void TurnManager()
    {
        //nextTurnButton.interactable = false;
        //アイテム使用が1回だけならアイテムを使用可能
        //if (ItemManager.instance.usedItems == true)
        //{
        //    canUseItems = true;
        //}

        //爆弾を1回以上叩かなければならない
        if (BombManager.instance != null && nextTurnButton != null)
        {
            nextTurnButton.interactable = true;
        }
        else
        {
            nextTurnButton.interactable = false;
        }
    }

    //ターンを相手に渡す処理
    public void PassTurn()
    {
        if (ItemDisplay.instance == null)
        {
            Debug.LogWarning("ItemDisplay が見つかりません");
            return;
        }

        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            currentPlayerTurn = PlayerTurn.Player2;
            PassBomb();

            //プレイヤーアイテム切り替える
            Debug.Log("プレイヤーのアイテムを切り替えた");
            ItemDisplay.instance.SetPlayerTarget(ItemDisplay.PlayerTarget.Player2);//ItemRate.instance.conditionalaGetRandomItem(ItemDistributor.instance, isPlayer1);
            
            Debug.Log($"{player1name}から{player2name}へターンを渡した");

        }
        else if (currentPlayerTurn == PlayerTurn.Player2)
        {
            currentPlayerTurn = PlayerTurn.Player1;
            PassBomb();

            //プレイヤーアイテム切り替える
            Debug.Log("プレイヤーのアイテムを切り替えた");
            ItemDisplay.instance.SetPlayerTarget(ItemDisplay.PlayerTarget.Player1);//ItemRate.instance.conditionalaGetRandomItem(ItemDistributor.instance, isPlayer1);

            Debug.Log($"{player2name}から{player1name}へターンを渡した");

        }

        //爆弾を叩いた回数をリセット
        if (BombManager.instance != null)
        {
            BombManager.instance.ResetTrunBombClick();
        }

        //ItemManager.instance.ResetUsedItems();

        if (CountDownTimer.instance != null)
        {
            //タイマーをリセットする
            CountDownTimer.instance.ResetCountDownTimer();
            //タイマーのカウントダウン開始
            CountDownTimer.instance.StartCountDownTimer();
        }

        UpdateTurnUI();
    }

    //GameOver時の処理
    public void GameOver()
    {
        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            Player2Win();
        }
        else if (currentPlayerTurn == PlayerTurn.Player2)
        {
            Player1Win();
        }
    }

    //プレイヤー1が勝利したときの処理
    private void Player1Win()
    {
        Debug.Log($"{player1name}が勝利しました");
        player1WinCount++;

        RoundManager();
    }

    //プレイヤー2が勝利したときの処理
    private void Player2Win()
    {
        Debug.Log($"{player2name}が勝利しました");
        player2WinCount++;

        RoundManager();
    }

    //現在のターンを表示
    private void UpdateTurnUI()
    {
        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            turnText.text = $"{player1name}";
        }
        else if (currentPlayerTurn == PlayerTurn.Player2)
        {
            turnText.text = $"{player2name}";
        }
        
    }

    //入力した名前を反映させる処理
    public void SetPlayerNames(string name1, string name2)
    {
        player1name = name1;
        player2name = name2;

        UpdateTurnUI();
    }
}
