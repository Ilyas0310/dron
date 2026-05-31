using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AcroDroneController : MonoBehaviour
{
    [Header("Мощность моторов (Тяга)")]
    [Tooltip("Максимальная тяга (Например, 2-3 для резкого подрыва)")]
    public float maxThrustMultiplier = 2.0f; 
    [Tooltip("Инерция моторов. Чем меньше, тем дольше они разгоняются и тормозят")]
    public float motorInertia = 5.0f;

    [Header("Крутящий момент (Acro-вращение)")]
    public float pitchTorque = 15f; // Наклон вперед/назад (W/S)
    public float rollTorque = 15f;  // Наклон вбок (A/D)
    public float yawTorque = 8f;    // Вращение вокруг оси (Q/E)

    [Header("Настройки кнопок")]
    public KeyCode thrustUpKey = KeyCode.Space;
    public KeyCode thrustDownKey = KeyCode.LeftShift; 
    public KeyCode pitchForwardKey = KeyCode.W;
    public KeyCode pitchBackwardKey = KeyCode.S;
    public KeyCode rollLeftKey = KeyCode.A;
    public KeyCode rollRightKey = KeyCode.D;
    public KeyCode yawLeftKey = KeyCode.Q;
    public KeyCode yawRightKey = KeyCode.E;
    
    // === НОВОЕ: Кнопка для тепловизора ===
    public KeyCode switchViewKey = KeyCode.C;

    [Header("Эффекты камеры (Новое)")]
    public GameObject thermalVisionObject; // Сюда спавнер положит ссылку
    private bool isThermalActive = false;

    private Rigidbody rb;
    private float currentThrottle = 0f; 
    private float hoverForce; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = 0.5f; 
        rb.angularDamping = 4.0f; 

        hoverForce = Mathf.Abs(Physics.gravity.y) * rb.mass;

        // Загрузка настроек полета
        thrustUpKey = (KeyCode)PlayerPrefs.GetInt("acro_thrust_up", (int)KeyCode.Space);
        thrustDownKey = (KeyCode)PlayerPrefs.GetInt("acro_thrust_down", (int)KeyCode.LeftShift);
        pitchForwardKey = (KeyCode)PlayerPrefs.GetInt("acro_pitch_fwd", (int)KeyCode.W);
        pitchBackwardKey = (KeyCode)PlayerPrefs.GetInt("acro_pitch_back", (int)KeyCode.S);
        rollLeftKey = (KeyCode)PlayerPrefs.GetInt("acro_roll_left", (int)KeyCode.A);
        rollRightKey = (KeyCode)PlayerPrefs.GetInt("acro_roll_right", (int)KeyCode.D);
        yawLeftKey = (KeyCode)PlayerPrefs.GetInt("acro_yaw_left", (int)KeyCode.Q);
        yawRightKey = (KeyCode)PlayerPrefs.GetInt("acro_yaw_right", (int)KeyCode.E);
        
        // Загрузка кнопки тепловизора (по умолчанию 'C')
        switchViewKey = (KeyCode)PlayerPrefs.GetInt("acro_switch_view", (int)KeyCode.C);

        // При старте выключаем тепловизор
        if (thermalVisionObject != null)
        {
            thermalVisionObject.SetActive(false);
        }
    }

    // === НОВОЕ: Проверка нажатия кнопки тепловизора ===
    void Update()
    {
        if (Input.GetKeyDown(switchViewKey))
        {
            isThermalActive = !isThermalActive;
            if (thermalVisionObject != null)
            {
                thermalVisionObject.SetActive(isThermalActive);
            }
        }
    }

    void FixedUpdate()
    {
        ProcessThrust();
        ProcessAcroRotation();
    }

    private void ProcessThrust()
    {
        float targetThrottle = 0f;

        if (Input.GetKey(thrustUpKey))
        {
            targetThrottle = maxThrustMultiplier;
        }

        currentThrottle = Mathf.Lerp(currentThrottle, targetThrottle, motorInertia * Time.fixedDeltaTime);
        rb.AddRelativeForce(Vector3.up * (hoverForce * currentThrottle), ForceMode.Force);
    }

    private void ProcessAcroRotation()
    {
        float pitchInput = 0f;
        if (Input.GetKey(pitchForwardKey)) pitchInput = 1f;
        if (Input.GetKey(pitchBackwardKey)) pitchInput = -1f;

        float rollInput = 0f;
        if (Input.GetKey(rollLeftKey)) rollInput = 1f;   
        if (Input.GetKey(rollRightKey)) rollInput = -1f; 

        float yawInput = 0f;
        if (Input.GetKey(yawRightKey)) yawInput = 1f;
        if (Input.GetKey(yawLeftKey)) yawInput = -1f;

        Vector3 torque = new Vector3(pitchInput * pitchTorque, yawInput * yawTorque, rollInput * rollTorque);
        rb.AddRelativeTorque(torque, ForceMode.Acceleration);
    }
}