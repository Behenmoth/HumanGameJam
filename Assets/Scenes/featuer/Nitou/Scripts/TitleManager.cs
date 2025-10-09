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

    //�Q�[���X�^�[�g�{�^��
    public void GameStartButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    //�Q�[���I���{�^��
    public void GameQuitButton()
    {
        //�G�f�B�^�[�̏ꍇ
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        //�A�v���P�[�V�����̏ꍇ
#else
        Application.Quit();
#endif
    }
}
