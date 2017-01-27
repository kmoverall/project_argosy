using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {

    public CharacterMovement target;
    [SerializeField]
    private Transform cameraRoot;

    public void Update()
    {
        Vector3 input = cameraRoot.forward * Input.GetAxisRaw("Vertical");
        input += cameraRoot.right * Input.GetAxisRaw("Horizontal");
        input = Vector3.ClampMagnitude(input, 1);
        target.Move(input);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            target.Jump();
        }
    }
}
