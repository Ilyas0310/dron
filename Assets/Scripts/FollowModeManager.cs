using UnityEngine;
using TMPro;

public class FollowModeManager : MonoBehaviour
{
    [Header("Дистанция (настраивается рандомно в Start)")]
    public float minRange; 
    public float maxRange;
    
    [Header("Условия победы")]
    public float requiredTime = 10f; // Сколько секунд нужно удержать цель
    public float lookAngleThreshold = 20f; // Допуск "прицела" в градусах

    [Header("Ссылки")]
    public Transform target;
    public GameObject zoneVisual; // Наша полупрозрачная сфера
    public TextMeshProUGUI progressText;
    public CameraFollow camScript;

    private float currentFixationTime = 0f;
    private float activeMinDist;
    private float activeMaxDist;

    void Start()
    {
        // 1. Рандомно определяем коридор дистанции
        // Например: от 3 до 7 или от 12 до 18 метров
        float startPoint = Random.Range(3f, 15f);
        float width = Random.Range(3f, 6f);
        activeMinDist = startPoint;
        activeMaxDist = startPoint + width;

        // 2. Рандомно выбираем вид (H в CameraFollow)
        if (camScript != null)
            camScript.isFirstPerson = Random.value > 0.5f;

        // 3. Подгоняем визуал сферы под внешнюю границу
        if (zoneVisual != null)
            zoneVisual.transform.localScale = Vector3.one * (activeMaxDist * 2f);
            
        Debug.Log($"Режим запущен! Дистанция: {activeMinDist:F1} - {activeMaxDist:F1} м.");
    }

    void Update()
    {
        if (target == null) return;

        // Вычисляем расстояние
        float distance = Vector3.Distance(transform.position, target.position);
        
        // Проверяем направление (смотрим ли мы на цель)
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToTarget);

        bool inDistance = distance >= activeMinDist && distance <= activeMaxDist;
        bool inSight = angle <= lookAngleThreshold;

        // Если оба условия выполнены — прогресс идет
        if (inDistance && inSight)
        {
            currentFixationTime += Time.deltaTime;
            // Можно менять цвет сферы на зеленый, когда мы в зоне
        }

        // UI
        float progressPercent = (currentFixationTime / requiredTime) * 100f;
        if (progressText != null)
            progressText.text = $"Фиксация: {Mathf.Clamp(progressPercent, 0, 100):F0}%";

        if (progressPercent >= 100f)
        {
            Debug.Log("ЦЕЛЬ ЗАФИКСИРОВАНА! ПОБЕДА!");
            this.enabled = false; // Выключаем скрипт после победы
        }
        if (progressPercent >= 100f)
        {
            Debug.Log("ЦЕЛЬ ЗАФИКСИРОВАНА! ПОБЕДА!");
            
            // Ищем наш VictoryManager на сцене и запускаем остановку игры
            VictoryManager.instance.FinishLevel();
            
            this.enabled = false; 
        }
    }
}