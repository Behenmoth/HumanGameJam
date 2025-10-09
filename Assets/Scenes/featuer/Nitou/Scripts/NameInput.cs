using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [Header("���O���͗�")]
    public TMP_InputField player1NameText;
    public TMP_InputField player2NameText;

    [Header("�Q�[���X�^�[�g�{�^��")]
    public Button gameStartButton;

    [Header("���O���̓I�u�W�F�N�g")]
    public GameObject nameInputObj;

    //���O�𔽉f������
    public void OnGameStart()
    {
        string player1Name = player1NameText.text;
        string player2name = player2NameText.text;

        if (string.IsNullOrWhiteSpace(player1Name) || string.IsNullOrWhiteSpace(player2name))
        {
            Debug.Log("���O����͂��Ă�������");
            return;
        }

        //GameManager�ɖ��O�𔽉f������
        GameManager.instance.SetPlayerNames(player1Name, player2name);

        //�w�i�������ăQ�[���X�^�[�g
        nameInputObj.SetActive(false);

        GameManager.instance.RoundManager();
    }
}
