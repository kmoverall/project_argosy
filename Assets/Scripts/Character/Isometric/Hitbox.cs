using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Isometric
{
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {

        [SerializeField]
        float damage;

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<CharacterHealth>() != null)
            {
                other.GetComponentInParent<CharacterHealth>().Damage(damage);
            }
        }
    }
}
