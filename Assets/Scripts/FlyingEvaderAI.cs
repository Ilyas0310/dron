using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlyingEvaderAI : MonoBehaviour
{
    public float speed = 12f;
    public Transform player;
    
    // Границы полета (включая высоту Y)
    public Vector3 minBounds = new Vector3(-45, 5, -45); 
    public Vector3 maxBounds = new Vector3(45, 25, 45);

    private Rigidbody rb;
    private Vector3 evadeDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Воздушная цель не должна падать
        CalculateDirection();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            CalculateDirection();
            timer = Random.Range(1f, 2f);
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Летаем во всех трех осях
        rb.linearVelocity = evadeDirection * speed;

        if (evadeDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(evadeDirection), 0.1f);
        }

        ClampPosition();
    }

    void CalculateDirection()
    {
        if (player == null) return;
        
        Vector3 dir = transform.position - player.position;
        Vector3 noise = Random.insideUnitSphere * 0.8f; // Шум в 3D (сфера)
        
        evadeDirection = (dir.normalized + noise).normalized;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y); // Ограничиваем и высоту
        pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);
        transform.position = pos;
    }
}