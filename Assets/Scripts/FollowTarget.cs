using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Настройки движения")]
    public float radius = 20f;
    public float baseSpeed = 5f;
    public float maxSpeed = 15f; 
    
    private float angle;
    private float currentSpeed;
    private float speedTimer;

    void Start()
    {
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        // Периодическое изменение скорости (ускорение/замедление)
        speedTimer -= Time.deltaTime;
        if (speedTimer <= 0)
        {
            currentSpeed = Random.Range(baseSpeed, maxSpeed);
            speedTimer = Random.Range(3f, 7f); // Меняем скорость каждые 3-7 секунд
        }

        // Движение по кругу через тригонометрию
        angle += currentSpeed * Time.deltaTime / radius;
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        
        transform.position = new Vector3(x, transform.position.y, z);
        
        // Поворачиваем цель по направлению движения
        transform.LookAt(transform.position + new Vector3(-Mathf.Sin(angle), 0, Mathf.Cos(angle)));
    }
}