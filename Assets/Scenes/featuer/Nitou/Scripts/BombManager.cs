using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public int forcedClickLimit = -1;

    [Header("�e�L�X�gUI")]
    public TMP_Text bombCountText;

    [Header("�N���b�N����p�J����")]
    public Camera mainCamera;

    [Header("���O���͉��")]
    public GameObject nameInputObj;

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

    //�N���b�N�����Ƃ��̏���
    public void OnClickBomb(InputAction.CallbackContext callbackContext)
    {
        //�����ꂽ���̂ݎ��s����
        if (!callbackContext.performed)
        {
            return;
        }

        //���O���͉�ʂ��o�Ă���Ƃ��͉������Ȃ�
        if (nameInputObj != null && nameInputObj.activeSelf)
        {
            return;
        }

        //�A�C�e��UI���o�Ă���Ƃ��͉������Ȃ�
        if(ItemUIManager.instance != null && ItemUIManager.instance.itemUI.activeSelf)
        {
            return;
        }

        //�N���b�N�����ꏊ��Ray���΂�
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Ray��������������s
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Ray�����e�Ƀq�b�g���Ă��������s
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                //�ݒ肵���C�x���g�����s
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
        //���˂��g�p���ꂽ��
        if (forcedClickLimit > 0)
        {
            currentBombClickCount++;
            currentBombCount--;

            //�w��񐔒@�����Ƃ�
            if (currentBombClickCount >= forcedClickLimit)
            {
                Debug.Log("���ˌ��ʏI��");
                
                bombClicked = true;
                forcedClickLimit = -1;
                GameManager.instance.PassTurn();
            }
        }
        //�ʏ���
        else
        {
            //���e��@����񐔂ɏ����݂���
            maxBombClickCount = 3;

            if (currentBombClickCount >= maxBombClickCount)
            {
                Debug.Log("����ȏ㔚�e�͒@���Ȃ�");
                return;
            }

            bombClicked = true;
            currentBombClickCount++;
            currentBombCount--;
            Debug.Log("���݂̃J�E���g����" + currentBombCount);
        }
        

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

    public void SetLimitedClicks(int max)
    {
        forcedClickLimit = max;
        maxBombClickCount = forcedClickLimit;
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
