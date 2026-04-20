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
    public float acceleration = 2f;
    public float horizontalDrag = 3f;

    [Header("Стабилизация и Наклон")]
    public float maxTiltAngle = 35f;
    public float tiltSpeed = 5f;

    [Header("Поворот (Yaw)")]
    public float yawSpeed = 100f;

    [Header("Настройки кнопок (Клавиатура)")]
    public KeyCode thrustUpKey = KeyCode.Space;
    public KeyCode thrustDownKey = KeyCode.LeftControl;
    public KeyCode yawLeftKey = KeyCode.Q;
    public KeyCode yawRightKey = KeyCode.E;

    private Rigidbody rb;
    private float currentYaw;
    private float currentPitch = 0f;
    private float currentRoll = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentYaw = transform.eulerAngles.y;
        currentHealth = maxHealth;

        // ЗАГРУЗКА КНОПОК: Если игрок их менял, грузим новые. Если нет - берем стандартные.
        thrustUpKey = (KeyCode)PlayerPrefs.GetInt("key_up", (int)KeyCode.Space);
        thrustDownKey = (KeyCode)PlayerPrefs.GetInt("key_down", (int)KeyCode.LeftControl);
        yawLeftKey = (KeyCode)PlayerPrefs.GetInt("key_left", (int)KeyCode.Q);
        yawRightKey = (KeyCode)PlayerPrefs.GetInt("key_right", (int)KeyCode.E);
    }

    void FixedUpdate()
    {
        // === 1. ВВОД НАКЛОНОВ (Правый стик Xbox или W/S A/D) ===
        // Читаем ввод с правого стика (названия осей из шага 1)
        float targetPitch = Input.GetAxis("VerticalRS") + Input.GetAxis("Vertical");
        float targetRoll = Input.GetAxis("HorizontalRS") + Input.GetAxis("Horizontal");

        // Ограничиваем значения в пределах -1...1
        targetPitch = Mathf.Clamp(targetPitch, -1f, 1f);
        targetRoll = Mathf.Clamp(targetRoll, -1f, 1f);

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, acceleration * Time.fixedDeltaTime);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, acceleration * Time.fixedDeltaTime);

        // === 2. ТЯГА ===
        float verticalInput = 0f;
        verticalInput += Input.GetAxis("Triggers"); // Геймпад
        
        // КЛАВИАТУРА (теперь используем переменные!)
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
        }

       // === 3. ПОВОРОТ ===
        float yawInput = Input.GetAxis("Horizontal"); // Геймпад
        
        // КЛАВИАТУРА (используем переменные)
        if (Input.GetKey(yawRightKey)) yawInput = 1f;
        if (Input.GetKey(yawLeftKey)) yawInput = -1f;
        
        currentYaw += yawInput * yawSpeed * Time.fixedDeltaTime;

        // === 4. ПРИМЕНЕНИЕ ФИЗИКИ ===
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