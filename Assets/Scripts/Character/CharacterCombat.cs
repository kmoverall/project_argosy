using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCombat : MonoBehaviour {

	public void Attack(bool isAttacking)
    {
        GetComponentInChildren<Animator>().SetBool("Attack", isAttacking);
    }

    public void Roll(bool isRolling)
    {
        GetComponentInChildren<Animator>().SetBool("Roll", isRolling);
    }
}
