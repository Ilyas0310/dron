using UnityEngine;
using TMPro;

public class LandingModeManager : MonoBehaviour
{
    [Header("Настройки режима")]
    [Tooltip("Сколько раз нужно успешно приземлиться для победы")]
    public int requiredLandings = 3;
    
    private int currentLandings = 0;

    [Header("Интерфейс")]
    public TextMeshProUGUI progressText; // Текст в углу экрана "Посадок: 0/3"
    public GameObject winPanel;          // Панель победы

    void Start()
    {
        currentLandings = 0;
        if (winPanel != null) winPanel.SetActive(false);
        UpdateUI();
    }

    // Этот метод будут вызывать сами машины, когда дрон на них сядет
    public void RegisterLanding()
    {
        currentLandings++;
        UpdateUI();

        if (currentLandings >= requiredLandings)
        {
            WinGame();
        }
    }

    private void UpdateUI()
    {
        if (progressText != null)
        {
            progressText.text = $"ПОСАДОК: {currentLandings} / {requiredLandings}";
        }
    }

    private void WinGame()
    {
        Debug.Log("Все посадки выполнены! Победа!");
        
        if (winPanel != null) winPanel.SetActive(true);

        // Безопасно отключаем управление у любого заспавненного дрона
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            DroneController dc = playerObj.GetComponent<DroneController>();
            if (dc != null) dc.enabled = false;

            AcroDroneController adc = playerObj.GetComponent<AcroDroneController>();
            if (adc != null) adc.enabled = false;
        }

        Time.timeScale = 0f; // Останавливаем время
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}