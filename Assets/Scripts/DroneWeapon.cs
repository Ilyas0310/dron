using UnityEngine;

public class DroneWeapon : MonoBehaviour
{
    public float range = 100f;
    public Camera droneCamera;
    
    // ДОБАВИЛИ: Маска слоев (позволяет выбирать, во что можно попадать)
    public LayerMask hitLayers; 

    private TargetGameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<TargetGameManager>();
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
            }
            // ВАРИАНТ 2: Попали в ЛОЖНУЮ цель (НОВОЕ)
            else if (hit.transform.CompareTag("FalseTarget"))
            {
                // Мы тоже уничтожаем ложную цель, чтобы она не мешала
                Destroy(hit.transform.gameObject);
                
                // Но сообщаем менеджеру об ошибке
                if(gameManager != null) gameManager.RegisterMistake();
            }
        }
    }
}