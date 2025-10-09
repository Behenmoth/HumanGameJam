using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BombManager : MonoBehaviour
{
    public static BombManager instance;

    [Header("���e�̃J�E���g��")]
    public int bombCount;
    public int currentBombCount = 0;
    public int halfBombCount;

    [Header("���e��bool")]
    public bool bombClicked = false;
    public bool hasHalfBombCount = false;

    [Header("���e��@�����")]
    public int maxBombClickCount;
    public int currentBombClickCount;

    [Header("���e��@���{�^��")]
    public Button bombClickButton;

    [Header("�e�L�X�gUI")]
    public TMP_Text bombCountText;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //���e�̃J�E���g���������_���Ō��߂�
    public void StartBombCount()
    {
        bombCount = UnityEngine.Random.Range(20, 41);
        currentBombCount = bombCount;

        //���e�̃J�E���g�̔����̒l��ۑ�
        halfBombCount = (bombCount % 2 == 0) ? bombCount / 2 : (bombCount - 1) / 2;

        UpdateBombCount();
    }

    //���e�̃J�E���g�����炷
    public void BombClick()
    {
        currentBombClickCount++;
        //���e��@����񐔂ɏ����݂���
        if (currentBombClickCount == maxBombClickCount)
        {
            bombClickButton.interactable = false;
            Debug.Log("����ȏ㔚�e�͒@���Ȃ�");
        }

        bombClicked = true;
        currentBombCount--;
        Debug.Log("���݂̃J�E���g����" + currentBombCount);

        //�J�E���g�������ɂȂ�΃A�C�e����z�z
        if (!hasHalfBombCount && halfBombCount >= currentBombCount)
        {
            hasHalfBombCount = true;
            GiveItemToCurrentPlayer();
        }

        //�J�E���g��0�ȉ��ɂȂ�Δ�������
        if (currentBombCount <= 0)
        {
            currentBombCount = 0;
            Debug.Log("���e����������");

            GameManager.instance.GameOver();
        }

        UpdateBombCount();
    }

    //���݂̔��e�̃J�E���g����\������
    private void UpdateBombCount()
    {
        bombCountText.text = $"{currentBombCount}";
    }

    //�^�[���I�����ɒ@�����J�E���g�����Z�b�g����
    public void ResetTrunBombClick()
    {
        currentBombClickCount = 0;
        bombClickButton.interactable = true;
        bombClicked = false;
        Debug.Log("���e��@�����������Z�b�g���܂���");
    }

    //�����ȉ��ɂȂ����^�[���̃v���C���[�ɃA�C�e����z�z���鏈��
    private void GiveItemToCurrentPlayer()
    {
        Debug.Log("���e�̃J�E���g�������ɂȂ���");

        //���݂̃^�[���͎擾
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

        //�A�C�e����z�z
        ItemDistribution.instance.GiveRandomItems(targetInventry, 1);
    }

    //�c����
    public void SetLimitedClicks(int max)
    {
        Debug.Log($"�@����񐔂� {max} ��ɐ���");
        // �@���񐔂𐧌����郍�W�b�N��������
    }

    public void AddBombCount(int add)
    {
        currentBombCount += add;
        Debug.Log($"���e�J�E���g�� +{add} ���܂����B����: {bombCount}");
    }

    public void HideBombCountForOpponent()
    {
        Debug.Log("���肩�甚�e�J�E���g���B���܂���");
        // UI��ő���ɕ\�����Ȃ��Ȃǂ̏���������
    }

}
