using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class CharacterControl : MonoBehaviour {

    [SerializeField]
    private Transform character;
    [SerializeField]
    private Animator characterAnimator;


    [SerializeField]
    private float maxWalkSpeed = 5f;
    [SerializeField]
    private float walkAcceleration = 10f;
    [SerializeField]
    private float walkDeceleration = 20f;
    [SerializeField]
    private float gravityScale = 1f;
    [SerializeField]
    private float terminalVelocity = 40f;

    private Vector3 currentMotion;

    void Start () 
    {
	    
	}
	
	void Update () 
    {
	
	}
}
