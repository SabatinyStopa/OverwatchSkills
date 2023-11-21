using UnityEngine;
using UnityEngine.InputSystem;

namespace OverwatchClone.Player
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float mouseSensitive = 100f;
        [SerializeField] private float moveSpeed = 5f;

        private SimpleControls controls;
        private Vector2 rotation;

        public SimpleControls Controls { get => controls; set => controls = value; }

        private void Awake() => controls = new SimpleControls();
        private void OnEnable() => controls.Enable();
        private void OnDisable() => controls.Disable();

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            RotationHandler();
            MovimentHandler();
        }

        private void MovimentHandler()
        {
            var direction = controls.gameplay.move.ReadValue<Vector2>();

            if (direction.sqrMagnitude < 0.01) return;

            var scaledMoveSpeed = moveSpeed * Time.deltaTime;
            var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
            transform.position += move * scaledMoveSpeed;
        }

        private void RotationHandler()
        {
            var look = controls.gameplay.look.ReadValue<Vector2>();
            if (look.sqrMagnitude < 0.01) return;

            var scaledRotateSpeed = mouseSensitive * Time.deltaTime;

            rotation.y += look.x * scaledRotateSpeed;
            rotation.x = Mathf.Clamp(rotation.x - look.y * scaledRotateSpeed, -89, 89);
            transform.localEulerAngles = rotation;
        }
    }

}