using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConvoyVehicle : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 10f;
    public float turnSpeed = 5f; // Плавность поворота машины
    public float waypointTolerance = 2f; // На каком расстоянии от точки переключаемся на следующую

    [Header("Маршрут")]
    public Transform[] waypoints; // Список точек маршрута
    public bool loopPath = false; // Зациклить маршрут (поедет от последней к первой)?

    private Rigidbody rb;
    private int currentWaypointIndex = 0;
    private bool hasFinishedPath = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Замораживаем вращение, чтобы машина не кувыркалась
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (waypoints.Length == 0)
        {
            Debug.LogWarning($"У машины {gameObject.name} нет маршрута!");
        }
    }

    void FixedUpdate()
    {
        // Если точек нет или маршрут закончен — останавливаемся
        if (waypoints.Length == 0 || hasFinishedPath)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // Гравитация остается
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null) return;

        // 1. Вычисляем вектор к следующей точке (без учета высоты Y)
        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0; 

        // 2. Плавно поворачиваем нос машины в сторону точки
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
        }

        // 3. Едем вперед туда, куда смотрим
        Vector3 vel = transform.forward * speed;
        vel.y = rb.linearVelocity.y; 
        rb.linearVelocity = vel;

        // 4. Проверяем, доехали ли мы до текущей точки
        if (direction.magnitude < waypointTolerance)
        {
            currentWaypointIndex++; // Переключаемся на следующую

            // Если точки закончились
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loopPath)
                {
                    currentWaypointIndex = 0; // Едем на второй круг
                }
                else
                {
                    hasFinishedPath = true; // Тормозим на финише
                }
            }
        }
    }

    // Эта магия рисует синие линии маршрута в самом редакторе Unity!
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawSphere(waypoints[i].position, 0.5f); // Рисуем шарик на самой точке
            }
        }

        // Рисуем последнюю точку и замыкающую линию, если маршрут зациклен
        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.5f);
            
            if (loopPath && waypoints[0] != null)
            {
                Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
            }
        }
    }
}