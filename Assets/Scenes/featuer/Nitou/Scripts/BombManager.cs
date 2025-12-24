using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public struct InjectionState
{
    public bool active;
    public bool isInjectionFinish;
    public int limit;

    //状態をリセット
    public void Clear()
    {
        active = false;
        limit = -1;
        isInjectionFinish = false;
    }

    //叩く固定数をセットする
    public void Set(int value)
    {
        active = true;
        limit = value;
        isInjectionFinish = false;
    }
}

public class BombManager : MonoBehaviour
{
    public static BombManager instance;

    [Header("爆弾のカウント数")]
    public int bombCount;
    public int currentBombCount = 0;
    public int halfBombCount;

    [Header("爆弾のbool")]
    public bool bombClicked = false;
    public bool hasHalfBombCount = false;
    public bool isInjectionActive = false;
    public bool isDriverActive = false;

    [Header("爆弾を叩く上限")]
    public int maxBombClickCount;
    public int currentBombClickCount;
    public int forcedClickLimit = -1;

    [Header("テキストUI")]
    public TMP_Text bombCountText;

    [Header("クリック判定用カメラ")]
    public Camera mainCamera;

    [Header("名前入力画面")]
    public GameObject nameInputObj;

    [Header("注射器用のプレイヤーの状態")]
    public InjectionState useInjectionPlayer1;
    public InjectionState useInjectionPlayer2;

    [Header("ドライバーを使用したターン")]
    public int useDriver = -1;
    public GameManager.PlayerTurn useDriverTurn = GameManager.PlayerTurn.None;

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

