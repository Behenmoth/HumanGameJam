using UnityEngine;
using UnityEngine.UI;

public class injection : MonoBehaviour
{
    public static injection instance;

    [Header("���w��{�^��")]
    public Button button1;
    public Button button2;
    public Button button3;

    [Header("���\��")]
    public GameObject p1to2;
    public GameObject p2to1;

    [Header("UI�{��")]
    public GameObject ui;

    [Header("���˂��g�p�����^�[��")]
    public GameManager.PlayerTurn useInjectionTurn = GameManager.PlayerTurn.None;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        ui.SetActive(false);
    }

    //UI�̕\��
    public void OpenUI(GameManager.PlayerTurn useTurn)
    { 
        ui.SetActive(true);
        useInjectionTurn = useTurn;
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
        BombManager.instance.SetLimitedClicks(count,useInjectionTurn);

    }

    public void OnCloseChange()
    {
        p1to2.SetActive(false);
        p2to1.SetActive(false);
    }

}
