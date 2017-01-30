using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterHealth : MonoBehaviour {

    [SerializeField]
    float maxHealth;

    [ReadOnly]
    public float currentHealth;

    bool vulnerable = true;

    public void SetVulnerable(int isVulnerable) { vulnerable = isVulnerable != 0; }

    void Awake () {
        currentHealth = maxHealth;
    }
	
	void Update () {
	    if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
	}

    public void Damage(float amount) {
        if (!vulnerable)
            return;

        currentHealth -= amount;
        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().SetTrigger("Hurt");
        }
    }
}
