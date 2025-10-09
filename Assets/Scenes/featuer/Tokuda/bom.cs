using UnityEngine;

public class bom : MonoBehaviour
{
    private Game gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {// シーン内のGameManagerを見つける
        gameManager = FindObjectOfType<Game>();

        if (gameManager == null)
        {
            Debug.LogError("gameがシーンに見つかりません。");
        }

    }
    // マウスがコライダーを持つオブジェクトをクリックしたときに呼び出されるUnity関数
    void OnMouseDown()
    {
        Debug.Log("爆弾がクリックされました！");
        // GameManagerのクリック処理を呼び出す
        if (gameManager != null)
        {
            gameManager.HandlePushClick();
        }
    }
}
