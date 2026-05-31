using UnityEngine;

public class TargetGoal : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Если в нас врезался объект с тегом "Player"...
        if (collision.gameObject.CompareTag("Player"))
        {
            // === ВЫДАЕМ АЧИВКУ ЗА УСПЕШНУЮ ОХОТУ ===
            if (AchievementManager.instance != null)
            {
                AchievementManager.instance.UnlockAchievement(
                    "first_chase_win", 
                    "Перехватчик", 
                    "Успешно догоните и перехватите ускользающую цель в режиме ChaseMode."
                );
            }

            // ...сообщаем менеджеру о победе!
            if (VictoryManager.instance != null)
            {
                VictoryManager.instance.FinishLevel();
            }
        }
    }
}