using Unity.Mathematics;
using UnityEngine;

namespace OverwatchClone.Player
{
    public class HeadBob : MonoBehaviour
    {
        [SerializeField] private float amount = 0.002f;
        [SerializeField] private float frequency = 10.0f;
        [SerializeField] private float smooth = 10.0f;
        [SerializeField] private FirstPersonController controller;
        private Vector3 startPos;

        private void Start() => startPos = transform.localPosition;

        private void Update() => BobHandler();

        private void BobHandler()
        {
            Bob();
            StopBob();
        }

        private void StopBob()
        {
            if (transform.localPosition == startPos) return;

            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, 1 * Time.deltaTime);
        }

        private void Bob()
        {
            var move = controller.Controls.gameplay.move.ReadValue<Vector2>().magnitude;

            if (move > 0)
            {
                var pos = Vector3.zero;
                pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
                pos.x += math.lerp(pos.x, Mathf.Cos(Time.time * frequency / 2) * amount * 1.6f, smooth * Time.deltaTime);
                transform.localPosition += pos;
            }
        }
    }
}