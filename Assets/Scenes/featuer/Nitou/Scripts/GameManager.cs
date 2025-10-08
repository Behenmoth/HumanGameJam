using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    [Header("ラウンド数")]
    public int roundCount;
    public int currentRoundCount;
    public int winCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RoundManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ラウンドを管理する処理
    private void RoundManager()
    {
        while (currentRoundCount < roundCount)
        {
            //プレイヤー1が勝利したとき
            if (player1WinCount == winCount)
            {
                Player1Win();
            }

            //プレイヤー2が勝利したとき
            if (player2WinCount == winCount)
            {
                Player2Win();
            }

            //アイテムを各プレイヤーに配る
            GiveItems();
            currentRoundCount++;
        }
    }

    //アイテムを各プレイヤーに配る処理
    private void GiveItems()
    {
        for(int i = 0; i < giveItem; i++)
        {

        }
        Debug.Log("アイテムを配った");
    }


    //プレイヤー1が勝利したときの処理
    private void Player1Win()
    {
        Debug.Log("プレイヤー1が勝利しました");
    }

    //プレイヤー2が勝利したときの処理
    private void Player2Win()
    {
        Debug.Log("プレイヤー2が勝利しました");
    }

}
