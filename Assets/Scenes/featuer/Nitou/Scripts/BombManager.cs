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

    [Header("���e��@������")]
    public bool bombClicked = false;

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

    //�c����
    public void SetLimitedClicks(int max)
    {
        Debug.Log($"�@����񐔂� {max} ��ɐ���");
        // �@���񐔂𐧌����郍�W�b�N��������
    }

    public void AddBombCount(int add)
    {
        bombCount += add;
        Debug.Log($"���e�J�E���g�� +{add} ���܂����B����: {bombCount}");
    }

    public void HideBombCountForOpponent()
    {
        Debug.Log("���肩�甚�e�J�E���g���B���܂���");
        // UI��ő���ɕ\�����Ȃ��Ȃǂ̏���������
    }

}
