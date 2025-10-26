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
    public bool canUseItems = true;

    [Header("プレイヤーの名前")]
    string player1name = "Player1";
    string player2name = "Player2";


    [Header("リザルト")]
    public GameObject resultUI;

    [Header("勝者")]
    public TMP_Text winerText;

    [Header("ゲーム勝利")]
    public GameObject gameWin;
    public TMP_Text gameWinerText;
    public TMP_Text scoreText;

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

    [Header("各プレイヤーのインベントリー")]
    public PlayerInventry player1Inventory;
    public PlayerInventry player2Inventory;

    [Header("ボタン")]
    public Button nextTurnButton;
    public Button useItemButton;

    [Header("テキストUI")]
    public TMP_Text roundText;
    public TMP_Text turnText;

    [Header("UI制御")]
    public bool canNexttrunButton;

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
        //乱数の種を変える
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        canUseItems = true;

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
        if (player1WinCount >= winCount)
        {
            Debug.Log($"{player1name}");
            Player1GameWin();
            return;
        }

        if (player2WinCount >= winCount)
        {
            Debug.Log($"{player2name}");
            Player2GameWin();
            return;
        }

        currentRoundCount++;

        //ラウンドテキストを変える
        if (roundText != null) 
        {
            roundText.text = $"Round {currentRoundCount}";
        }

        //どちらかのプレイヤーに爆弾を渡す
        GiveBombs();

        //アイテムを各プレイヤーに配る
        ItemDistribution.instance.Distribution();

        //表示するアイテムを切り替え
        player1Inventory.SetActiveObjects(currentPlayerTurn);
        player2Inventory.SetActiveObjects(currentPlayerTurn);

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

            currentPlayerTurn = PlayerTurn.Player1;
            Debug.Log($"最初は{player1name}");
        }
        else
        {
            //プレイヤー2に爆弾を持たせる
            currentBombholder = BombHolder.Player2;

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
        //アイテム使用が1回だけならアイテムを使用可能
        useItemButton.interactable = canUseItems;

        //爆弾を1回以上叩かなければならない
        if (BombManager.instance != null && nextTurnButton != null)
        {
            if (BombManager.instance.bombClicked == true)
            {
                canNexttrunButton = true;
            }
            else
            {
                canNexttrunButton = false;
            }
        }

        //アイテムUIが開いている間はネクストターンボタンは押せない
        if (ItemUIManager.instance.isUiOpen)
        {
            canNexttrunButton = false;
        }

        nextTurnButton.interactable = canNexttrunButton;
    }

    //ターンを相手に渡す処理
    public void PassTurn()
    {
        Debug.Log($"{currentPlayerTurn}");

        //爆弾を相手に渡す(ターンを相手に渡す)
        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            currentPlayerTurn = PlayerTurn.Player2;
            PassBomb();

        }
        else if (currentPlayerTurn == PlayerTurn.Player2)
        {
            currentPlayerTurn = PlayerTurn.Player1;
            PassBomb();

            //プレイヤーアイテム切り替える
            Debug.Log($"{player2name}から{player1name}へターンを渡した");
        }

        //爆弾を叩いた回数をリセット
        if (BombManager.instance != null)
        {
            BombManager.instance.ResetTrunBombClick();
        }


        if (CountDownTimer.instance != null)
        {
            //タイマーをリセットする
            CountDownTimer.instance.ResetCountDownTimer();
            //タイマーのカウントダウン開始
            CountDownTimer.instance.StartCountDownTimer();
        }

        //ターン開始時にアイテムを使用可にする
        canUseItems = true;
        useItemButton.interactable = true;

        // ターンUI更新
        UpdateTurnUI();

        //表示するアイテムを切り替え
        player1Inventory.SetActiveObjects(currentPlayerTurn);
        player2Inventory.SetActiveObjects(currentPlayerTurn);
        Debug.Log($"{currentPlayerTurn}");
    }

    //相手のターンを飛ばす関数
    public void SkipOpponentTurn()
    {
        Debug.Log("相手のターンをスキップします");

        if (currentPlayerTurn == PlayerTurn.Player1)
        {

            currentBombholder = BombHolder.Player1;

            //爆弾のクリック数をリセットする
            if (BombManager.instance != null)
            {
                BombManager.instance.ResetTrunBombClick();
            }

            //カウントダウンタイマーをリセットする
            if (CountDownTimer.instance != null)
            {
                CountDownTimer.instance.ResetCountDownTimer();
                CountDownTimer.instance.StartCountDownTimer();
            }

            // プレイヤーターン更新
            currentPlayerTurn = PlayerTurn.Player1;
        }
        else if (currentPlayerTurn == PlayerTurn.Player2)
        {
            currentBombholder = BombHolder.Player2;

            //爆弾のクリック数をリセットする
            if (BombManager.instance != null)
            {
                BombManager.instance.ResetTrunBombClick();
            }

            //カウントダウンタイマーをリセットする
            if (CountDownTimer.instance != null)
            {
                CountDownTimer.instance.ResetCountDownTimer();
                CountDownTimer.instance.StartCountDownTimer();
            }

            currentPlayerTurn = PlayerTurn.Player2;
        }

        //ターン開始時にアイテムを使用可にする
        canUseItems = true;
        useItemButton.interactable = true;

        // ターンUI更新
        UpdateTurnUI();

        // 表示アイテム更新
        player1Inventory.SetActiveObjects(currentPlayerTurn);
        player2Inventory.SetActiveObjects(currentPlayerTurn);

        Debug.Log($"{currentPlayerTurn}");
    }

    //GameOver時の処理
    public void GameOver()
    {
        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            winerText.text = player2name;
            resultUI.SetActive(true);
            
        }
        else if (currentPlayerTurn == PlayerTurn.Player2)
        {
            winerText.text = player1name;
            resultUI.SetActive(true);
        }
    }

    //プレイヤー1が勝利したときの処理
    public void Player1Win()
    {
        Debug.Log($"{player1name}が勝利しました");
        player1WinCount++;

        RoundManager();
    }

    //プレイヤー2が勝利したときの処理
    public void Player2Win()
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


    public void Player1GameWin()
    {

    }

    public void Player2GameWin()
    {

    }

}
