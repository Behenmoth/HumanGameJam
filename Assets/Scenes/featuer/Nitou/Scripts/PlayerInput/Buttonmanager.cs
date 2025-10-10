using UnityEngine;

public class Buttonmanager : MonoBehaviour
{
    public GameObject roleBook;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roleBook.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRoleBook()
    {
        roleBook.SetActive(true);
    }

    public void OnCloseRoleBook()
    {
        roleBook.SetActive(false);
    }
}
