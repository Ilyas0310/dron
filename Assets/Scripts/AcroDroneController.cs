using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AcroDroneController : MonoBehaviour
{
    [Header("Мощность моторов (Тяга)")]
    [Tooltip("Максимальная тяга (2 = набор высоты)")]
    public float maxThrustMultiplier = 2.0f; 
    [Tooltip("Инерция моторов. Чем меньше, тем дольше они разгоняются и тормозят")]
    public float motorInertia = 2.5f;

    [Header("Крутящий момент (Acro-вращение)")]
    public float pitchTorque = 15f; // Наклон вперед/назад (W/S)
    public float rollTorque = 15f;  // Наклон вбок (A/D)
    public float yawTorque = 8f;    // Вращение вокруг оси (Q/E)

    [Header("Настройки кнопок (Можно менять в инспекторе)")]
    public KeyCode thrustUpKey = KeyCode.Space;
    public KeyCode thrustDownKey = KeyCode.LeftShift; // Сделали Shift по твоему ТЗ
    
    public KeyCode pitchForwardKey = KeyCode.W;
    public KeyCode pitchBackwardKey = KeyCode.S;
    public KeyCode rollLeftKey = KeyCode.A;
    public KeyCode rollRightKey = KeyCode.D;
    public KeyCode yawLeftKey = KeyCode.Q;
    public KeyCode yawRightKey = KeyCode.E;

    private Rigidbody rb;
    
    // Текущая тяга моторов. 1 = вес дрона (висит). 0 = падаем.
    private float currentThrottle = 1f; 
    private float hoverForce; // Рассчитанная сила для идеального зависания

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Важные настройки физики прямо из кода, чтобы дрон вел себя как настоящий
        rb.useGravity = true;
        
        // Сопротивление воздуха для линейного движения (чтобы не скользил вечно)
        rb.linearDamping = 0.5f; 
        
        // ВАЖНО ДЛЯ ACRO: Сопротивление вращению. 
        // Оно тормозит вращение, когда ты отпускаешь WASD, не давая дрону крутиться как волчок
        rb.angularDamping = 4.0f; 

        // Вычисляем, какая сила нужна ровно для того, чтобы побороть гравитацию (усилие = 1)
        hoverForce = Mathf.Abs(Physics.gravity.y) * rb.mass;
    }

    void FixedUpdate()
    {
        ProcessThrust();
        ProcessAcroRotation();
    }

    private void ProcessThrust()
    {
        // Базовая цель тяги - 1 (удержание высоты, если дрон стоит ровно)
        float targetThrottle = 1f;

        // Если жмем Пробел -> цель тяги становится максимальной (например, 2)
        if (Input.GetKey(thrustUpKey))
        {
            targetThrottle = maxThrustMultiplier;
        }
        // Если жмем Shift -> отключаем моторы (тяга 0, падаем)
        else if (Input.GetKey(thrustDownKey))
        {
            targetThrottle = 0f;
        }

        // Плавное изменение текущей тяги к целевой (Имитация инерции пропеллеров)
        // Моторы не могут мгновенно остановиться или разогнаться
        currentThrottle = Mathf.Lerp(currentThrottle, targetThrottle, motorInertia * Time.fixedDeltaTime);

        // Применяем силу СТРОГО ВВЕРХ относительно самого дрона (transform.up)
        // Сила = (Вес дрона) * (Текущий множитель тяги от 0 до 2)
        rb.AddRelativeForce(Vector3.up * (hoverForce * currentThrottle), ForceMode.Force);
    }

    private void ProcessAcroRotation()
    {
        // Считываем оси, как в настоящем полетном контроллере (Betaflight)
        float pitchInput = 0f;
        if (Input.GetKey(pitchForwardKey)) pitchInput = 1f;
        if (Input.GetKey(pitchBackwardKey)) pitchInput = -1f;

        float rollInput = 0f;
        if (Input.GetKey(rollLeftKey)) rollInput = 1f;
        if (Input.GetKey(rollRightKey)) rollInput = -1f;

        float yawInput = 0f;
        if (Input.GetKey(yawRightKey)) yawInput = 1f;
        if (Input.GetKey(yawLeftKey)) yawInput = -1f;

        // Формируем вектор крутящего момента
        // Важно: по оси Z (Roll) обычно требуется инверсия, чтобы A наклоняло влево, а D вправо
        Vector3 torque = new Vector3(pitchInput * pitchTorque, yawInput * yawTorque, -rollInput * rollTorque);

        // AddRelativeTorque - это главное отличие Acro! 
        // Мы применяем ускорение вращения, а не меняем угол напрямую.
        rb.AddRelativeTorque(torque, ForceMode.Acceleration);
    }
}