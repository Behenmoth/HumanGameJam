using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ゲームスタートボタン
    public void GameStartButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    //ゲーム終了ボタン
    public void GameQuitButton()
    {
        //エディターの場合
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        //アプリケーションの場合
#else
        Application.Quit();
#endif
    }
}
