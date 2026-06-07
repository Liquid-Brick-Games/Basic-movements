using UnityEngine;

public class BaseMovement : MonoBehaviour {
    [SerializeField] private float speed = 5f; //Выносим переменную, модификатор скорости, во внешнюю среду. Полезно для ручной отладки, без необходимости перекомпилировать скрипт каждый раз

    private GameActions inputActions; //Создаём локальный экземпляр класса GameActions
    private Vector2 moveInput; //Аналогично, создаём экземпляр Vector 2 (переменную с типа Vector 2)

    private void Awake() {
        inputActions = new GameActions(); //Инициализируем карты действий
    }

    private void OnEnable() {
        inputActions.Player.Enable(); //Включаем карту действий Player, когда объект игрока будет включён в сцене.
    }

    private void OnDisable() {
        inputActions.Player.Disable(); //Отключаем карту действий Player, когда объект игрока будет выключен в сцене.
    }

    //*---- inputActions - общая схема
    //*---- Player - конкретная карта Player, в общем списке всех карт
    //!---- OnEnable() и OnDisable() прописывать обязательно какждый раз. Прямой аналог из React - useEffect и утечки памяти, в случае если не сделать очистку при размонтировании компонента

    private void Update() {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>(); //*---- Считываем значение координат Vector 2
        /*---- inputActions - обращаемся к классу действий ----*/
        /*---- Player - обращаемся к карте Player ----*/
        /*---- Move - обращаемся к действию Move ----*/
        /*---- ReadValue<Vector2>() - Считываем значение координат Vector 2 ----*/

        //*---- Поскольку координаты положения персонажа записаны в двухмерной системе, а перемещаемся в сцене мы в трёхмерной - необходимо конвертировать координаты из одной системы с другую ----*//

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        //*---- Vector 3 (X,Y,Z) ----*//
        /*---- X - влево-вправо ----*/
        /*---- Y - вверх-вниз ----*/
        /*---- Z - вперёд-назад ----*/

        //*---- Передаём положение координат, умножив на модификатор скорости и дельту времени ФПС ----*//

        transform.Translate(moveDirection * speed * Time.deltaTime);
        /*---- transform - напрямую обращаемся к ссылке на положение объекта в сцене ----*/
        /*---- Translate - сдвигает объект в сцене на заданное значение, Vector 3 ----*/

        //?---- Также ещё есть ----?//
        /*---- LookAt - поворачивает объект в сторону дгругог (указанного) объекта ----*/
        /*---- Rotate - вращает объект вокруг оси ----*/
    }
}
