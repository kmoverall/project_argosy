using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Isometric
{
    public class IsoPlayerInput : MonoBehaviour
    {

        public CharacterMovement movementTarget;
        public CharacterCombat combatTarget;
        [SerializeField]
        private Transform cameraRoot;

        [SerializeField]
        string horizontalAxis;
        [SerializeField]
        string verticalAxis;
        [SerializeField]
        KeyCode jumpKey;
        [SerializeField]
        KeyCode attackKey;
        [SerializeField]
        KeyCode rollKey;

        public void Update()
        {
            Vector3 input = cameraRoot.forward * Input.GetAxisRaw(verticalAxis);
            input += cameraRoot.right * Input.GetAxisRaw(horizontalAxis);
            input = Vector3.ClampMagnitude(input, 1);
            movementTarget.Move(input);

            if (Input.GetKeyDown(jumpKey))
            {
                movementTarget.Jump();
            }

            combatTarget.Attack(Input.GetKeyDown(attackKey));
            combatTarget.Roll(Input.GetKeyDown(rollKey));
        }
    }
}
