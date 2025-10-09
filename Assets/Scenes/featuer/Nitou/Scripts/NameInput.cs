using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [Header("名前入力欄")]
    public TMP_InputField player1NameText;
    public TMP_InputField player2NameText;

    [Header("ゲームスタートボタン")]
    public Button gameStartButton;

    [Header("名前入力オブジェクト")]
    public GameObject nameInputObj;

    //名前を反映させる
    public void OnGameStart()
    {
        string player1Name = player1NameText.text;
        string player2name = player2NameText.text;

        if (string.IsNullOrWhiteSpace(player1Name) || string.IsNullOrWhiteSpace(player2name))
        {
            Debug.Log("名前を入力してください");
            return;
        }

        //GameManagerに名前を反映させる
        GameManager.instance.SetPlayerNames(player1Name, player2name);

        //背景を消してゲームスタート
        nameInputObj.SetActive(false);

        GameManager.instance.RoundManager();
    }
}
