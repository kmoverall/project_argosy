using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class CharacterControl : MonoBehaviour
{

    CharacterController controller;

    [SerializeField]
    private Transform character;
    [SerializeField]
    private Animator characterAnimator;
    [SerializeField]
    private Transform cameraRoot;

    [SerializeField]
    private float maxWalkSpeed = 5f;
    [SerializeField]
    private float walkAcceleration = 10f;
    [SerializeField]
    private float walkDeceleration = 20f;
    [SerializeField]
    private float gravityScale = 1f;
    [SerializeField]
    private float jumpVelocity = 9f;
    [SerializeField]
    private float terminalVelocity = 40f;
    [SerializeField]
    private float snapDistance = 0.25f;

    private Vector3 currentMotion;
    public bool freezeMotion = false;

    public void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Update()
    {
        Vector3 input = cameraRoot.forward * Input.GetAxis("Vertical");
        input += cameraRoot.right * Input.GetAxis("Horizontal");
        input = Vector3.ClampMagnitude(input, 1);
        Move(input);
    }

    public void Move(Vector3 input)
    {
        if (!controller.isGrounded)
            return;

        //Model should face in input direction
        if (input.magnitude != 0)
            character.transform.rotation = Quaternion.LookRotation(input, Vector3.up);

        
        Vector3 planeMotion = new Vector3(currentMotion.x, 0, currentMotion.z);
        Vector3 acceleration = Vector3.zero;

        acceleration = -1 * planeMotion.normalized * walkDeceleration;
        acceleration += input * (walkAcceleration + walkDeceleration);

        if (Vector3.Dot(acceleration * Time.deltaTime + planeMotion, planeMotion) < 0 && input.magnitude == 0)
        {
            planeMotion = Vector3.zero;
        }
        else
        {
            planeMotion += acceleration * Time.deltaTime;
            //Clamp to max speed
            planeMotion = Vector3.ClampMagnitude(planeMotion, maxWalkSpeed);
        }

        //Handle animation parameter
        characterAnimator.SetFloat("Forward", planeMotion.magnitude / maxWalkSpeed);

        currentMotion = planeMotion;
    }

    void FixedUpdate()
    {
        if (freezeMotion)
        {
            currentMotion = Vector3.zero;
            return;
        }

        currentMotion += Physics.gravity * gravityScale * Time.deltaTime;

        currentMotion = Vector3.ClampMagnitude(currentMotion, terminalVelocity);

        controller.Move(currentMotion * Time.deltaTime);

        if (controller.isGrounded == false)
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hitInfo, snapDistance))
            {
                controller.Move(hitInfo.point - transform.position);
            }
        }

        characterAnimator.SetBool("OnGround", controller.isGrounded);

        // Set the vertical animation
        characterAnimator.SetFloat("Jump", controller.velocity.y);
    }
}
