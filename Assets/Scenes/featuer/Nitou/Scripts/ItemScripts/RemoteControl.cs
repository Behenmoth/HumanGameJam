using UnityEngine;
using UnityEngine.UI;

public class RemoteControl : MonoBehaviour
{
    public static RemoteControl instance;

    [Header("数指定ボタン")]
    public Button button1;
    public Button button2;
    public Button button3;

    [Header("UI本体")]
    public GameObject ui;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        ui.SetActive(false);
    }

    //UIの表示
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

    //相手の叩く数を指定する
    public void OnSelectLimit(int count)
    {
        Debug.Log($"相手の叩く回数を {count} 回に設定しました");
        ui.SetActive(false);

        // 相手ターンの爆弾叩く回数を設定
        BombManager.instance.AddBombCount(count);

    }

}
