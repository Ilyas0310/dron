using UnityEngine;

public class TargetGoal : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Если в нас врезался объект с тегом "Player"...
        if (collision.gameObject.CompareTag("Player"))
        {
            // ...сообщаем менеджеру о победе!
            if (VictoryManager.instance != null)
            {
                VictoryManager.instance.FinishLevel();
            }
        }
    }
}