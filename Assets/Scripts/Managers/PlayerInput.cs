using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {

    public CharacterMovement movementTarget;
    public CharacterAttack attackTarget;
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
        
        attackTarget.Attack(Input.GetKeyDown(attackKey));
    }
}
