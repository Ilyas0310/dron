using UnityEngine;

public class LandingPad : MonoBehaviour
{
    [Header("Настройки посадки")]
    [Tooltip("Сколько секунд нужно удержаться на крыше")]
    public float requiredTime = 2.0f; 
    
    [Header("Визуал")]
    public Renderer padRenderer; // Чтобы перекрасить крышу при успехе
    public Material successMaterial; // Зеленый материал

    private float currentTime = 0f;
    private bool isLanded = false; // Флаг, чтобы нельзя было сесть на одну машину дважды
    private LandingModeManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<LandingModeManager>();
    }

    // Срабатывает каждый кадр, пока дрон находится внутри зоны-триггера
    void OnTriggerStay(Collider other)
    {
        if (isLanded) return; // Если уже садились сюда - игнорируем

        if (other.CompareTag("Player"))
        {
            currentTime += Time.deltaTime;

            if (currentTime >= requiredTime)
            {
                SuccessfulLanding();
            }
        }
    }

    // Если дрон соскользнул или улетел раньше времени - таймер сбрасывается
    void OnTriggerExit(Collider other)
    {
        if (!isLanded && other.CompareTag("Player"))
        {
            currentTime = 0f;
        }
    }

    private void SuccessfulLanding()
    {
        isLanded = true;
        Debug.Log("Успешная посадка!");

        // Сообщаем главному менеджеру, что мы сели
        if (manager != null)
        {
            manager.RegisterLanding();
        }

        // Выдаем ачивку за сложный трюк!
        if (AchievementManager.instance != null)
        {
            AchievementManager.instance.UnlockAchievement(
                "moving_landing", 
                "Мастер посадки", 
                "Успешно приземлитесь на движущийся транспорт."
            );
        }

        // Перекрашиваем крышу (чтобы игрок знал, что сюда больше садиться не нужно)
        if (padRenderer != null && successMaterial != null)
        {
            padRenderer.material = successMaterial;
        }
    }
}