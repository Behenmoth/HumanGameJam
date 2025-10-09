using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemSelectUI : MonoBehaviour
{
    [Header("ボタン設定")]
    // UI上の3つの選択ボタン
    public Button button1;
    public Button button2;
    public Button button3;
    // ※キャンセルボタンが必要な場合はコメントアウトを外す
    // public Button cancelButton;

    [Header("説明テキスト")]
    // プレイヤーに表示する説明文（選択内容などを伝える）
    public TMP_Text messageText;

    // 値を返すためのコールバック関数（呼び出し元に選択結果を返す）
    private Action<int> onSelected;

    public void Start()
    {
        Close();
    }
    // ===============================
    // ✅ UIを開くメソッド（ItemManagerなどから呼び出す）
    // ===============================
    public void Open(string message, Action<int> callback)
    {
        // --- ① すでにUIが開いていた場合に備えて、まず閉じて初期化 ---
        Close();

        // UIを表示状態にする
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);

        // 説明メッセージを設定
        if (messageText != null)
            messageText.text = message;

        // 呼び出し元から渡されたコールバックを保存
        onSelected = callback;

        // --- ② 各ボタンにクリックイベント（リスナー）を登録 ---
        // ボタンが押されたときにSelect()を呼び出すように設定
        button1.onClick.AddListener(() => Select(1));
        button2.onClick.AddListener(() => Select(2));
        button3.onClick.AddListener(() => Select(3));

        // ※キャンセルボタンを使う場合はこちらを有効に
        // if (cancelButton != null)
        //     cancelButton.onClick.AddListener(() => Cancel());
    }

    // ===============================
    // ✅ ボタンが押されたときに呼ばれる処理
    // ===============================
    private void Select(int value)
    {
        Debug.Log($"ItemSelectUI: 値 {value} が選択されました");

        // コールバックに選択した値を返す
        onSelected?.Invoke(value);

        // UIを閉じる
        Close();
    }

    // ===============================
    // ✅ キャンセル処理（必要な場合）
    // ===============================
    private void Cancel()
    {
        Debug.Log("ItemSelectUI: キャンセルされました");

        // -1 を返すことで「キャンセルされた」ことを呼び出し元に伝える
        onSelected?.Invoke(-1);

        // UIを閉じる
        Close();
    }

    // ===============================
    // ✅ UIを閉じる処理
    // ===============================
    public void Close()
    {
        // --- ③ 登録したリスナーをすべて解除（多重登録の防止） ---
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();

        // if (cancelButton != null)
        //     cancelButton.onClick.RemoveAllListeners();

        // UIを非表示にする
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);

        // コールバックの参照をクリアしてメモリリーク防止
        onSelected = null;
    }
}
