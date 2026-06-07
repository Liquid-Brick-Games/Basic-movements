using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Гарантированно создаём RigidBody на указанный объект. Также - защита от дурака. Это поле гарантиреут, что компонент Rigidbody не может быть удалён с объекта
public class RigidMouseFollowMovement : MonoBehaviour {

    //*---- Разделяем модификаторы скорость передвижения для ходьбы и бега ----*//
    [Header("Movement Speeds")] //Декоративный атрибут. Задаёт заголовок в инспектор. Удобно для разграничения при работе с большим количеством вынесенных полей
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float rotationSpeed = 1200f; // Задаём модификатор для поворота объекта. Значение указывается в градусах/сек

    //*---- Добавляем модификаторы ускорения для прыжка и рывка ----*//
    [Header("Jump & Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float jumpForce = 5f;

    private GameActions inputActions; //Создаём локальный экземпляр класса GameActions
    private Vector2 moveInput; //Аналогично, создаём экземпляр Vector 2 (переменную с типа Vector 2)
    private Rigidbody rigidbody; //Экземпляр класса RigidBody
    private Transform cameraTransform; //Экземпляр класса Transform для управления камерой

    private bool isSprinting; //Булиан для проверки состояния

    private void Awake() {
        inputActions = new GameActions(); //Инициализируем карты действий
        rigidbody = GetComponent<Rigidbody>(); //Передаём данные о "физике" объекта
        /*---- GetComponent<>() - ищет компонент по типу на этом же объекте ----*/

        if (Camera.main != null) {
            cameraTransform = Camera.main.transform;
        }

    }

    private void OnEnable() {
        inputActions.Player.Enable(); //Включаем карту действий Player, когда объект игрока будет включён в сцене.


        //*---- Накидываем "прослушку" событий ----*//
        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Dash.performed += OnDashPerformed;
    }

    private void OnDisable() {
        //*---- Снимаем "прослушку" событий ----*//
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Dash.performed -= OnDashPerformed;


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

        isSprinting = inputActions.Player.Sprint.ReadValue<float>() > 0.1f; // Проверяем зажат ли Shift для спринта


    }

    private void FixedUpdate() {
        //!---- Вся логика по перемещению физических объектов находится здесь ----!//

        RotatePlayerWithCamera(); // Функция для поворачивания объекта за камерой
        MovePlayerStrafe(); // Функция для движения объекта его относительно нового поворота
    }





    private void RotatePlayerWithCamera() {
        if (!cameraTransform) return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        if (forward != Vector3.zero) {

            Quaternion targerRotation = Quaternion.LookRotation(forward);

            rigidbody.MoveRotation(Quaternion.RotateTowards(rigidbody.rotation, targerRotation, rotationSpeed * Time.fixedDeltaTime));
        }

    }
    private void MovePlayerStrafe() {
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        //*---- Cоздаем вектор движения на основе локальных осей объекта (transform.forward и transform.right) ----*//
        Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        //--- transform.forward - вектор, указывающий "из носа" объекта. Умножаем его на moveInput.y (нажатие W/S)
        //--- transform.right - вектор, указывающий "из правого уха" объекта. Умножаем его на moveInput.x (нажатие A/D)

        rigidbody.linearVelocity = new Vector3(moveDirection.x * currentSpeed, rigidbody.linearVelocity.y, moveDirection.z * currentSpeed);
    }


    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        //*---- При срабатываении события для прыжка ----*//
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnDashPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        //*---- При срабатываении события для рывка ----*//
        Vector3 dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized; //Задаём направление для рывка. Куда указывал WASD - туда объект и полетит

        if (dashDirection == Vector3.zero) dashDirection = transform.forward; // Условие, если во время нажатия WASD не нажат - просто толкнуть объект строго вперёд

        rigidbody.AddForce(dashDirection * dashForce, ForceMode.Impulse);
    }
}
