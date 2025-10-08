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

    [Header("Item Card UI")]
    public Button[] itemCardButtons;         // 各アイテムカード

    [Header("Item Panel UI")]
    public GameObject itemPanel;            // 説明と選択肢を含むパネル全体
    public Image itemDescriptionImage;       // アイテムの説明を表示するImageコンポーネント
    public Button useItemButton;            // 「使う」ボタン
    public Button cancelItemButton;         // 「キャンセル」ボタン

    public Sprite[] itemDescriptionSprites; // アイテム画像
    
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
    public Button nextButton;                 // NEXTボタン

    [Header("Game Settings")]
    public int minStartCounter = 20;
    public int maxStartCounter = 40;

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

        // ★ ゲームオーバーパネルのボタンリスナーと初期非表示を追加
        nextButton.onClick.AddListener(OnNextButtonClicked);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

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
        currentCounter = Random.Range(minStartCounter, maxStartCounter + 1);
        counterText.text = currentCounter.ToString();
        isPlayer1Turn = (Random.Range(0, 2) == 0);
        StartTurn();
    }
    
    IEnumerator ShowRoundStartMessage(float delay)
    {
        // 操作を一時的にロック
        isClickingAllowed = false; 
        mainClickButton.interactable = false;
        passButton.interactable = false;
        
        // ★ ラウンド数とスコアを表示
        turnIndicatorText.text = $"ラウンド {currentRound} 開始！ スコア: {player1Score} - {player2Score}";

        // 待機
        yield return new WaitForSeconds(delay);

        // 待機後に実際のターンを開始
        StartTurn(); 
    }

    void StartTurn()
    {
        clicksMadeInTurn = 0;
        isClickingAllowed = true;
        mainClickButton.interactable = true;

 // ★ 追加: 前のターンで無効化されたPASSボタンを有効化して、操作を再開する
        passButton.interactable = true; 

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

        if (playerWhoLost)
        {
            player2Score++;
        }
        else
        {
            player1Score++;
        }

        turnIndicatorText.text = $"ラウンド終了！ スコア: {player1Score} - {player2Score}";

        if (player1Score == 2 || player2Score == 2)
        {
            EndGame();
            return;
        }

        currentRound++;
        StartCoroutine(WaitAndStartNextRound(3.0f));
    }

    IEnumerator WaitAndStartNextRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartRound();
    }
    public void OnNextButtonClicked()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // パネルを非表示にする
        }

        // ★ ゲームをリスタートする
        StartGame();

        // 必要に応じて、他のUI要素（例えばturnIndicatorText）も初期状態に戻す
        turnIndicatorText.text = "Game Start!"; // 例
    }

    void EndGame()
    {  // 全てのゲーム操作をロック
        isClickingAllowed = false;
        mainClickButton.interactable = false;
        passButton.interactable = false;

        // アイテムやルールパネルも閉じる（もし開いていた場合）
        if (itemPanel.activeSelf) itemPanel.SetActive(false);
        if (rulePanel.activeSelf) rulePanel.SetActive(false);


        // 勝者を判定
        string winnerName = (player1Score == 2) ? "1P" : "2P";
        string loserName = (player1Score == 2) ? "2P" : "1P"; // 負けたプレイヤー

        // WINNER表示
        winnerText.text = $"WINNER {winnerName}";

        // YOU DIEDと負け犬メッセージは固定テキスト
        youDiedText.text = "YOU DIED";
        loserMessageText.text = $"今日からお前の名前は負け犬だ。 ({loserName})"; // 誰が負けたか分かるように

        // ゲームオーバーパネルを表示
        gameOverPanel.SetActive(true);

    }
}