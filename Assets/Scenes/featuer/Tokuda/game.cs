using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // ★ TextMeshProを使用するために追加

public class GameManager_HotSeat : MonoBehaviour
{
    // --- UI/設定 ---
    // 型をTextMeshProUGUIに統一
    public TextMeshProUGUI counterText;       // ★ TMPROに修正
    public Button mainClickButton;
    public Button passButton;
    public TextMeshProUGUI turnIndicatorText; // ★ TMPROに修正

    [Header("Game Settings")]
    public int minStartCounter = 20;
    public int maxStartCounter = 40;

    // --- 状態変数 ---
    private int currentCounter;
    private int clicksMadeInTurn;
    private bool isPlayer1Turn = true; // Player 1 (true) または Player 2 (false)
    private bool isClickingAllowed;
    private const int MAX_CLICKS_PER_TURN = 3;

    // スコアリング (3ラウンド制、2本先取)
    private int player1Score = 0;
    private int player2Score = 0;
    private int currentRound = 1;

    void Start()
    {
        // UIイベントの設定
        mainClickButton.onClick.AddListener(HandlePushClick);
        passButton.onClick.AddListener(HandlePass);

        StartGame();
    }

    // --- ゲームフロー ---
    void StartGame()
    {
        player1Score = 0;
        player2Score = 0;
        currentRound = 1;

        StartRound();
    }

    void StartRound()
    {
        // カウンターをランダムに初期化
        currentCounter = Random.Range(minStartCounter, maxStartCounter + 1);
        counterText.text = currentCounter.ToString(); // TMProのTextへの代入

        // 最初のターンはランダムに決定
        isPlayer1Turn = (Random.Range(0, 2) == 0);

        StartTurn();
    }

    void StartTurn()
    {
        clicksMadeInTurn = 0;
        passButton.gameObject.SetActive(false);
        isClickingAllowed = true;
        mainClickButton.interactable = true;

        // どちらのプレイヤーのターンかを表示
        string playerTurnName = isPlayer1Turn ? "プレイヤー 1" : "プレイヤー 2";
        turnIndicatorText.text = playerTurnName + " のターンです"; // TMProのTextへの代入
    }

    // --- プレイヤーアクション ---
    public void HandlePushClick()
    {
        if (!isClickingAllowed) return; // クリックが許可されていない場合は無視

        // ★ 早期にクリックを無効化 (連続クリック防止)
        isClickingAllowed = false;

        // 1. カウンターを減らす
        currentCounter--;
        clicksMadeInTurn++;
        counterText.text = currentCounter.ToString();

        // 2. 爆発チェック
        if (currentCounter <= 0)
        {
            EndRound(isPlayer1Turn); // 爆発させた側のプレイヤーが敗北
            return;
        }

        // 3. PASSボタンと継続可否の判定
        if (clicksMadeInTurn >= 1 && clicksMadeInTurn < MAX_CLICKS_PER_TURN)
        {
            // 1回目または2回目のクリック後
            passButton.gameObject.SetActive(true); // PASSボタンを表示
            isClickingAllowed = true; // ★ PASSか継続クリックの選択肢を与えるため再有効化
        }
        else if (clicksMadeInTurn == MAX_CLICKS_PER_TURN)
        {
            // 3回目のクリック完了後 -> 強制ターン交代
            mainClickButton.interactable = false;
            StartCoroutine(WaitAndEndTurn(0.5f));
        }
        else
        {
            // PASSが押されず、かつ3回未満の場合は、継続クリックが可能なため再有効化
            isClickingAllowed = true;
        }
    }

    public void HandlePass()
    {
        if (!isClickingAllowed) return;

        isClickingAllowed = false; // ★ ターン終了前にクリックを無効化

        // PASSボタンを無効化し、ターン交代
        mainClickButton.interactable = false;
        passButton.gameObject.SetActive(false);
        StartCoroutine(WaitAndEndTurn(0.1f));
    }

    // --- ターン終了 ---
    IEnumerator WaitAndEndTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPlayer1Turn = !isPlayer1Turn; // ターンを交代
        StartTurn();
    }

    // --- ラウンド終了とスコアリング ---
    void EndRound(bool playerWhoLost)
    {
        // 敗者側を判定し、相手にスコアを加算
        if (playerWhoLost)
        {
            player2Score++; // プレイヤー1が爆発させた場合、プレイヤー2の勝利
            turnIndicatorText.text = "プレイヤー 2 の勝利！\nスコア: " + player1Score + " - " + player2Score;
        }
        else
        {
            player1Score++; // プレイヤー2が爆発させた場合、プレイヤー1の勝利
            turnIndicatorText.text = "プレイヤー 1 の勝利！\nスコア: " + player1Score + " - " + player2Score;
        }

        mainClickButton.interactable = false;

        // ゲーム全体の勝敗チェック (2本先取)
        if (player1Score == 2 || player2Score == 2)
        {
            EndGame();
            return;
        }

        // 次のラウンドへ
        currentRound++;
        StartCoroutine(WaitAndStartNextRound(3.0f));
    }

    IEnumerator WaitAndStartNextRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartRound();
    }

    void EndGame()
    {
        string winner = (player1Score == 2) ? "プレイヤー 1" : "プレイヤー 2";
        turnIndicatorText.text = winner + "がゲームの勝者です！";
        mainClickButton.interactable = false; // クリックを停止
    }
}