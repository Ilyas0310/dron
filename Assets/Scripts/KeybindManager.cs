using UnityEngine;
using TMPro; // Для работы с текстом на кнопках

public class KeybindManager : MonoBehaviour
{
    public DroneController drone; // Ссылка на наш дрон (чтобы менять ему кнопки)

    [Header("Тексты на кнопках в меню")]
    public TextMeshProUGUI upText;
    public TextMeshProUGUI downText;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;

    private string actionToRebind = ""; // Какое действие мы сейчас переназначаем?

    void Start()
    {
        UpdateUI(); // При старте меню пишем на кнопках текущие клавиши
    }

    void Update()
    {
        // Если мы нажали на какую-то кнопку в меню и ждем ввода с клавиатуры
        if (actionToRebind != "")
        {
            if (Input.anyKeyDown)
            {
                // Перебираем все возможные кнопки на клавиатуре
                foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(k))
                    {
                        SaveNewKey(k);
                        break;
                    }
                }
            }
        }
    }

    // Эти методы мы будем вешать на OnClick() наших кнопок в интерфейсе
    public void StartRebindUp() { actionToRebind = "Up"; upText.text = "Нажмите клавишу..."; }
    public void StartRebindDown() { actionToRebind = "Down"; downText.text = "Нажмите клавишу..."; }
    public void StartRebindLeft() { actionToRebind = "Left"; leftText.text = "Нажмите клавишу..."; }
    public void StartRebindRight() { actionToRebind = "Right"; rightText.text = "Нажмите клавишу..."; }

    // Сохранение новой кнопки
    private void SaveNewKey(KeyCode newKey)
    {
        if (actionToRebind == "Up") { drone.thrustUpKey = newKey; PlayerPrefs.SetInt("key_up", (int)newKey); }
        else if (actionToRebind == "Down") { drone.thrustDownKey = newKey; PlayerPrefs.SetInt("key_down", (int)newKey); }
        else if (actionToRebind == "Left") { drone.yawLeftKey = newKey; PlayerPrefs.SetInt("key_left", (int)newKey); }
        else if (actionToRebind == "Right") { drone.yawRightKey = newKey; PlayerPrefs.SetInt("key_right", (int)newKey); }

        PlayerPrefs.Save(); // Жестко сохраняем на диск
        actionToRebind = ""; // Заканчиваем режим переназначения
        UpdateUI(); // Обновляем текст на кнопках
    }

    // Обновление надписей
    private void UpdateUI()
    {
        if (drone != null)
        {
            upText.text = drone.thrustUpKey.ToString();
            downText.text = drone.thrustDownKey.ToString();
            leftText.text = drone.yawLeftKey.ToString();
            rightText.text = drone.yawRightKey.ToString();
        }
    }
}