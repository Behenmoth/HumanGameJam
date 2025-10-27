using UnityEngine;
using UnityEngine.UI;

public class RemoteControl : MonoBehaviour
{
    public static RemoteControl instance;

    [Header("���w��{�^��")]
    public Button button1;
    public Button button2;
    public Button button3;

    [Header("UI�{��")]
    public GameObject ui;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        ui.SetActive(false);
    }

    //UI�̕\��
    public void OpenUI()
    {
        ui.SetActive(true);
    }

    public void OnButton1()
    {
        OnSelectLimit(1);
    }

    public void OnButton2()
    {
        OnSelectLimit(2);
    }

    public void OnButton3()
    {
        OnSelectLimit(3);
    }

    //����̒@�������w�肷��
    public void OnSelectLimit(int count)
    {
        Debug.Log($"����̒@���񐔂� {count} ��ɐݒ肵�܂���");
        ui.SetActive(false);

        // ����^�[���̔��e�@���񐔂�ݒ�
        BombManager.instance.AddBombCount(count);

    }

}
