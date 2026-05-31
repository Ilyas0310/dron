using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPV_HUDManager : MonoBehaviour
{
    [Header("Ссылки на UI элементы")]
    public RectTransform hudParent;    // Объект FollowHUD на Canvas
    public Image distanceBar;        // Картинка шкалы дистанции (Filled Vertical)
    public Image aimCircle;          // Кольцо прицела по центру экрана
    public Image targetIcon;          // Маленький ромбик, следующий за целью
    public TextMeshProUGUI statusText; // Текст "Удержание: 0%"

    [Header("Настройки визуала")]
    public Color badColor = Color.red;      // Цвет при потере цели
    public Color normalColor = Color.white;  // Нейтральный цвет (в зоне, но не держим угол)
    public Color goodColor = Color.green;   // Всё идеально

    private FollowModeManager modeManager;
    private Camera mainCamera;

    void Start()
    {
        modeManager = FindFirstObjectByType<FollowModeManager>();
        
        // ИСПРАВЛЕНО: Добавлено .gameObject
        if (hudParent != null) hudParent.gameObject.SetActive(false); // Прячем до спавна дрона
    }

    void Update()
    {
        if (modeManager == null) return;

        // Показываем HUD только когда дрон заспавнился
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            
            // ИСПРАВЛЕНО: Добавлено .gameObject
            if (mainCamera != null && hudParent != null) hudParent.gameObject.SetActive(true);
            return;
        }

        // Берем ТЕКУЩУЮ цель из менеджера
        FollowTarget activeTarget = modeManager.GetCurrentTarget();

        if (activeTarget == null)
        {
            UpdateUI_LostSignal();
            return;
        }

        // === ГЛАВНАЯ ЛОГИКА ВИЗУАЛИЗАЦИИ ===
        UpdateDistanceHUD(activeTarget);
        UpdateAimHUD(activeTarget);
        UpdateStatusText();
    }

    private void UpdateDistanceHUD(FollowTarget target)
    {
        if (distanceBar == null) return;

        // 1. Считаем реальную дистанцию (от камеры, т.к. HUD на камере)
        float dist = Vector3.Distance(mainCamera.transform.position, target.transform.position);

        // 2. Визуализируем дистанцию на шкале (от Min до Max)
        float fillAmount = 1f - Mathf.InverseLerp(target.minDistance, target.maxDistance, dist);
        distanceBar.fillAmount = Mathf.Clamp01(fillAmount);

        // 3. Красим шкалу: красная, если выходим за границы
        if (dist < target.minDistance || dist > target.maxDistance)
        {
            distanceBar.color = badColor;
        }
        else
        {
            distanceBar.color = goodColor;
        }
    }

    private void UpdateAimHUD(FollowTarget target)
    {
        if (aimCircle == null || targetIcon == null) return;

        // 1. Считаем угол
        Vector3 dirToTarget = (target.transform.position - mainCamera.transform.position).normalized;
        float angle = Vector3.Angle(mainCamera.transform.forward, dirToTarget);

        // 2. Визуализация маркера цели на экране
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.transform.position);

        // Если цель перед нами
        if (screenPoint.z > 0)
        {
            targetIcon.enabled = true;
            targetIcon.rectTransform.position = screenPoint;
            
            // Если цель невидима (вышла за границы кольца) - делаем маркер полупрозрачным
            Color iconColor = (angle > target.maxAngle) ? badColor : goodColor;
            iconColor.a = 0.5f; 
            targetIcon.color = iconColor;
        }
        else
        {
            targetIcon.enabled = false; // Цель сзади
        }

        // 3. Красим кольцо прицела: зеленое только если и дистанция, и угол соблюдены
        if (target.isBeingTracked)
        {
            aimCircle.color = goodColor;
        }
        else
        {
            // Красим в красный, если нарушен ИМЕННО УГОЛ (при соблюденной дистанции)
            float dist = Vector3.Distance(mainCamera.transform.position, target.transform.position);
            bool distOk = dist >= target.minDistance && dist <= target.maxDistance;
            aimCircle.color = (distOk && angle > target.maxAngle) ? badColor : normalColor;
        }
    }

    private void UpdateStatusText()
    {
        if (statusText == null) return;
        statusText.text = modeManager.progressText.text;
    }

    private void UpdateUI_LostSignal()
    {
        if (aimCircle != null) aimCircle.color = badColor;
        if (targetIcon != null) targetIcon.enabled = false;
        if (statusText != null) statusText.text = "СИГНАЛ ПОТЕРЯН";
        if (distanceBar != null) distanceBar.fillAmount = 0;
    }
}