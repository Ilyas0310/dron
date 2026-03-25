using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель слежения")]
    public Transform target;

    [Header("Режим камеры")]
    public bool isFirstPerson = false; // Текущий режим (по умолчанию 3-е лицо)

    [Header("Настройки 3-го лица")]
    public Vector3 thirdPersonOffset = new Vector3(0, 3f, -5f);
    public float lookHeightOffset = 1.5f;

    [Header("Настройки 1-го лица (FPV)")]
    // Смещение от центра дрона, чтобы камера была "в носу", а не внутри текстур самого дрона
    public Vector3 firstPersonOffset = new Vector3(0, 0.2f, 0.5f); 

    void Update()
    {
        // Переключение вида по нажатию кнопки V на клавиатуре (для удобства)
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleCameraView();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (isFirstPerson)
        {
            // === РЕЖИМ ОТ 1-ГО ЛИЦА (FPV) ===
            // Камера жестко привязывается к позиции и всем наклонам дрона
            transform.position = target.position + (target.rotation * firstPersonOffset);
            transform.rotation = target.rotation;
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

    // Эту функцию мы будем вызывать при нажатии на UI-кнопку в настройках
    public void ToggleCameraView()
    {
        isFirstPerson = !isFirstPerson; // Меняем значение на противоположное
    }
}