using UnityEngine;

namespace OverwatchClone.Player
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] protected float mouseSensitive = 100f;
        [SerializeField] protected float moveSpeed = 5f;

        protected SimpleControls controls;
        protected Vector2 rotation;

        public SimpleControls Controls { get => controls; set => controls = value; }

        public virtual void Awake() => controls = new SimpleControls();
        public virtual void OnEnable() => controls.Enable();
        public virtual void OnDisable() => controls.Disable();

        public virtual void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public virtual void Update()
        {
            RotationHandler();
            MovimentHandler();
        }

        public virtual void MovimentHandler()
        {
            var direction = controls.gameplay.move.ReadValue<Vector2>();

            if (direction.sqrMagnitude < 0.01) return;

            var scaledMoveSpeed = moveSpeed * Time.deltaTime;
            var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
            transform.position += move * scaledMoveSpeed;
        }

        public virtual void RotationHandler()
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