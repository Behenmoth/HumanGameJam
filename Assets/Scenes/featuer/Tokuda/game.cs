using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Game : MonoBehaviour
{
    // --- UI/設定 ---
    public TextMeshPro counterText;       // 3Dオブジェクトのカウンター表示用
    public Button mainClickButton;
    public Button passButton;

    public Text turnIndicatorText; // Canvas上のターン表示用

    [Header("Turn Indicator Settings")]
    public RectTransform turnIndicatorRect; // ★ turnIndicatorTextのRectTransform
    public float leftPositionX = -200f;     // ★ 1P側のX座標（例: -200）
    public float rightPositionX = 200f;    // ★ 2P側のX座標（例: 200）

    [Header("Item Card UI")]
    public Button[] itemCardButtons;         // 各アイテムカード

    [Header("Item Panel UI")]
    public GameObject itemPanel;            // 説明と選択肢を含むパネル全体
    public Image itemDescriptionImage;       // アイテムの説明を表示するImageコンポーネント
    public Button useItemButton;            // 「使う」ボタン
    public Button cancelItemButton;         // 「キャンセル」ボタン

    public Sprite[] itemDescriptionSprites; // アイテム画像


    [Header("Round Result UI")]
    public GameObject roundResultPanel;       // ラウンド結果パネル全体
    public TextMeshProUGUI roundWinnerText;  // 勝利者表示用テキスト (例: "1P WIN")
    public TextMeshProUGUI roundScoreText;   // スコア表示用テキスト (例: "SCORE 1 - 0")
    public Button nextButton;                 // NEXTボタン


    [Header("Game Info UI")]
    public TextMeshProUGUI roundNumberText; // ★ ラウンド数を常に表示するための新しいテキスト
    public TextMeshProUGUI scoreText;       // ★ スコアを常に表示するための新しいテキスト

    [Header("Round Start UI")] // ★ 新しいヘッダー
    public GameObject roundStartPanel;        // ラウンド開始パネル全体
    public TextMeshProUGUI roundStartNumberText; // "ROUND ○" 表示用テキスト

    [Header("Game Info Rects")] // ★ 新しいRectTransform変数
    public RectTransform roundNumberRect;
    public RectTransform scoreRect;
    public float infoRightX = 200f;       // ★ 情報表示が右側に来る場合のX座標 (元: topPositionX)
    public float infoLeftX = -200f;      // ★ 情報表示が左側に来る場合のX座標 (元: bottomPositionX)

    [Header("Rule UI")]
    public Button ruleButton;             // ルール表示開始ボタン
    public GameObject rulePanel;          // ルールパネル全体
    public Button closeRuleButton;        // ルールパネルを閉じるボタン
    public Image ruleImage;               // ルール画像 (Imageコンポーネント)
    public Sprite ruleSprite;             // Unityエディタで設定するルール画像

    [Header("Game Over UI")]
    public GameObject gameOverPanel;          // ゲームオーバーパネル全体
    public Text winnerText;       // WINNER表示用テキスト
    public Text youDiedText;      // YOU DIED表示用テキスト
    public Text loserMessageText; // 負け犬メッセージ用テキスト
   
    [Header("Game Settings")]
    public int minStartCounter = 20;
    public int maxStartCounter = 40;

    [Header("Messages")]
    public string[] loserMessages = new string[] // ★ 負け犬メッセージの候補
    {
        "今日からお前の名前は負け犬だ。",
        "才能の差だ。家で泣け。",
        "敗北を知りたい。",
        "ゴミが、消えろ。",
        "ふふ...所詮、この程度の男よ。",
        "残念だが、才能も運もなかったな。"
    };

    // --- 状態変数 ---
    private int currentCounter;
    private int clicksMadeInTurn;
    private bool isPlayer1Turn = true;
    private bool isClickingAllowed;
    private const int MAX_CLICKS_PER_TURN = 3;

    // 現在選択中のアイテムを保持する変数 (ここではインデックスを文字列で保存するのが一般的)
    private string selectedItemName = ""; // ★ 今回は使わないが、将来の機能拡張のために残す

    // スコアリング (3ラウンド制、2本先取)
    private int player1Score = 0;
    private int player2Score = 0;
    private int currentRound = 1;

    void Start()
    {
        // UIイベントの設定
        mainClickButton.onClick.AddListener(HandlePushClick);
        passButton.onClick.AddListener(HandlePass);
    
        // ★ ルールボタンのリスナーを追加
        ruleButton.onClick.AddListener(ShowRulePanel);
        closeRuleButton.onClick.AddListener(HideRulePanel);

        // ★ リスナー登録のブロックを1回にまとめる
        for (int i = 0; i < itemCardButtons.Length; i++)
        {
            int index = i;
            // ラムダ式を使い、クリック時にそのボタンのインデックスを渡す
            itemCardButtons[index].onClick.AddListener(() => OnItemCardClicked(index));
        }

        // アイテムパネルのボタンリスナー
        useItemButton.onClick.AddListener(UseItem);
        cancelItemButton.onClick.AddListener(HideItemPanel);

        // 初期状態でアイテムパネルを非表示にする
        if (itemPanel != null)
        {
            itemPanel.SetActive(false);
        }

        // ★ 初期状態でルールパネルを非表示にする
        if (rulePanel != null)
        {
            rulePanel.SetActive(false);
        }

        // ★ ラウンド結果パネルを初期状態で非表示にする
        if (roundResultPanel != null)
        {
            roundResultPanel.SetActive(false);
        }


        // ★ ゲームオーバーパネルのボタンリスナーと初期非表示を追加
        nextButton.onClick.AddListener(OnNextButtonClicked);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
 
        // ★ ラウンド開始パネルを初期状態で非表示にする
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(false);
        }
        StartGame();
    }

    // --- ゲームフロー ---
    void StartGame()
    {
        player1Score = 0;
        player2Score = 0;
        currentRound = 1;
        
        // ★ ラウンド数を初期表示
        if (roundNumberText != null)
        {
            roundNumberText.text = $"ROUND {currentRound}";
        }
        
        // ★ スコアを初期表示
        if (scoreText != null)
        {
            scoreText.text = $"SCORE {player1Score} - {player2Score}";
        }

        StartRound();
    }

    void StartRound()
    {
        currentCounter = Random.Range(minStartCounter, maxStartCounter + 1);
        counterText.text = currentCounter.ToString();
        isPlayer1Turn = (Random.Range(0, 2) == 0);
        
        // ★ ターン開始処理をコルーチンに置き換え、メッセージを表示
        StartCoroutine(ShowRoundStartMessage(2.0f));
    }
    
    IEnumerator ShowRoundStartMessage(float delay)
    {
        // 操作を一時的にロック
        isClickingAllowed = false; 
        mainClickButton.interactable = false;
        passButton.interactable = false;
        
        // ★ 他の常時表示UIも一時的に非表示にする
        if (turnIndicatorText != null) turnIndicatorText.gameObject.SetActive(false);
        if (roundNumberText != null) roundNumberText.gameObject.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);


        // --- ★ ラウンド開始パネルを表示し、テキストを設定 ---
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(true);
            if (roundStartNumberText != null)
            {
                roundStartNumberText.text = $"ROUND {currentRound}";
            }
        }
        

        // ★ ラウンド数とスコアを表示
        turnIndicatorText.text = $"ラウンド {currentRound} 開始！ スコア: {player1Score} - {player2Score}";

        // 待機
        yield return new WaitForSeconds(delay);

        // --- ★ ラウンド開始パネルを非表示にする ---
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(false);
        }
        
        // ★ 他の常時表示UIを再表示する
        if (turnIndicatorText != null) turnIndicatorText.gameObject.SetActive(true);
        if (roundNumberText != null) roundNumberText.gameObject.SetActive(true);
        if (scoreText != null) scoreText.gameObject.SetActive(true);


        // 待機後に実際のターンを開始
        StartTurn(); 
    }

    void StartTurn()
    {
        clicksMadeInTurn = 0;
        isClickingAllowed = true;
        mainClickButton.interactable = true;
        passButton.interactable = true;
        
        // ここでUIのgameObject.SetActive(true)は行わない。
        // ShowRoundStartMessageの最後に再表示されるため。

        // --- ターンインジケーターの位置を更新 ---
        float targetXTurn = isPlayer1Turn ? leftPositionX : rightPositionX;
        if (turnIndicatorRect != null)
        {
            turnIndicatorRect.anchoredPosition = new Vector2(targetXTurn, turnIndicatorRect.anchoredPosition.y); 
        }

        // --- ラウンド/スコアの位置を更新 ---
        float targetXInfo = isPlayer1Turn ? infoRightX : infoLeftX;
        
        if (roundNumberRect != null)
        {
            roundNumberRect.anchoredPosition = new Vector2(targetXInfo, roundNumberRect.anchoredPosition.y);
        }

        if (scoreRect != null)
        {
            scoreRect.anchoredPosition = new Vector2(targetXInfo, scoreRect.anchoredPosition.y);
        }

        string playerTurnName = isPlayer1Turn ? "1P" : "2P";
        turnIndicatorText.text = playerTurnName + " turn";
    }
    

    // --- ルール表示アクション ---
    // ルールボタンがクリックされたときに、ルールパネルを表示します。
    public void ShowRulePanel()
    {
        if (rulePanel == null || ruleSprite == null)
        {
            Debug.LogError("ルールパネルまたはルール画像が設定されていません。");
            return;
        }

        // 1. ルール画像をImageコンポーネントに設定
        ruleImage.sprite = ruleSprite;

        // 2. パネルを表示
        rulePanel.SetActive(true);

        // 3. 他のメイン操作を一時的にロック
        isClickingAllowed = false;
        mainClickButton.interactable = false;
        passButton.interactable = false;

        // ルールボタン自体も連打防止のため無効化
        ruleButton.interactable = false;
    }
    
    // 閉じるボタンがクリックされたときに、ルールパネルを非表示にします。

    public void HideRulePanel()
    {
        if (rulePanel == null) return;

        // 1. パネルを非表示
        rulePanel.SetActive(false);

        // 2. メイン操作のロックを解除（ターン開始時と同じ状態に戻す）
        isClickingAllowed = true;
        
        // 現在のclicksMadeInTurnに基づき、メインボタンとPASSボタンの状態を復元
        if (clicksMadeInTurn < MAX_CLICKS_PER_TURN)
        {
            mainClickButton.interactable = true;
        }
        else
        {
            // 3回プッシュ後の場合は、PASSのみ可能
            mainClickButton.interactable = false;
        }
        
        // PASSボタンとルールボタンは常に有効化
        passButton.interactable = true;
        ruleButton.interactable = true; 
    }

    // --- アイテムアクション ---

    // ★ アイテムカードのクリック時に呼び出されるメソッド
    public void OnItemCardClicked(int itemIndex)
    {
        // ターン中でない、または既にアイテムパネルが表示中の場合は無視
        if (!isClickingAllowed || itemPanel.activeSelf) return;

        // データのチェック
        if (itemIndex < 0 || itemIndex >= itemDescriptionSprites.Length)
        {
            Debug.LogError("アイテムインデックスが無効です: " + itemIndex);
            return;
        }

        // 1. 説明画像を切り替える
        itemDescriptionImage.sprite = itemDescriptionSprites[itemIndex];

        // 2. メインの操作を一時停止し、パネルを表示
        mainClickButton.interactable = false;
        passButton.interactable = false; // ★ PASSボタンも操作不能にする
        isClickingAllowed = false; // アイテム処理中は他の操作をロック

        // パネルを表示
        itemPanel.SetActive(true);
    }

    // ★ 「使う」ボタンが押されたときの処理
    public void UseItem()
    {
        Debug.Log("アイテムの使用を確定しました（効果なし）。ターンは継続します。");

        // 1. パネルを非表示にする
        HideItemPanel();

    }

    // ★ 「キャンセル」ボタンが押されたときの処理
    public void HideItemPanel()
    {
        itemPanel.SetActive(false);
        selectedItemName = "";

        // 1. メインボタンの再有効化
        if (clicksMadeInTurn < MAX_CLICKS_PER_TURN)
        {
            // 3回プッシュ未満ならメインボタンを有効にする
            mainClickButton.interactable = true;
        }
        else
        {
            // 3回プッシュ済みの場合は無効のまま（PASS必須）
            mainClickButton.interactable = false;
        }

        // 2. PASSボタンを有効化
        
        passButton.interactable = true;
        

        // 3. ゲーム全体のクリックロックを解除
        isClickingAllowed = true;
    }

    // --- プレイヤーアクション ---
    public void HandlePushClick()
    {
        if (!isClickingAllowed) return;

        isClickingAllowed = false; // 連続クリック防止

        // 1. カウンターを減らす
        currentCounter--;
        clicksMadeInTurn++;
        counterText.text = currentCounter.ToString();

        // 2. 爆発チェック
        if (currentCounter <= 0)
        {
            isClickingAllowed = false;
            mainClickButton.interactable = false;
            passButton.interactable = false;
            EndRound(isPlayer1Turn);
            return;
        }

        // 3. PASSボタンと継続可否の判定
      
            isClickingAllowed = true; // 継続クリックまたはPASS選択のため再有効化

        if (clicksMadeInTurn == MAX_CLICKS_PER_TURN)
        {
            mainClickButton.interactable = false; // 3回プッシュ後はメインボタンを無効化
                // PASSボタンは有効のまま
        }
        else
        {
            // 1回目、2回目ならプッシュボタンは有効のまま
            mainClickButton.interactable = true;
        }
        
        
    }

    public void HandlePass()
    {
        Debug.Log("HandlePass() Called. Attempting turn end."); // ★ 確認用ログ

        if (!isClickingAllowed) return;

        //  1回もプッシュしていない場合は処理を中断する
        if (clicksMadeInTurn == 0)
        {
            Debug.Log("PASSボタン: 1回もプッシュしていないため無効です。");
            return;
        }
        isClickingAllowed = false;
        mainClickButton.interactable = false;
        passButton.interactable = false; 
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
        isClickingAllowed = false;
        mainClickButton.interactable = false;
        passButton.interactable = false;
        ruleButton.interactable = false; // ルールボタンも無効化

        // アイテムやルールパネルも閉じる（もし開いていた場合）
        if (itemPanel.activeSelf) itemPanel.SetActive(false);
        if (rulePanel.activeSelf) rulePanel.SetActive(false);

        // ★ ターン表示とラウンド/スコア表示を非表示にする
        if (turnIndicatorText != null) turnIndicatorText.gameObject.SetActive(false);
        if (roundNumberText != null) roundNumberText.gameObject.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);


        if (playerWhoLost)
        {
            player2Score++;
        }
        else
        {
            player1Score++;
        }
        
        // ★ スコアを更新
        if (scoreText != null)
        {
            scoreText.text = $"SCORE {player1Score} - {player2Score}";
        }

       // turnIndicatorText.text = $"ラウンド終了！ スコア: {player1Score} - {player2Score}";

        if (player1Score == 2 || player2Score == 2)
        {
            EndGame();
            return;
        }

       
        // --- ★ ラウンド結果パネル（YOU DIED画面）の表示ロジック ---
        // 敗北メッセージは、ラウンド終了時に表示する。
        if (gameOverPanel != null) //gameOverPanelがYOU DIED画面
        {
            gameOverPanel.SetActive(true); // パネルを表示

            string loserName = playerWhoLost ? "1P" : "2P"; // 爆発させたプレイヤーが負け
            string winnerName = playerWhoLost ? "2P" : "1P"; // 相手プレイヤーが勝利

            // テキストを設定
            youDiedText.text = "YOU DIED";
            
            // 2. ★ ランダムな負け犬メッセージを選択
            string randomMessage = "";
            if (loserMessages.Length > 0)
            {
                int randomIndex = Random.Range(0, loserMessages.Length);
                randomMessage = loserMessages[randomIndex];
            } else
            {
                randomMessage = "敗者よ、静かに眠れ。"; // 候補がない場合のフォールバック
            }
            
            // 3. 負けたプレイヤー名とランダムメッセージを表示
            loserMessageText.text = $"{randomMessage} ({loserName})"; 
            
            // 勝利したプレイヤー名を表示
            winnerText.text = $"{winnerName} WIN! (Round {currentRound})"; 

            // 次のラウンドへ移行
            StartCoroutine(WaitAndStartNextRound(3.0f)); 
        }
        else
        {
            // パネルが設定されていない場合のフォールバック
            currentRound++;
            StartCoroutine(WaitAndStartNextRound(3.0f));
        }

    }

    IEnumerator WaitAndStartNextRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        

        // ★ YOU DIEDパネルを非表示にする
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        currentRound++; // 次のラウンドへ進める
        
        // ★ 新しいラウンド数を表示
        if (roundNumberText != null)
        {
            roundNumberText.text = $"ROUND {currentRound}";
        }
        
        StartRound();
    }
    public void OnNextButtonClicked()
    {
        if (roundResultPanel != null)
        {
            roundResultPanel.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // パネルを非表示にする
        }

        // ★ ゲームをリスタートする
        StartGame();

    }

    void EndGame()
    {  
        // ★ シンプルなスコアパネル（roundResultPanel）で最終結果を表示
        if (roundResultPanel == null)
        {
            string winner = (player1Score == 2) ? "1P" : "2P";
            turnIndicatorText.text = $"Winner {winner}";
            return;
        }
        
        // --- ゲーム終了パネル（シンプルなスコア画面）の表示ロジック ---

        roundResultPanel.SetActive(true); // パネルを表示

        string finalWinnerName = (player1Score == 2) ? "1P" : "2P";

        // テキストを設定
        roundWinnerText.text = $"{finalWinnerName} WIN!";
        roundScoreText.text = $"SCORE {player1Score} - {player2Score}";
        
        // NOTE: ゲーム終了後はNEXTボタン（roundResultPanel内にはない）か、手動でのリスタートが必要です。
        // （NEXTボタンはgameOverPanelにのみ実装されているため、リスタート処理は省略します）

    }
}