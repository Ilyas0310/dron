using UnityEngine;

public class DroneWeapon : MonoBehaviour
{
    public float range = 100f;
    public Camera droneCamera;
    
    // Маска слоев (позволяет выбирать, во что можно попадать)
    public LayerMask hitLayers; 

    private TargetGameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<TargetGameManager>();

        // === РЕШЕНИЕ ПРОБЛЕМЫ ===
        // Если камера не назначена (потому что дрон заспавнился из префаба),
        // скрипт сам ищет на сцене активную главную камеру.
        if (droneCamera == null)
        {
            droneCamera = Camera.main; 
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Защита от ошибок: если камеры на сцене почему-то нет, не пытаемся стрелять
        if (droneCamera == null)
        {
            Debug.LogError("DroneWeapon: Не найдена Camera.main! Убедись, что у камеры на сцене стоит тег 'MainCamera'.");
            return;
        }

        RaycastHit hit;

        // Проверяем попадание с учетом маски слоев
        if (Physics.Raycast(droneCamera.transform.position, droneCamera.transform.forward, out hit, range, hitLayers))
        {
            Debug.Log("Попали в: " + hit.transform.name);

            // ВАРИАНТ 1: Попали в ЦЕЛЬ
            if (hit.transform.CompareTag("Target"))
            {
                Destroy(hit.transform.gameObject);
                if(gameManager != null) gameManager.TargetHit();

                // ВЫДАЕМ АЧИВКУ! 
                // "first_target" - это технический ключ для сохранения.
                AchievementManager.instance.UnlockAchievement
                (
                    "first_target", 
                    "Снайпер", 
                    "Уничтожьте свою первую мишень."
                );
            }
            // ВАРИАНТ 2: Попали в ЛОЖНУЮ цель
            else if (hit.transform.CompareTag("FalseTarget"))
            {
                Destroy(hit.transform.gameObject);
                if(gameManager != null) gameManager.RegisterMistake();
            }
        }
    }
}