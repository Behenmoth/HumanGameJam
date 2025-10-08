using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    [Header("�A�C�e���g�p�̉�")]
    public bool canUseItems = false;

    //���e�̏�����
    public enum BombHolder {None,Player1,Player2}

    //�v���C���[�̃^�[��
    public enum PlayerTurn {None,Player1,Player2}

    [Header("���e�ێ�")]
    public BombHolder currentBombholder = BombHolder.None;

    [Header("�^�[��")]
    public PlayerTurn currentPlayerTurn = PlayerTurn.None;

    [Header("���E���h��")]
    public int roundCount;
    public int currentRoundCount;
    public int winCount;

    [Header("�{�^��")]
    public Button nextTurnButton;

    [Header("�e�L�X�gUI")]
    public TMP_Text roundText;
    public TMP_Text turnText;

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
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        currentRoundCount = 0;
        RoundManager();
    }

    // Update is called once per frame
    void Update()
    {
        TurnManager();
    }

    //���E���h���Ǘ����鏈��
    private void RoundManager()
    {
        if (currentRoundCount >= roundCount)
        {
            Debug.Log("�S���E���h�I��");
            return;
        }

        if (player1WinCount >= winCount)
        {
            Debug.Log("�v���C���[1");
        }

        if (player2WinCount >= winCount)
        {
            Debug.Log("�v���C���[2");
        }

        currentRoundCount++;

        roundText.text = $"Round {currentRoundCount}";

        //�A�C�e�����e�v���C���[�ɔz��
        GiveItems();

        //�ǂ��炩�̃v���C���[�ɔ��e��n��
        GiveBombs();

        //���e�̃J�E���g�������_���Ō��߂�
        BombManager.instance.StartBombCount();

        //�@�����J�E���g�����Z�b�g����
        BombManager.instance.ResetTrunBombClick();

        //���݂̃^�[����\��
        UpdateTurnUI();
    }

    //�A�C�e�����e�v���C���[�ɔz�鏈��
    private void GiveItems()
    {
        for(int i = 0; i < giveItem; i++)
        {

        }
        Debug.Log("�A�C�e����z����");
    }

    //�ǂ��炩�̃v���C���[�ɔ��e��n������
    private void GiveBombs()
    {
        int randomPlayer = UnityEngine.Random.Range(1, 3);

        if (randomPlayer == 1)
        {
            //�v���C���[1�ɔ��e����������
            currentBombholder = BombHolder.Player1;
            currentPlayerTurn = PlayerTurn.Player1;
            Debug.Log("�ŏ��̓v���C���[1");
        }
        else
        {
            //�v���C���[2�ɔ��e����������
            currentBombholder = BombHolder.Player2;
            currentPlayerTurn = PlayerTurn.Player2;
            Debug.Log("�ŏ��̓v���C���[2");
        }

        UpdateTurnUI();
    }

    //���e��n������
    public void PassBomb()
    {
        if (currentBombholder == BombHolder.Player1)
        {
            currentBombholder = BombHolder.Player2;
            Debug.Log("�v���C���[1����v���C���[2�֔��e��n����");
        }
        else if (currentBombholder == BombHolder.Player2) 
        {
            currentBombholder = BombHolder.Player1;
            Debug.Log("�v���C���[2����v���C���[1�֔��e��n����");
        }
    }

    //�e�v���C���[�̃^�[������
    private void TurnManager()
    {
        //nextTurnButton.interactable = false;
        //�A�C�e���g�p��1�񂾂��Ȃ�A�C�e�����g�p�\
        //if (ItemManager.instance.usedItems == true)
        //{
        //    canUseItems = true;
        //}

        //���e��1��ȏ�@���Ȃ���΂Ȃ�Ȃ�
        if (BombManager.instance.bombClicked == true)
        {
            nextTurnButton.interactable = true;
            Debug.Log("�l�N�X�g�^�[���{�^����������悤�ɂȂ���");
        }
        else
        {
            nextTurnButton.interactable = false;
        }
    }

    //�^�[���𑊎�ɓn������
    public void PassTurn()
    {

        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            currentPlayerTurn = PlayerTurn.Player2;
            PassBomb();
            Debug.Log("�v���C���[1����v���C���[2�փ^�[����n����");
        }
        else if (currentPlayerTurn == PlayerTurn.Player2)
        {
            currentPlayerTurn = PlayerTurn.Player1;
            PassBomb();
            Debug.Log("�v���C���[2����v���C���[1�փ^�[����n����");
        }

        //���e��@�����񐔂����Z�b�g
        BombManager.instance.ResetTrunBombClick();

        UpdateTurnUI();
    }

    //GameOver���̏���
    public void GameOver()
    {
        if (currentPlayerTurn == PlayerTurn.Player1)
        {
            Player2Win();
        }
        if (currentPlayerTurn == PlayerTurn.Player2)
        {
            Player1Win();
        }
    }

    //�v���C���[1�����������Ƃ��̏���
    private void Player1Win()
    {
        Debug.Log("�v���C���[1���������܂���");
        player1WinCount++;

        RoundManager();
    }

    //�v���C���[2�����������Ƃ��̏���
    private void Player2Win()
    {
        Debug.Log("�v���C���[2���������܂���");
        player2WinCount++;

        RoundManager();
    }

    //���݂̃^�[����\��
    private void UpdateTurnUI()
    {
        turnText.text = $"{currentPlayerTurn}";
    }
}
