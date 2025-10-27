using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttonmanager : MonoBehaviour
{
    //public GameObject roleBook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //roleBook.SetActive(false);
        //resultUi.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRoleBook()
    {
        //roleBook.SetActive(true);
    }

    public void OnCloseRoleBook()
    {
        //roleBook.SetActive(false);
 
    }

    public void OnCloseResultUI()
    {
        if (GameManager.instance.currentPlayerTurn == GameManager.PlayerTurn.Player1)
        {
            GameManager.instance.resultUI.SetActive(false);
            GameManager.instance.Player1Win();
        }
        else if (GameManager.instance.currentPlayerTurn == GameManager.PlayerTurn.Player2)
        {
            GameManager.instance.resultUI.SetActive(false);
            GameManager.instance.Player2Win();
        }
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void ReStart()
    {
        SceneManager.LoadScene("MaineScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
