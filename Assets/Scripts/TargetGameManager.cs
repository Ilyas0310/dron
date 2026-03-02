using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TargetGameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI targetsLeftText;
    public TextMeshProUGUI accuracyText; // <--- НОВОЕ: Поле для вывода точности
    public GameObject winPanel;

    private int totalTargetsAtStart; // Сколько всего было правильных целей
    private int currentTargets;      // Сколько осталось
    private int mistakes = 0;        // <--- НОВОЕ: Счетчик ошибок

    void Start()
    {
        // Считаем, сколько целей было изначально
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        totalTargetsAtStart = targets.Length;
        currentTargets = totalTargetsAtStart;

        UpdateUI();
        
        if(winPanel != null) winPanel.SetActive(false);
    }

    // Этот метод вызываем при попадании в ПРАВИЛЬНУЮ цель
    public void TargetHit()
    {
        currentTargets--;
        UpdateUI();

        if (currentTargets <= 0)
        {
            Win();
        }
    }

    // <--- НОВОЕ: Этот метод вызываем при попадании в ЛОЖНУЮ цель
    public void RegisterMistake()
    {
        mistakes++;
        Debug.Log("Ошибка! Всего ошибок: " + mistakes);
        // Тут можно добавить звук ошибки или красную вспышку на экране
    }

    void UpdateUI()
    {
        if (targetsLeftText != null)
            targetsLeftText.text = "Целей осталось: " + currentTargets;
    }

    void Win()
    {
        Debug.Log("Все цели уничтожены!");
        if (winPanel != null) 
        {
            winPanel.SetActive(true);
            
            // <--- НОВОЕ: Расчет точности
            CalculateAndShowAccuracy();
        }
    }

    void CalculateAndShowAccuracy()
    {
        // Всего выстрелов (успешных + ошибочных)
        float totalHits = totalTargetsAtStart + mistakes;

        // Формула: (Правильные / Всего) * 100
        float accuracy = (totalTargetsAtStart / totalHits) * 100f;

        // Округляем до целого или 1 знака после запятой
        if (accuracyText != null)
        {
            accuracyText.text = "Точность: " + accuracy.ToString("F1") + "%";
            
            // Меняем цвет текста: если 100% - зеленый, если меньше - красный
            if (mistakes == 0) 
                accuracyText.color = Color.green;
            else 
                accuracyText.color = Color.red;
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}