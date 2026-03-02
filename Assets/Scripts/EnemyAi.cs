using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Настройки полета")]
    public float speed = 10f;             // Скорость врага
    public float changeTargetDistance = 2f; // Как близко нужно подлететь к точке, чтобы выбрать новую

    [Header("Зона полета (Размеры карты)")]
    public Vector3 mapBounds = new Vector3(40, 20, 40); // Ширина(X), Высота(Y), Длина(Z)

    private Vector3 targetPosition; // Текущая точка, куда летит враг

    void Start()
    {
        SetNewRandomTarget(); // Выбираем первую цель при старте
    }

    void Update()
    {
        // 1. Плавно летим к выбранной точке
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 2. Поворачиваемся лицом к точке (чтобы летел передом)
        transform.LookAt(targetPosition);

        // 3. Проверяем, долетели ли мы
        if (Vector3.Distance(transform.position, targetPosition) < changeTargetDistance)
        {
            SetNewRandomTarget(); // Если долетели, выбираем новую точку
        }
    }

    // Метод для выбора случайной точки в пространстве
    void SetNewRandomTarget()
    {
        // Выбираем случайные координаты в пределах mapBounds
        float randomX = Random.Range(-mapBounds.x / 2, mapBounds.x / 2);
        float randomY = Random.Range(3f, mapBounds.y); // Y от 3, чтобы не летал прямо по земле
        float randomZ = Random.Range(-mapBounds.z / 2, mapBounds.z / 2);

        targetPosition = new Vector3(randomX, randomY, randomZ);
    }

    // Рисуем зону полета в редакторе (чтобы было удобно настраивать)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Рисуем куб, показывающий, где враг может летать
        Gizmos.DrawWireCube(new Vector3(0, mapBounds.y/2, 0), mapBounds);
    }
}