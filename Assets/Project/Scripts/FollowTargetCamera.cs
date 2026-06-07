using UnityEngine;

public class FollowTargetCamera : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // Задаём ссылку на положение координат объекта следования
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -3f); // Выносим настройки камеры "из-за спины"

    void LateUpdate() // LateUpdate выполняется после того, как следуемый объект подвинулся в FixedUpdate
    {
        if (playerTransform != null)
        {

            transform.position = playerTransform.position + offset; // Присваиваем позицую следуемого объекта
        }
    }
}
