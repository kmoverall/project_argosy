using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterAttack : MonoBehaviour {

	public void Attack(bool isAttacking)
    {
        GetComponentInChildren<Animator>().SetBool("Attack", isAttacking);
    }
}
