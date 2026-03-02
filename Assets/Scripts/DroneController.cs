using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Тяга (Взлет)")]
    public float thrustForce = 25f;  

    [Header("Инерция и Торможение (Реализм)")]
    public float acceleration = 1.5f;   // Как долго дрон разгоняется (меньше = дольше разгон)
    public float horizontalDrag = 2f;   // Торможение об воздух по горизонтали (чтобы не скользил вечно)

    [Header("Управление наклоном (Стабилизация)")]
    public float maxTiltAngle = 35f; 
    public float tiltSpeed = 4f;     

    [Header("Поворот вокруг своей оси (Yaw)")]
    public float yawSpeed = 100f;    

    private Rigidbody rb;
    private float currentYaw;        
    
    // Переменные для хранения "плавного" текущего состояния
    private float currentPitch = 0f;
    private float currentRoll = 0f;
    private float currentThrust = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentYaw = transform.eulerAngles.y; 
    }

    void FixedUpdate()
    {
        // === 1. ПЛАВНЫЙ ВВОД (Инерция) ===
        // Считываем, что нажал игрок (целевые значения)
        float targetPitch = Input.GetAxis("Vertical");   // W/S
        float targetRoll = Input.GetAxis("Horizontal");  // A/D
        float targetThrust = Input.GetKey(KeyCode.Space) ? 1f : 0f;

        // Mathf.Lerp плавно переводит текущее значение к целевому. 
        // Это создает эффект "тяжести" и долгого разгона моторов.
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, acceleration * Time.fixedDeltaTime);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, acceleration * Time.fixedDeltaTime);
        currentThrust = Mathf.Lerp(currentThrust, targetThrust, acceleration * Time.fixedDeltaTime);

        // === 2. ТЯГА ВВЕРХ ===
        // Теперь тяга нарастает плавно
        rb.AddRelativeForce(Vector3.up * (currentThrust * thrustForce));

        // === 3. РЕАЛИСТИЧНАЯ ГРАВИТАЦИЯ И ТОРМОЖЕНИЕ ===
        // Получаем текущую скорость дрона (в Unity 6 это linearVelocity, в старых velocity)
        Vector3 velocity = rb.linearVelocity; 
        
        // Берем только горизонтальную скорость (без высоты Y)
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        
        // Применяем обратную силу для торможения только по горизонтали
        rb.AddForce(-horizontalVelocity * horizontalDrag, ForceMode.Acceleration);

        // === 4. ПОВОРОТ И НАКЛОН ===
        float yawInput = 0f;
        if (Input.GetKey(KeyCode.E)) yawInput = 1f;
        if (Input.GetKey(KeyCode.Q)) yawInput = -1f;
        currentYaw += yawInput * yawSpeed * Time.fixedDeltaTime;

        // Вычисляем углы на основе сглаженного ввода
        float pitchAngle = currentPitch * maxTiltAngle; 
        float rollAngle = -currentRoll * maxTiltAngle; 

        Quaternion targetRotation = Quaternion.Euler(pitchAngle, currentYaw, rollAngle);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime));
    }
}