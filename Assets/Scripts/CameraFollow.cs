using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель слежения")]
    public Transform target;

    [Header("Настройки позиции")]
    // Теперь Y (высота) больше, чтобы камера была выше дрона
    public Vector3 offset = new Vector3(0, 3f, -5f); 
    public float positionSmoothSpeed = 10f; // Плавность движения

    [Header("Настройки взгляда")]
    public float lookHeightOffset = 1.5f;   // На сколько выше дрона будет смотреть камера
    public float rotationSmoothSpeed = 5f;  // Плавность поворота камеры

    void FixedUpdate()
    {
        if (target == null) return;

        // 1. Берем ТОЛЬКО поворот дрона по горизонтали (Y - Yaw)
        // Игнорируем наклоны вперед/назад и влево/вправо
        float currentYaw = target.eulerAngles.y;
        Quaternion horizontalRotation = Quaternion.Euler(0, currentYaw, 0);

        // 2. Вычисляем идеальную позицию сзади и сверху (с учетом только горизонтального поворота)
        Vector3 desiredPosition = target.position + (horizontalRotation * offset);

        // 3. Плавно летим в эту точку
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.fixedDeltaTime);

        // 4. Куда смотрим?
        // Чтобы не пялиться в землю, смотрим в точку, которая находится чуть ВЫШЕ самого дрона
        Vector3 lookAtPoint = target.position + (Vector3.up * lookHeightOffset);
        
        // Вычисляем нужный угол поворота камеры и плавно поворачиваемся
        Quaternion desiredRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.fixedDeltaTime);
    }
}