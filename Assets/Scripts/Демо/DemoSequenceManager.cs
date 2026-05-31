using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DemoSequenceManager : MonoBehaviour
{
    [Header("Интерфейс")]
    public TextMeshProUGUI instructionText; // Крупный текст по центру или сверху
    public GameObject demoCompletePanel;    // Панель в конце демо-уровня

    [Header("Объекты для показа (Выключить на старте)")]
    public GameObject targetShowcase; // Обычный куб с тегом Target
    public GameObject chaseEnemy;     // Враг со скриптом EvaderAI

    private int currentStep = 0;
    private Transform player;
    private Rigidbody playerRb;

    // Переменные для правильного отображения кнопок
    private string keyUp, keyForward, keyThermal, keyShoot;

    void Start()
    {
        if (demoCompletePanel != null) demoCompletePanel.SetActive(false);
        
        // Загружаем названия кнопок из PlayerPrefs (чтобы текст в обучении был правильным)
        keyUp = ((KeyCode)PlayerPrefs.GetInt("key_up", (int)KeyCode.Space)).ToString();
        keyForward = ((KeyCode)PlayerPrefs.GetInt("key_forward", (int)KeyCode.W)).ToString();
        keyThermal = ((KeyCode)PlayerPrefs.GetInt("key_switch_view", (int)KeyCode.C)).ToString();
        
        // Кнопка стрельбы у нас зашита в Input Manager ("Fire1"), обычно это ЛКМ
        keyShoot = "ЛКМ"; 

        instructionText.text = "ВЫБЕРИТЕ ДРОН ДЛЯ НАЧАЛА ОБУЧЕНИЯ";
    }

    void Update()
    {
        // 1. Ждем, пока игрок заспавнит дрон
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) 
            {
                player = p.transform;
                playerRb = player.GetComponent<Rigidbody>();
                NextStep(); // Переходим к Уроку 1
            }
            return; 
        }

        // 2. Машина состояний (Фазы обучения)
        switch (currentStep)
        {
            case 1:
                instructionText.text = $"УРОК 1: Удерживайте [{keyUp}], чтобы набрать высоту (выше 3 метров).";
                if (player.position.y > 3f) NextStep();
                break;

            case 2:
                instructionText.text = $"УРОК 2: Используйте [{keyForward}] и мышь для полета вперед.\nНаберите скорость!";
                // Проверяем, что дрон летит достаточно быстро
                if (playerRb != null && playerRb.linearVelocity.magnitude > 8f) NextStep();
                break;

            case 3:
                instructionText.text = $"ОБОРУДОВАНИЕ: Нажмите [{keyThermal}], чтобы включить тепловизор.";
                if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("key_switch_view", (int)KeyCode.C))) NextStep();
                break;

            case 4:
                instructionText.text = $"РЕЖИМ МИШЕНЕЙ: Найдите цель и выстрелите ([{keyShoot}]).";
                
                // Включаем мишень, если она еще выключена
                if (targetShowcase != null && !targetShowcase.activeSelf) targetShowcase.SetActive(true);
                
                // Если мишень уничтожили (объект удален) - идем дальше
                if (targetShowcase == null) NextStep();
                break;

            case 5:
                instructionText.text = "РЕЖИМ ПЕРЕХВАТА: Найдите машину и врежьтесь в неё!";
                
                if (chaseEnemy != null && !chaseEnemy.activeSelf) chaseEnemy.SetActive(true);
                
                // Если врага догнали (он уничтожен или мы сами написали логику исчезновения)
                if (chaseEnemy == null) NextStep();
                break;

            case 6:
                instructionText.text = "ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА!";
                FinishDemo();
                break;
        }
    }

    private void NextStep()
    {
        currentStep++;
        Debug.Log($"Переход к этапу обучения: {currentStep}");
    }

    private void FinishDemo()
    {
        currentStep = 99; // Останавливаем логику
        
        if (demoCompletePanel != null) demoCompletePanel.SetActive(true);
        
        // Отключаем управление
        DroneController dc = player.GetComponent<DroneController>();
        if (dc != null) dc.enabled = false;
        AcroDroneController adc = player.GetComponent<AcroDroneController>();
        if (adc != null) adc.enabled = false;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Укажи точное название своей сцены меню
    }
}