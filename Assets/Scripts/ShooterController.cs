using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverwatchClone.Player
{
    public class ShooterController : MonoBehaviour
    {
        [SerializeField] private GameObject[] shootingEffects;
        [SerializeField] private Transform impactEffect;
        [SerializeField] private float interval = 0.1f;
        [SerializeField] private float realodingTime = 1.2f;
        [SerializeField] private int magazineCapacity = 40;

        private float intervalTimer = 0;

        private int currentMagazine;
        private SimpleControls controls;
        private bool isRealoding = false;
        private float timeWithoutShoot = 0.1f;
        private float timeNotShooting;

        private void Awake() => controls = new SimpleControls();
        private void OnEnable() => controls.Enable();
        private void OnDisable() => controls.Disable();

        private void Start()
        {
            currentMagazine = magazineCapacity;
            controls.gameplay.reload.performed += Reload;
        }

        private void Update()
        {
            intervalTimer += Time.deltaTime;
            timeNotShooting += Time.deltaTime;

            if (isRealoding) return;

            if (controls.gameplay.fire.IsPressed() && intervalTimer >= interval) Fire();

            if (timeNotShooting >= timeWithoutShoot) StopFire();
        }

        private void StopFire()
        {
            foreach (GameObject shoot in shootingEffects) shoot.SetActive(false);
        }

        private void Fire()
        {
            if (currentMagazine <= 0)
            {
                StartCoroutine(Realoding());
                return;
            }

            timeNotShooting = 0;
            intervalTimer = 0;

            foreach (GameObject shoot in shootingEffects) shoot.SetActive(true);

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, 1000000f))
            {
                var radius = 1.0f;
                var randomOffset = UnityEngine.Random.insideUnitSphere * radius;
                randomOffset.y = 0;
                var randomPosition = hit.point + randomOffset;

                var hitted = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                var hitted2 = Instantiate(impactEffect, randomPosition, Quaternion.identity);

                Destroy(hitted.gameObject, 3f);
                Destroy(hitted2.gameObject, 3f);
            }

            currentMagazine -= 2;
        }

        private void Reload(InputAction.CallbackContext context)
        {
            if (!isRealoding) StartCoroutine(Realoding());
        }

        private IEnumerator Realoding()
        {
            isRealoding = true;
            yield return new WaitForSeconds(realodingTime);
            isRealoding = false;
            currentMagazine = magazineCapacity;
        }
    }
}