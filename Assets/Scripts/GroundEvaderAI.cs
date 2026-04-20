using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GroundEvaderAI : MonoBehaviour
{
    public float speed = 15f;
    public Transform player;
    public Vector3 minBounds = new Vector3(-45, 0, -45);
    public Vector3 maxBounds = new Vector3(45, 0, 45);

    private Rigidbody rb;
    private Vector3 evadeDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Чтобы машинка не кувыркалась по земле
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        CalculateDirection();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            CalculateDirection();
            timer = Random.Range(1f, 2.5f); // Меняет направление каждые 1-2.5 сек
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Едем только по X и Z, гравитацию (Y) не трогаем
        Vector3 vel = evadeDirection * speed;
        vel.y = rb.linearVelocity.y; 
        rb.linearVelocity = vel;

        // Поворот "мордой" по ходу движения
        if (evadeDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(evadeDirection), 0.1f);
        }

        ClampPosition();
    }

    void CalculateDirection()
    {
        if (player == null) return;
        
        // Вектор ОТ игрока
        Vector3 dir = transform.position - player.position;
        dir.y = 0; 
        
        // Добавляем случайный шум, чтобы она петляла
        Vector3 noise = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        evadeDirection = (dir.normalized + noise * 0.5f).normalized;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);
        transform.position = pos;
    }
}