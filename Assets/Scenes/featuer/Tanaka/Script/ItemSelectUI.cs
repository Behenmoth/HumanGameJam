using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemSelectUI : MonoBehaviour
{
    [Header("ボタン設定")]
    public Button button1;
    public Button button2;
    public Button button3;
    // public Button cancelButton;

    [Header("説明テキスト")]
    public TMP_Text messageText;

    // 値を返すためのコールバック
    private Action<int> onSelected;

    // ✅ UIを開く（ItemManagerなどから呼ぶ）
    public void Open(string message, Action<int> callback)
    {
        // --- ① すでにUIが開いていたら閉じて初期化 ---
        Close();

        gameObject.SetActive(true);
        if (messageText != null)
            messageText.text = message;

        onSelected = callback;

        // --- ② リスナー登録 ---
        button1.onClick.AddListener(() => Select(1));
        button2.onClick.AddListener(() => Select(2));
        button3.onClick.AddListener(() => Select(3));

        // if (cancelButton != null)
        //     cancelButton.onClick.AddListener(() => Cancel());
    }

    private void Select(int value)
    {
        Debug.Log($"ItemSelectUI: 値 {value} が選択されました");
        onSelected?.Invoke(value); // ✅ 値を返す
        Close();
    }

    private void Cancel()
    {
        Debug.Log("ItemSelectUI: キャンセルされました");
        onSelected?.Invoke(-1);
        Close();
    }

    public void Close()
    {
        // --- ③ リスナー解除（多重登録防止） ---
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();
        // if (cancelButton != null)
        //     cancelButton.onClick.RemoveAllListeners();

        gameObject.SetActive(false);
        onSelected = null; // ✅ コールバック参照もクリア（安全）
    }
}
