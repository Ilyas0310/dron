using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    [Header("Префабы дронов (Перетащи из Project)")]
    public GameObject standardDronePrefab;
    public GameObject acroDronePrefab;

    [Header("Точка появления дрона")]
    public Transform spawnPoint; // Создай пустой объект на сцене и укажи его сюда

    [Header("Интерфейс выбора режима")]
    public GameObject selectionPanel; // Твоя UI панель с кнопками выбора

    [Header("Ссылка на скрипт камеры")]
    public CameraFollow cameraFollow; // Наш скрипт камеры, который должен следовать за дроном

    [Header("Эффекты сцены (Новое)")]
    [Tooltip("Перетащи сюда твой объект Global Volume (ThermalEffect) со сцены")]
    public GameObject thermalVolumeSceneObject; 

    void Start()
    {
        // При старте уровня полностью останавливаем время в игре, 
        // чтобы мир «замер», пока игрок выбирает режим
        Time.timeScale = 0f;

        // Показываем панель выбора
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
        }

        // Включаем курсор мыши, чтобы можно было нажать на кнопки
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Этот метод повесим на OnClick() кнопки "Обычный режим"
    public void SelectStandardMode()
    {
        SpawnDrone(standardDronePrefab);
    }

    // Этот метод повесим на OnClick() кнопки "Acro режим"
    public void SelectAcroMode()
    {
        SpawnDrone(acroDronePrefab);
    }

    private void SpawnDrone(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("Префаб дрона не назначен в DroneSpawner!");
            return;
        }

        // 1. Скрываем панель выбора режима
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }

        // Определяем позицию для спавна (если точка не задана, спавним в нулях сцены)
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        // 2. Создаем дрон на сцене из префаба
        GameObject spawnedDrone = Instantiate(prefabToSpawn, spawnPos, spawnRot);

        // 3. Перепривязываем камеру к новому созданному дрону
        if (cameraFollow != null)
        {
            // Передаем трансформ нового дрона в скрипт камеры
            cameraFollow.target = spawnedDrone.transform; 
        }

        // 4. Синхронизируем новый дрон с KeybindManager и передаем ссылку на тепловизор
        KeybindManager km = Object.FindFirstObjectByType<KeybindManager>();
        
        // Проверяем, заспавнился ли обычный дрон (компонент DroneController)
        DroneController dc = spawnedDrone.GetComponent<DroneController>();
        if (dc != null)
        {
            if (km != null) km.drone = dc;
            
            // ПЕРЕДАЧА ССЫЛКИ: Записываем объект тепловизора со сцены в заспавненный дрон
            dc.thermalVisionObject = thermalVolumeSceneObject; 
        }

        // Проверяем, заспавнился ли Acro дрон (компонент AcroDroneController)
        AcroDroneController adc = spawnedDrone.GetComponent<AcroDroneController>();
        if (adc != null)
        {
            if (km != null) km.acroDrone = adc;
            
            // Если в будущем добавишь логику тепловизора и для хардкорного Acro-режима,
            // просто раскомментируй строчку ниже:
            adc.thermalVisionObject = thermalVolumeSceneObject;
        }

        // 5. Запускаем время в игре обратно!
        Time.timeScale = 1f;

        // Если для управления симулятором мышь не нужна, можно залочить её обратно:
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        
        Debug.Log($"Успешно заспавнен объект: {spawnedDrone.name}. Ссылки синхронизированы. Игра запущена.");
    }
}