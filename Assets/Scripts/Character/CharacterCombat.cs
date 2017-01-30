using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCombat : MonoBehaviour {

    [SerializeField]
    Hitbox attackHitbox;

	public void Attack(bool isAttacking)
    {
        GetComponentInChildren<Animator>().SetBool("Attack", isAttacking);
    }

    public void Roll(bool isRolling)
    {
        GetComponentInChildren<Animator>().SetBool("Roll", isRolling);
    }

    public void AttackHitboxSetActive(int active)
    {
        attackHitbox.gameObject.SetActive(active != 0);
    }
}
