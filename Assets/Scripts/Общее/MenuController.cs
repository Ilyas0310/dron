using UnityEngine;
using UnityEngine.SceneManagement; // Нужно для загрузки сцен

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject settingsPanel;

    // Метод для кнопки "Играть" (открывает выбор уровня)
    public void OpenLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // Метод для кнопки "Настройки"
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Метод для кнопок "Назад"
    public void BackToMenu()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // Метод для загрузки уровня по имени сцены
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Метод для выхода
    public void QuitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }
}