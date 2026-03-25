using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Двигатели и Тяга (Колесико мыши)")]
    public float maxThrust = 40f;    
    public float throttleSpeed = 2f; 
    
    [SerializeField] 
    private float currentThrottle = 0f; 

    [Header("Система урона")]
    public float maxHealth = 100f;          // Максимальное здоровье
    public float currentHealth = 100f;      // Текущее здоровье
    public float crashTolerance = 5f;       // Порог: удары слабее этой скорости игнорируются
    public float damageMultiplier = 2f;     // На сколько умножать урон при сильном ударе

    [Header("Инерция и Торможение")]
    public float acceleration = 2f;
    public float horizontalDrag = 3f;

    [Header("Стабилизация и Наклон")]
    public float maxTiltAngle = 35f;
    public float tiltSpeed = 5f;

    [Header("Поворот (Yaw)")]
    public float yawSpeed = 100f;

    private Rigidbody rb;
    private float currentYaw;
    private float currentPitch = 0f;
    private float currentRoll = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentYaw = transform.eulerAngles.y;
        currentHealth = maxHealth; // При старте дрон полностью цел
    }

    void Update()
    {
        // Управление газом
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            currentThrottle += scroll * throttleSpeed * Time.deltaTime;
            currentThrottle = Mathf.Clamp(currentThrottle, 0f, 1f);
        }
    }

    void FixedUpdate()
    {
        float targetPitch = Input.GetAxis("Vertical");   
        float targetRoll = Input.GetAxis("Horizontal");  

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, acceleration * Time.fixedDeltaTime);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, acceleration * Time.fixedDeltaTime);

        // === ВЛИЯНИЕ УРОНА НА ТЯГУ ===
        // Вычисляем процент здоровья (от 1.0 до 0.0)
        float healthFactor = currentHealth / maxHealth; 
        
        // Умножаем тягу на процент здоровья (если сломан наполовину, тяга упадет в 2 раза)
        float actualThrust = currentThrottle * maxThrust * healthFactor;

        float tiltCompensation = 1f + (Mathf.Abs(currentPitch) + Mathf.Abs(currentRoll)) * 0.2f;

        // Если здоровье больше 0, моторы работают
        if (currentHealth > 0)
        {
            rb.AddRelativeForce(Vector3.up * (actualThrust * tiltCompensation));
        }

        Vector3 velocity = rb.linearVelocity; 
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        rb.AddForce(-horizontalVelocity * horizontalDrag, ForceMode.Acceleration);

        float yawInput = 0f;
        if (Input.GetKey(KeyCode.E)) yawInput = 1f;
        if (Input.GetKey(KeyCode.Q)) yawInput = -1f;
        currentYaw += yawInput * yawSpeed * Time.fixedDeltaTime;

        float pitchAngle = currentPitch * maxTiltAngle;
        float rollAngle = -currentRoll * maxTiltAngle;

        Quaternion targetRotation = Quaternion.Euler(pitchAngle, currentYaw, rollAngle);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime));
    }

    // === ФИЗИКА УДАРОВ ===
    void OnCollisionEnter(Collision collision)
    {
        // Измеряем силу столкновения
        float impactSpeed = collision.relativeVelocity.magnitude;

        // Если удар оказался сильнее нашей "мягкой посадки"
        if (impactSpeed > crashTolerance)
        {
            // Формула урона: превышение скорости умножаем на множитель
            float damage = (impactSpeed - crashTolerance) * damageMultiplier;
            currentHealth -= damage;
            
            // Не даем здоровью уйти в минус
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            Debug.Log($"Авария! Сила удара: {impactSpeed:F1} | Урон: {damage:F1} | Остаток прочности: {currentHealth:F1}%");

            if (currentHealth <= 0)
            {
                Debug.Log("Дрон полностью уничтожен! Моторы заглохли.");
            }
        }
    }
}