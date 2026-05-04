using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Высота и Зависание (DJI Mode)")]
    public float verticalSpeed = 5f;
    public float verticalAcceleration = 10f;

    [Header("Система урона")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float crashTolerance = 5f;
    public float damageMultiplier = 2f;

    [Header("Инерция и Торможение")]
    public float acceleration = 2f; // Скорость наклона
    public float moveForce = 15f;   // ТО САМОЕ УСКОРЕНИЕ вперед/вбок
    public float horizontalDrag = 3f;

    [Header("Стабилизация и Наклон")]
    public float maxTiltAngle = 35f;
    public float tiltSpeed = 5f;

    [Header("Поворот (Yaw)")]
    public float yawSpeed = 100f;

    [Header("Настройки кнопок")]
    public KeyCode thrustUpKey = KeyCode.Space;
    public KeyCode thrustDownKey = KeyCode.LeftControl;
    public KeyCode yawLeftKey = KeyCode.Q;
    public KeyCode yawRightKey = KeyCode.E;
    public KeyCode pitchForwardKey = KeyCode.W;
    public KeyCode pitchBackwardKey = KeyCode.S;
    public KeyCode rollLeftKey = KeyCode.A;
    public KeyCode rollRightKey = KeyCode.D;
    public KeyCode switchViewKey = KeyCode.C;

    private Rigidbody rb;
    private float currentYaw;
    private float currentPitch = 0f;
    private float currentRoll = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentYaw = transform.eulerAngles.y;
        currentHealth = maxHealth;

        // ЗАГРУЗКА КНОПОК
        thrustUpKey = (KeyCode)PlayerPrefs.GetInt("key_up", (int)KeyCode.Space);
        thrustDownKey = (KeyCode)PlayerPrefs.GetInt("key_down", (int)KeyCode.LeftControl);
        yawLeftKey = (KeyCode)PlayerPrefs.GetInt("key_left", (int)KeyCode.Q);
        yawRightKey = (KeyCode)PlayerPrefs.GetInt("key_right", (int)KeyCode.E);
        
        // Загружаем новые клавиши
        pitchForwardKey = (KeyCode)PlayerPrefs.GetInt("key_forward", (int)KeyCode.W);
        pitchBackwardKey = (KeyCode)PlayerPrefs.GetInt("key_backward", (int)KeyCode.S);
        rollLeftKey = (KeyCode)PlayerPrefs.GetInt("key_strafe_left", (int)KeyCode.A);
        rollRightKey = (KeyCode)PlayerPrefs.GetInt("key_strafe_right", (int)KeyCode.D);
        switchViewKey = (KeyCode)PlayerPrefs.GetInt("key_switch_view", (int)KeyCode.C);
    }

    void FixedUpdate()
    {
        // === 1. ВВОД НАКЛОНОВ ===
        float targetPitch = Input.GetAxis("VerticalRS");
        float targetRoll = Input.GetAxis("HorizontalRS");

        // Добавляем ввод с клавиатуры (ваши переменные клавиш)
        if (Input.GetKey(pitchForwardKey)) targetPitch = 1f;
        if (Input.GetKey(pitchBackwardKey)) targetPitch = -1f;
        if (Input.GetKey(rollRightKey)) targetRoll = 1f;
        if (Input.GetKey(rollLeftKey)) targetRoll = -1f;

        targetPitch = Mathf.Clamp(targetPitch, -1f, 1f);
        targetRoll = Mathf.Clamp(targetRoll, -1f, 1f);

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, acceleration * Time.fixedDeltaTime);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, acceleration * Time.fixedDeltaTime);

        // === 2. ТЯГА (Вертикаль) ===
        float verticalInput = Input.GetAxis("Triggers");
        if (Input.GetKey(thrustUpKey)) verticalInput = 1f;
        if (Input.GetKey(thrustDownKey)) verticalInput = -1f;

        verticalInput = Mathf.Clamp(verticalInput, -1f, 1f);

        float targetVy = verticalInput * verticalSpeed;
        float currentVy = rb.linearVelocity.y;
        float verticalForce = (targetVy - currentVy) * verticalAcceleration;
        float hoverForce = rb.mass * Mathf.Abs(Physics.gravity.y);
        
        float desiredGlobalUpForce = hoverForce + verticalForce;
        float cosTheta = Mathf.Max(Vector3.Dot(transform.up, Vector3.up), 0.2f);
        float actualThrust = (desiredGlobalUpForce / cosTheta) * (currentHealth / maxHealth);

        if (currentHealth > 0)
        {
            rb.AddRelativeForce(Vector3.up * actualThrust);
            
            // --- НОВОЕ: ДОПОЛНИТЕЛЬНОЕ УСКОРЕНИЕ ВПЕРЕД/ВБОК ---
            // Мы прикладываем силу в плоскости дрона на основе его текущих наклонов
            Vector3 moveDir = (transform.forward * currentPitch) + (transform.right * currentRoll);
            rb.AddForce(moveDir * moveForce, ForceMode.Acceleration);
        }

        // === 3. ПОВОРОТ (Yaw) ===
        float yawInput = Input.GetAxis("Horizontal"); 
        if (Input.GetKey(yawRightKey)) yawInput = 1f;
        if (Input.GetKey(yawLeftKey)) yawInput = -1f;
        
        currentYaw += yawInput * yawSpeed * Time.fixedDeltaTime;

        // === 4. ТОРМОЖЕНИЕ И ВРАЩЕНИЕ ===
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        rb.AddForce(-horizontalVelocity * horizontalDrag, ForceMode.Acceleration);

        Quaternion targetRotation = Quaternion.Euler(currentPitch * maxTiltAngle, currentYaw, -currentRoll * maxTiltAngle);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed > crashTolerance)
        {
            float damage = (impactSpeed - crashTolerance) * damageMultiplier;
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            Debug.Log($"Удар! Прочность: {currentHealth:F1}%");
        }
    }
}