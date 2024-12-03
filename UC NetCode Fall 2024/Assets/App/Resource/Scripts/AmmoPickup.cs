using App.Resource.Scripts.obj;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.obj
{
    public class AmmoPickup : NetworkBehaviour
    {
        public void OnCollisionEnter(Collision other)
        {
            if (!IsServer) return;
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<BulletSpawnerScript>()._ammo.Value++;
                Destroy(gameObject);
            }
        }
    }
}

