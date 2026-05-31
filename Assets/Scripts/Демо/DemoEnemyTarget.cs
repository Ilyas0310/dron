using UnityEngine;

public class DemoEnemyTarget : MonoBehaviour
{
    // Этот метод автоматически срабатывает при любом физическом столкновении
    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, что в нас врезался именно дрон игрока (по тегу)
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Враг перехвачен в демо-режиме!");
            
            // Уничтожаем этот игровой объект (машинку)
            Destroy(gameObject);
        }
    }
}