using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterMovement))]
public class EnemyFollowTarget : MonoBehaviour {

    [SerializeField]
    Transform target;
    [SerializeField]
    float range;

	void Update () {
        Vector3 direction = target.position - transform.position;
        GetComponent<CharacterMovement>().Move(direction);
	}
}
