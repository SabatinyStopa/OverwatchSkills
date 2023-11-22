using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;

namespace OverwatchClone.Player
{
    public class TracerController : FirstPersonController
    {
        [Header("Time")]
        [SerializeField] private float timeToGoBack = 5f;
        [SerializeField] private float timeToGoBackCooldown = 8f;
        [SerializeField] private float timeTravelTime = 1.2f;
        [Header("Dash")]
        [SerializeField] private int dashQuantity = 3;
        [SerializeField] private float dashMultiplier = 2f;
        [SerializeField] private float dashDuration = 0.5f;
        [SerializeField] private float dashCooldown = 3;

        private float dashTimer = 0;
        private float timeBackTimer = 0;
        private float originalSpeed;
        private bool isDashing = false;
        private int currentDashQuantity;
        private bool canGoBackInTime = true;
        private float registerStepTimer;
        private bool isTimeTraveling = false;

        private Step[] steps = new Step[10];
        private float stepTime = 0.5f;
        private int stepCounter = 0;
        private float allElapsedTime = 0;

        [System.Serializable]
        struct Step
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public float ElapsedTime;
        }

        public override void Start()
        {
            base.Start();
            originalSpeed = moveSpeed;
            currentDashQuantity = dashQuantity;
            steps[0].Position = transform.position;
            steps[0].Rotation = transform.rotation;
            controls.gameplay.E.performed += GoBackInTime;
        }

        private void GoBackInTime(InputAction.CallbackContext context)
        {
            if (!isTimeTraveling && canGoBackInTime) StartCoroutine(TimeTravel());
        }

        public override void Update()
        {
            allElapsedTime += Time.deltaTime;

            if (!isTimeTraveling)
            {
                base.Update();
                RegisterStep();
            }

            HandleSkillCooldown();
        }

        public override void MovimentHandler()
        {
            var direction = controls.gameplay.move.ReadValue<Vector2>();

            if (direction.sqrMagnitude < 0.01) return;

            var scaledMoveSpeed = moveSpeed * Time.deltaTime;
            var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
            transform.position += move * scaledMoveSpeed;

            if (controls.gameplay.shift.WasPressedThisFrame() && !isDashing) StartCoroutine(Dash());
        }

        private void RegisterStep()
        {
            if (registerStepTimer >= stepTime)
            {
                registerStepTimer = 0;
                steps[stepCounter].Position = transform.position;
                steps[stepCounter].Rotation = transform.rotation;
                steps[stepCounter].ElapsedTime = allElapsedTime;

                stepCounter++;

                if (stepCounter >= steps.Length) stepCounter = 0;
            }

            registerStepTimer += Time.deltaTime;
        }

        private void HandleSkillCooldown()
        {
            if (currentDashQuantity < dashQuantity) dashTimer += Time.deltaTime;

            if (!canGoBackInTime) timeBackTimer += Time.deltaTime;


            if (dashTimer >= dashCooldown)
            {
                dashTimer = 0;
                currentDashQuantity++;
                if (currentDashQuantity >= dashQuantity) currentDashQuantity = dashQuantity;
            }

            if (timeBackTimer >= timeToGoBackCooldown)
            {
                timeBackTimer = 0;
                canGoBackInTime = true;
            }
        }

        private IEnumerator TimeTravel()
        {
            isTimeTraveling = true;
            canGoBackInTime = false;

            var elapsedTime = 0f;
            var step = GetClosestStep();

            while (elapsedTime < timeTravelTime)
            {
                var t = elapsedTime / timeTravelTime;

                transform.position = Vector3.Lerp(transform.position, step.Position, t);
                transform.rotation = Quaternion.Lerp(transform.rotation, step.Rotation, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = step.Position;
            transform.rotation = step.Rotation;
            isTimeTraveling = false;
        }

        private Step GetClosestStep()
        {
            var lastSubtracted = Mathf.Infinity;
            var targetNumber = allElapsedTime - timeToGoBack;
            var targetIndex = 0;

            for (int i = 0; i < steps.Length; i++)
            {
                var step = steps[i];
                var number = step.ElapsedTime - targetNumber;

                if (number < 0 || step.Position == Vector3.zero) continue;

                if (lastSubtracted > number)
                {
                    lastSubtracted = number;
                    targetIndex = i;
                }

                lastSubtracted = number;

            }

            return steps[targetIndex];
        }

        private IEnumerator Dash()
        {
            currentDashQuantity--;
            isDashing = true;
            moveSpeed *= dashMultiplier;

            yield return new WaitForSeconds(dashDuration);

            moveSpeed = originalSpeed;
            isDashing = false;
        }
    }
}