        mainCamera = Camera.main;
    }

    //クリックしたときの処理
    public void OnClickBomb(InputAction.CallbackContext callbackContext)
    {
        //押された時のみ実行する
        if (!callbackContext.performed)
        {
            return;
        }

        //名前入力画面が出ているときは何もしない
        if (nameInputObj != null && nameInputObj.activeSelf)
        {
            return;
        }

        //アイテムUIが出ているときは何もしない
        if(ItemUIManager.instance != null && ItemUIManager.instance.itemUI.activeSelf)
        {
            return;
        }

        //クリックした場所にRayを飛ばす
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Rayが当たったら実行
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Rayが爆弾にヒットしていた時実行
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                //設定したイベントを実行
                BombClick();
            }
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

        //爆弾のカウントの半分の値を保存
        halfBombCount = (bombCount % 2 == 0) ? bombCount / 2 : (bombCount - 1) / 2;

        UpdateBombCount();
    }

    //爆弾のカウントを減らす
    public void BombClick()
    {
        //現在のターン
        var turn = GameManager.instance.currentPlayerTurn;

        //プレイヤー1の場合
        if (turn == GameManager.PlayerTurn.Player1 && useInjectionPlayer2.active) 
        {
            InjectionEffect(ref useInjectionPlayer2);
        }
        //プレイヤー2の場合
        else if (turn == GameManager.PlayerTurn.Player2 && useInjectionPlayer1.active)
        {
            InjectionEffect(ref useInjectionPlayer1);
        }
        //通常時
        else
        {
            NormalBombClick();
        }

        Debug.Log("現在のカウント数は" + currentBombCount);

        //カウントが半分になればアイテムを配布
        if (!hasHalfBombCount && halfBombCount >= currentBombCount)
        {
            hasHalfBombCount = true;
            GiveItemToCurrentPlayer();
        }

        //カウントが0以下になれば爆発する
        if (currentBombCount <= 0)
        {
            currentBombCount = 0;
            Debug.Log("爆弾が爆発した");
            bombClicked = false;
            hasHalfBombCount = false;
            GameManager.instance.GameOver();
            return;
        }

        UpdateBombCount();
    }

    //通常の爆弾を叩く処理
    private void NormalBombClick()
    {
        //叩ける回数に上限を設ける
        maxBombClickCount = 3;

        if (currentBombClickCount >= maxBombClickCount)
        {
            Debug.Log("これ以上爆弾は叩けない");
            return;
        }

        bombClicked = true;
        currentBombClickCount++;
        currentBombCount--;
    }

    //現在の爆弾のカウント数を表示する
    public void UpdateBombCount()
    {
        //ドライバーを使用したとき
        if (useDriver > 0 && useDriverTurn != GameManager.instance.currentPlayerTurn) 
        {
            Debug.Log("<color=yellow>ドライバーの効果適用</color>");
            isDriverActive = true;
            bombCountText.text = "  ";
        }
        //通常時
        else
        {
            bombCountText.text = $"{currentBombCount}";
        }
    }

    //ターン終了時に叩いたカウントをリセットする
    public void ResetTrunBombClick()
    {
        //現在のターンを取得
        var turn = GameManager.instance.currentPlayerTurn;

        //注射の効果を終了する
        //プレイヤー1の場合
        if (turn == GameManager.PlayerTurn.Player1 && useInjectionPlayer2.isInjectionFinish)  
        {
            Debug.Log("注射効果終了");
            useInjectionPlayer2.Clear();
        }
        //プレイヤー2の場合
        else if(turn == GameManager.PlayerTurn.Player2 && useInjectionPlayer1.isInjectionFinish)
        {
            Debug.Log("注射効果終了");
            useInjectionPlayer1.Clear();
        }

        //ドライバーの効果を終了する
        if (isDriverActive)
        {
            Debug.Log("ドライバー効果終了");
            useDriver = -1;
            isDriverActive = false;
            useDriverTurn = GameManager.PlayerTurn.None;
        }
        currentBombClickCount = 0;
        bombClicked = false;
        Debug.Log("爆弾を叩いた数をリセットしました");
    }

    //半分以下になったターンのプレイヤーにアイテムを配布する処理
    private void GiveItemToCurrentPlayer()
    {
        Debug.Log("爆弾のカウントが半分になった");

        //現在のターンは取得
        var currentTurn = GameManager.instance.currentPlayerTurn;
        PlayerInventry targetInventry = null;

        if (currentTurn == GameManager.PlayerTurn.Player1) 
        {
            targetInventry = GameManager.instance.player1Inventory;
        }
        else if (currentTurn == GameManager.PlayerTurn.Player2)
        {
            targetInventry = GameManager.instance.player2Inventory;
        }

        //アイテムを配布
        ItemDistribution.instance.GiveRandomItems(targetInventry, 1);
    }

    //注射器の処理
    //注射器使用時叩く回数を固定
    public void SetLimitedClicks(int max,GameManager.PlayerTurn injectionTurn)
    {
        //プレイヤー1が注射器を使用した場合
        if (injectionTurn == GameManager.PlayerTurn.Player1)
        {
            useInjectionPlayer1.Set(max);
        }
        //プレイヤー2が注射器を使用した場合
        else if (injectionTurn == GameManager.PlayerTurn.Player2) 
        {
            useInjectionPlayer2.Set(max);
        }
    }

    //注射器の効果発動
    private void InjectionEffect(ref InjectionState injection)
    {
        //叩ける回数を指定回数にする
        maxBombClickCount = injection.limit;

        if (currentBombClickCount >= maxBombClickCount)
        {
            Debug.Log("これ以上爆弾は叩けない");
            return;
        }

        currentBombClickCount++;
        currentBombCount--;

        //指定回数叩いたとき
        if (currentBombClickCount >= maxBombClickCount)
        {
            bombClicked = true;
            injection.isInjectionFinish = true;
        }
        else
        {
            bombClicked = false;
        }
    }
    //注射器を使用されているか判定
    private bool UseInjectionPlayer(ref InjectionState injectionState)
    {
        //現在のターンを取得
        var turn = GameManager.instance.currentPlayerTurn;

        //プレイヤー1のターン時プレイヤー2が注射器を使用している
        if(turn == GameManager.PlayerTurn.Player1 && useInjectionPlayer2.active)
        {
            injectionState = useInjectionPlayer2;
            return true;
        }
        //プレイヤー2のターン時プレイヤー1が注射器を使用している
        if(turn == GameManager.PlayerTurn.Player2 && useInjectionPlayer1.active)
        {
            injectionState = useInjectionPlayer1;
            return true;
        }

        //どちらも注射器を使用していない
        return false;
    }

    //リモコンの処理
    //カウントを増やす
    public void AddBombCount(int add)
    {
        currentBombCount += add;
        //カウント数は最大値より大きくしない
        if (currentBombCount >= bombCount)
        {
            currentBombCount = bombCount;
        }
        //カウントUIを更新
        UpdateBombCount();
        Debug.Log($"爆弾カウントを +{add} しました。現在: {bombCount}");
    }

    //ドライバーの処理
    //カウント数を隠す
    public void HideBombCountForOpponent(GameManager.PlayerTurn remoteControlTurn)
    {
        Debug.Log("相手から爆弾カウントを隠しました");
        useDriver = 1;
        //ドライバーを使用したターンを保存
        useDriverTurn = remoteControlTurn;
        UpdateBombCount();
    }
}
