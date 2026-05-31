using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель слежения")]
    public Transform target;

    [Header("Текущий режим камеры")]
    public bool isFirstPerson = false; // Управляется на H
    public bool isLookingDown = false; // Управляется на V (только в FPV)

    [Header("Настройки 3-го лица")]
    public Vector3 thirdPersonOffset = new Vector3(0, 3f, -5f);
    public float lookHeightOffset = 1.5f;

    [Header("Настройки 1-го лица (FPV - Основная камера)")]
    public Vector3 firstPersonOffset = new Vector3(0, 0.2f, 0.5f);

    [Header("Настройки 1-го лица (Нижняя камера)")]
    // Смещение для нижней камеры (обычно её крепят на "пузо" дрона)
    public Vector3 downwardOffset = new Vector3(0, -0.2f, 0f); 
    // Насколько сильно камера наклонена вниз (90 = строго в пол)
    public float downwardPitchAngle = 90f; 

    void Update()
    {
        // 1. ГЛОБАЛЬНЫЙ ВИД: Смена 1-го и 3-го лица на кнопку H
        if (Input.GetKeyDown(KeyCode.H))
        {
            isFirstPerson = !isFirstPerson;
        }

        // 2. БОРТОВАЯ КАМЕРА: Смена ракурса на V (работает только от 1-го лица)
        if (Input.GetKeyDown(KeyCode.V) && isFirstPerson)
        {
            isLookingDown = !isLookingDown;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (isFirstPerson)
        {
            if (isLookingDown)
            {
                // === РЕЖИМ ОТ 1-ГО ЛИЦА (КАМЕРА ВНИЗ) ===
                // Ставим камеру в точку для нижней камеры
                transform.position = target.position + (target.rotation * downwardOffset);
                
                // Копируем поворот дрона, но дополнительно наклоняем камеру вниз по оси X
                transform.rotation = target.rotation * Quaternion.Euler(downwardPitchAngle, 0, 0);
            }
            else
            {
                // === РЕЖИМ ОТ 1-ГО ЛИЦА (КАМЕРА ВПЕРЕД) ===
                transform.position = target.position + (target.rotation * firstPersonOffset);
                transform.rotation = target.rotation;
            }
        }
        else
        {
            // === РЕЖИМ ОТ 3-ГО ЛИЦА ===
            float currentYaw = target.eulerAngles.y;
            Quaternion horizontalRotation = Quaternion.Euler(0, currentYaw, 0);

            transform.position = target.position + (horizontalRotation * thirdPersonOffset);

            Vector3 lookAtPoint = target.position + (Vector3.up * lookHeightOffset);
            transform.LookAt(lookAtPoint);
        }
    }

    // Обновим функцию для UI-кнопки (на всякий случай)
    public void ToggleCameraView()
    {
        isFirstPerson = !isFirstPerson;
    }
}