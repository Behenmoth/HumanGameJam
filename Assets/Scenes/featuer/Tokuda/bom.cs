using UnityEngine;

public class bom : MonoBehaviour
{
    private Game gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {// �V�[������GameManager��������
        gameManager = FindObjectOfType<Game>();

        if (gameManager == null)
        {
            Debug.LogError("game���V�[���Ɍ�����܂���B");
        }

    }
    // �}�E�X���R���C�_�[�����I�u�W�F�N�g���N���b�N�����Ƃ��ɌĂяo�����Unity�֐�
    void OnMouseDown()
    {
        Debug.Log("���e���N���b�N����܂����I");
        // GameManager�̃N���b�N�������Ăяo��
        if (gameManager != null)
        {
            gameManager.HandlePushClick();
        }
    }
}
