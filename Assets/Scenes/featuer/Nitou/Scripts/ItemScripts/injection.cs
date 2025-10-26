using UnityEngine;
using UnityEngine.UI;

public class injection : MonoBehaviour
{
    public static injection instance;

    [Header("数指定ボタン")]
    public Button button1;
    public Button button2;
    public Button button3;

    [Header("交代表示")]
    public GameObject p1to2;
    public GameObject p2to1;

    [Header("UI本体")]
    public GameObject ui;

    [Header("注射を使用したターン")]
    public GameManager.PlayerTurn useInjectionTurn = GameManager.PlayerTurn.None;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        ui.SetActive(false);
    }

    //UIの表示
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

    //相手の叩く数を指定する
    public void OnSelectLimit(int count)
    {
        Debug.Log($"相手の叩く回数を {count} 回に設定しました");
        ui.SetActive(false);

        // 相手ターンの爆弾叩く回数を設定
        BombManager.instance.SetLimitedClicks(count,useInjectionTurn);

    }

    public void OnCloseChange()
    {
        p1to2.SetActive(false);
        p2to1.SetActive(false);
    }

}
