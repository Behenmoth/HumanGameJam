using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("�v���C���[�̏�����")]
    public int player1WinCount;
    public int player2WinCount;

    [Header("�A�C�e���̃C���x���g���[")]
    public List<int> player1ItemIdList = new List<int>();
    public List<int> player1ItemAmountList = new List<int>();

    public List<int> player2ItemIdList = new List<int>();
    public List<int> player2ItemAmountList = new List<int>();

    [Header("�A�C�e��")]
    public int maxItem;
    public int giveItem;

    [Header("���E���h��")]
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

    //���E���h���Ǘ����鏈��
    private void RoundManager()
    {
        while (currentRoundCount < roundCount)
        {
            //�v���C���[1�����������Ƃ�
            if (player1WinCount == winCount)
            {
                Player1Win();
            }

            //�v���C���[2�����������Ƃ�
            if (player2WinCount == winCount)
            {
                Player2Win();
            }

            //�A�C�e�����e�v���C���[�ɔz��
            GiveItems();
            currentRoundCount++;
        }
    }

    //�A�C�e�����e�v���C���[�ɔz�鏈��
    private void GiveItems()
    {
        for(int i = 0; i < giveItem; i++)
        {

        }
        Debug.Log("�A�C�e����z����");
    }


    //�v���C���[1�����������Ƃ��̏���
    private void Player1Win()
    {
        Debug.Log("�v���C���[1���������܂���");
    }

    //�v���C���[2�����������Ƃ��̏���
    private void Player2Win()
    {
        Debug.Log("�v���C���[2���������܂���");
    }

}
