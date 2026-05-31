using UnityEngine;
using TMPro;

public class FollowModeManager : MonoBehaviour
{
    [Header("Настройки режима")]
    [Tooltip("Сколько всего секунд нужно непрерывно/суммарно удерживать цель?")]
    public float requiredTrackingTime = 10f;
    
    [Header("Список целей")]
    [Tooltip("Перетащи сюда все объекты с FollowTarget. Можно добавить хоть 10 штук.")]
    public FollowTarget[] targets; 

    [Header("Интерфейс")]
    public TextMeshProUGUI progressText; // Текст "Прогресс: 0%"
    public GameObject winPanel;

    private float currentTrackingTime = 0f;
    private bool isModeActive = true;
    private Transform playerTransform;
    private Camera mainCamera;

    void Start()
    {
        currentTrackingTime = 0f;
        if (winPanel != null) winPanel.SetActive(false);
    }

    void Update()
    {
        if (!isModeActive) return;

        // Динамический поиск игрока (раз уж мы спавним его кнопкой)
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
            mainCamera = Camera.main; // Берем камеру дрона для точного прицела
            return; // Ждем следующий кадр, пока дрон не появится
        }

        bool isTrackingAny = false;

        // Проверяем каждую цель из нашего списка
        foreach (FollowTarget target in targets)
        {
            if (target == null) continue;

            if (CheckTrackingLogic(target))
            {
                target.isBeingTracked = true;
                isTrackingAny = true; // Мы успешно держим хотя бы одну цель в прицеле
            }
            else
            {
                target.isBeingTracked = false;
            }
        }

        // Логика таймера
        if (isTrackingAny)
        {
            currentTrackingTime += Time.deltaTime;
            UpdateUI();

            if (currentTrackingTime >= requiredTrackingTime)
            {
                WinGame();
            }
        }
        else
        {
            // Опционально: можно сделать так, чтобы прогресс падал, если потерял цель
            // currentTrackingTime -= Time.deltaTime * 0.5f; 
            // currentTrackingTime = Mathf.Max(0, currentTrackingTime);
            // UpdateUI();
        }
    }

    public FollowTarget GetCurrentTarget()
    {
        if (targets == null || targets.Length == 0) return null;
    
        // В этом режиме для простоты HUDа берем первую цель. 
        // На будущее можно сделать поиск ближайшей.
        return targets[0]; 
    }

    private bool CheckTrackingLogic(FollowTarget target)
    {
        if (mainCamera == null) return false;

        // 1. Проверяем дистанцию (от дрона до цели)
        float distance = Vector3.Distance(playerTransform.position, target.transform.position);
        if (distance < target.minDistance || distance > target.maxDistance)
        {
            return false; // Слишком далеко или слишком близко
        }

        // 2. Проверяем угол (смотрит ли камера прямо на цель)
        // Вектор направления от камеры к цели
        Vector3 directionToTarget = (target.transform.position - mainCamera.transform.position).normalized;
        
        // Угол между тем, куда "смотрит" камера, и тем, где находится цель
        float angle = Vector3.Angle(mainCamera.transform.forward, directionToTarget);

        if (angle > target.maxAngle)
        {
            return false; // Цель не в прицеле (вышла за границы экрана)
        }

        return true; // Дистанция и угол идеальны!
    }

    private void UpdateUI()
    {
        if (progressText != null)
        {
            // Считаем проценты для красивого отображения
            int percent = Mathf.FloorToInt((currentTrackingTime / requiredTrackingTime) * 100f);
            progressText.text = $"УДЕРЖАНИЕ ЦЕЛИ: {percent}%";
        }
    }

    private void WinGame()
    {
        isModeActive = false;
        
        // Выключаем кольца на всех машинах
        foreach (FollowTarget target in targets)
        {
            if (target != null) target.GetComponent<LineRenderer>().enabled = false;
        }

        // === ВЫДАЕМ НОВУЮ АЧИВКУ! ===
        if (AchievementManager.instance != null)
        {
            AchievementManager.instance.UnlockAchievement(
                "first_follow_win", 
                "Глаз в небе", 
                "Впервые успешно завершите режим слежения за целью."
        );
        }

        if (winPanel != null) winPanel.SetActive(true);

        // Отключаем управление дроном
        DroneController dc = playerTransform.GetComponent<DroneController>();
        if (dc != null) dc.enabled = false;

        AcroDroneController adc = playerTransform.GetComponent<AcroDroneController>();
        if (adc != null) adc.enabled = false;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
}