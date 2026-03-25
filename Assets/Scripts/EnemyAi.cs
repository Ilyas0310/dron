using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Настройки полета")]
    public float speed = 10f;             
    public float changeTargetDistance = 2f; 

    [Header("Настройки ИИ (Побег)")]
    public Transform player;         // Цель, от которой будем убегать
    public float fleeDistance = 15f; // Как далеко он пытается улететь за один раз

    [Header("Зона полета (Размеры карты)")]
    public Vector3 mapBounds = new Vector3(40, 20, 40); 

    private Vector3 targetPosition; 

    void Start()
    {
        // Автоматически находим игрока по тегу при старте сцены
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        SetNewEvasionTarget(); 
    }

    void Update()
    {
        // 1. Плавно летим к выбранной точке
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 2. Поворачиваемся лицом к точке
        transform.LookAt(targetPosition);

        // 3. Проверяем, долетели ли мы. Если да - снова убегаем!
        if (Vector3.Distance(transform.position, targetPosition) < changeTargetDistance)
        {
            SetNewEvasionTarget();
        }
    }

    // Новый метод: Выбор точки для побега
    void SetNewEvasionTarget()
    {
        if (player == null) return; // Если игрока нет, ничего не делаем

        // 1. Вычисляем вектор "ПОБЕГА" (направление от позиции игрока к позиции врага)
        Vector3 fleeDirection = (transform.position - player.position).normalized;

        // 2. Добавляем "панику" — случайное смещение, чтобы траектория была непредсказуемой
        Vector3 randomNoise = Random.insideUnitSphere * 5f;

        // 3. Вычисляем желаемую точку, куда враг хочет сбежать
        Vector3 desiredPoint = transform.position + (fleeDirection * fleeDistance) + randomNoise;

        // 4. ОГРАНИЧЕНИЕ КАРТЫ: не даем точке выйти за пределы наших mapBounds
        // Mathf.Clamp жестко обрезает координаты, если они больше или меньше дозволенного
        float clampedX = Mathf.Clamp(desiredPoint.x, -mapBounds.x / 2, mapBounds.x / 2);
        float clampedY = Mathf.Clamp(desiredPoint.y, 3f, mapBounds.y); // Y от 3, чтобы не скреб по земле
        float clampedZ = Mathf.Clamp(desiredPoint.z, -mapBounds.z / 2, mapBounds.z / 2);

        // Назначаем итоговую точку
        targetPosition = new Vector3(clampedX, clampedY, clampedZ);
    }

    // Рисуем зону полета желтым кубом
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0, mapBounds.y/2, 0), mapBounds);
    }
}