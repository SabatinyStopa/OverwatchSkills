using UnityEngine;

namespace OverwatchClone.Player
{
    public class WeaponSway : MonoBehaviour
    {
        [SerializeField] private float swayMultiplier = 1f;
        [SerializeField] private float smooth = 0.1f;
        [SerializeField] private FirstPersonController controller;

        private void Update() => Sway();

        private void Sway()
        {
            var mouseX = controller.Controls.gameplay.look.ReadValue<Vector2>().x * swayMultiplier;
            var mouseY = controller.Controls.gameplay.look.ReadValue<Vector2>().y * swayMultiplier;

            Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
    }
}