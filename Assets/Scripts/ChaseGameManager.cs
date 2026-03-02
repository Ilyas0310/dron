using UnityEngine;
using UnityEngine.SceneManagement;

public class ChaseGameManager : MonoBehaviour
{
    public GameObject winPanel;

    void Start()
    {
        // Прячем панель победы при старте
        if (winPanel != null) winPanel.SetActive(false);
    }

    // Этот метод вызовем, когда врежемся во врага
    public void WinGame()
    {
        Debug.Log("Враг пойман!");
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}