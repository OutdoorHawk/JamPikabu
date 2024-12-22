using System.Collections;
using UnityEngine;

namespace Code.Gameplay.Common.World
{
    public class WindSpawner : MonoBehaviour
    {
        [SerializeField] private float interval = 2f; // Интервал времени между активациями
        [SerializeField] private float radius = 5f; // Радиус перемещения

        private Transform[] childObjects;
        private Coroutine activationCoroutine;

        private void Start()
        {
            // Получаем все дочерние объекты
            childObjects = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                childObjects[i] = transform.GetChild(i);
                childObjects[i].gameObject.SetActive(false); // Выключаем их
            }

            // Запускаем корутину активации
            activationCoroutine = StartCoroutine(ActivateRandomChild());
        }

        private IEnumerator ActivateRandomChild()
        {
            while (true)
            {
                // Ожидаем заданный интервал
                yield return new WaitForSeconds(interval);

                if (childObjects.Length == 0)
                    yield break; // Если дочерних объектов нет, выходим из корутины

                // Выбираем случайный дочерний объект
                int randomIndex = Random.Range(0, childObjects.Length);
                Transform selectedChild = childObjects[randomIndex];

                // Перемещаем его в случайное место в радиусе
                Vector3 randomPosition = transform.position + (Vector3)(Random.insideUnitCircle * radius);
                selectedChild.position = randomPosition;

                // Включаем объект
                selectedChild.gameObject.SetActive(true);

                // Ожидаем 5 секунд, затем выключаем объект
                yield return new WaitForSeconds(2.5f);
                selectedChild.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (activationCoroutine != null)
            {
                StopCoroutine(activationCoroutine); // Останавливаем корутину при уничтожении объекта
            }
        }
    }
}