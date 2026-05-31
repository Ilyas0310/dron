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
        
        // УБРАЛИ CalculateDirection() отсюда, так как игрока на старте сцены еще нет
    }

    void Update()
    {
        // === ДИНАМИЧЕСКИЙ ПОИСК ИГРОКА ===
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) 
            {
                // Ура, игрок заспавнился! Запоминаем его и сразу вычисляем куда ехать
                player = p.transform;
                CalculateDirection(); 
            }
            else
            {
                // Если игрока еще нет, просто ждем следующий кадр
                return; 
            }
        }

        // === ЛОГИКА ТАЙМЕРА (работает только когда игрок найден) ===
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