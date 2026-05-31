using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FollowTarget : MonoBehaviour
{
    [Header("Настройки зоны слежения")]
    public float minDistance = 5f;   // Слишком близко - нельзя
    public float maxDistance = 15f;  // Слишком далеко - нельзя
    [Tooltip("Максимальный угол отклонения от центра экрана (в градусах)")]
    public float maxAngle = 20f;     // Нужно держать цель почти по центру

    [Header("Визуализация зоны (LineRenderer)")]
    public Color neutralColor = Color.red;    // Цвет, когда дрон НЕ в зоне
    public Color trackingColor = Color.green; // Цвет, когда дрон успешно следит
    [Range(10, 60)]
    public int circleSegments = 40; // Качество круга

    private LineRenderer lr;
    [HideInInspector] public bool isBeingTracked = false; // Флаг для менеджера

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    void Update()
    {
        DrawZone();
        
        // Меняем цвет в зависимости от того, следит ли за нами дрон прямо сейчас
        lr.startColor = isBeingTracked ? trackingColor : neutralColor;
        lr.endColor = isBeingTracked ? trackingColor : neutralColor;
    }

    private void SetupLineRenderer()
    {
        lr.positionCount = circleSegments + 1;
        lr.useWorldSpace = false; // Рисуем относительно самой машины
        lr.startWidth = 0.3f;
        lr.endWidth = 0.3f;
        
        // Опционально: можно назначить стандартный материал, чтобы линия светилась
        lr.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void DrawZone()
    {
        // Рисуем круг радиусом maxDistance вокруг объекта (на уровне земли, чуть приподняв, чтобы не мерцало)
        float angle = 0f;
        for (int i = 0; i < (circleSegments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * maxDistance;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * maxDistance;

            lr.SetPosition(i, new Vector3(x, 0.5f, z)); // 0.5f - высота над землей
            angle += (360f / circleSegments);
        }
    }

    // Рисуем конус угла обзора и зоны в редакторе Unity для удобства расстановки
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}