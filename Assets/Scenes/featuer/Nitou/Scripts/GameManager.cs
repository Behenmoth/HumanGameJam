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

    [Header("アイテムのインベントリー")]
    public List<int> player1ItemIdList = new List<int>();
    public List<int> player1ItemAmountList = new List<int>();

    public List<int> player2ItemIdList = new List<int>();
    public List<int> player2ItemAmountList = new List<int>();

    [Header("アイテム")]
    public int maxItem;
    public int giveItem;

    [Header("アイテム使用の可否")]
    public bool canUseItems = false;

    //爆弾の所持状況
    public enum BombHolder {None,Player1,Player2}

    //プレイヤーのターン
    public enum PlayerTurn {None,Player1,Player2}

    [Header("爆弾保持")]
    public BombHolder currentBombholder = BombHolder.None;

    [Header("ターン")]
    public PlayerTurn currentPlayerTurn = PlayerTurn.None;

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
        RoundManager();
    }

    // Update is called once per frame
    void Update()
    {
        TurnManager();
    }

    //ラウンドを管理する処理
    private void RoundManager()
    {
        if (currentRoundCount >= roundCount)
        {
            Debug.Log("全ラウンド終了");
            return;
        }

        if (player1WinCount >= winCount)
        {
            Debug.Log("プレイヤー1");
        }

        if (player2WinCount >= winCount)
        {
            Debug.Log("プレイヤー2");
        }

        currentRoundCount++;

        roundText.text = $"Round {currentRoundCount}";

        //アイテムを各プレイヤーに配る
        GiveItems();

        //どちらかのプレイヤーに爆弾を渡す
        GiveBombs();

        //爆弾のカウントをランダムで決める
        BombManager.instance.StartBombCount();

        //叩いたカウントをリセットする
        BombManager.instance.ResetTrunBombClick();

        //現在のターンを表示
        UpdateTurnUI();
    }

    //アイテムを各プレイヤーに配る処理
    private void GiveItems()
    {
        for(int i = 0; i < giveItem; i++)
        {

        }
        Debug.Log("アイテムを配った");
    }

    //どちらかのプレイヤーに爆弾を渡す処理
    private void GiveBombs()
    {
        int randomPlayer = UnityEngine.Random.Range(1, 3);

        if (randomPlayer == 1)
        {
            //プレイヤー1に爆弾を持たせる
            currentBombholder = BombHolder.Player1;
            currentPlayerTurn = PlayerTurn.Player1;
            Debug.Log("最初はプレイヤー1");
        }
        else
        {
            //プレイヤー2に爆弾を持たせる
            currentBombholder = BombHolder.Player2;
            currentPlayerTurn = PlayerTurn.Player2;
            Debug.Log("最初はプレイヤー2");
        }

        UpdateTurnUI();
    }

    //爆弾を渡す処理
    public void PassBomb()
    {
        if (currentBombholder == BombHolder.Player1)
        {
            currentBombholder = BombHolder.Player2;
            Debug.Log("プレイヤー1からプレイヤー2へ爆弾を渡した");
        }
        else if (currentBombholder == BombHolder.Player2) 
        {
            currentBombholder = BombHolder.Player1;
            Debug.Log("プレイヤー2からプレイヤー1へ爆弾を渡した");
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
        if (BombManager.instance.bombClicked == true)
        {
            nextTurnButton.interactable = true;
            Debug.Log("ネクストターンボタンを押せるようになった");
        }
        else
        {
            nextTurnButton.interactable = false;
        }
    }

    //ターンを相手に渡す処理
    public void PassTurn()
    {

        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            currentPlayerTurn = PlayerTurn.Player2;
            PassBomb();
            Debug.Log("プレイヤー1からプレイヤー2へターンを渡した");
        }
        else if (currentPlayerTurn == PlayerTurn.Player2)
        {
            currentPlayerTurn = PlayerTurn.Player1;
            PassBomb();
            Debug.Log("プレイヤー2からプレイヤー1へターンを渡した");
        }

        //爆弾を叩いた回数をリセット
        BombManager.instance.ResetTrunBombClick();

        UpdateTurnUI();
    }

    //GameOver時の処理
    public void GameOver()
    {
        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            Player2Win();
        }
        if (currentPlayerTurn == PlayerTurn.Player2)
        {
            Player1Win();
        }
    }

    //プレイヤー1が勝利したときの処理
    private void Player1Win()
    {
        Debug.Log("プレイヤー1が勝利しました");
        player1WinCount++;

        RoundManager();
    }

    //プレイヤー2が勝利したときの処理
    private void Player2Win()
    {
        Debug.Log("プレイヤー2が勝利しました");
        player2WinCount++;

        RoundManager();
    }

    //現在のターンを表示
    private void UpdateTurnUI()
    {
        turnText.text = $"{currentPlayerTurn}";
    }
}
