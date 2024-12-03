using App.Resource.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.obj
{
    public class ExplosionScript : NetworkBehaviour
    {
        public float _damage = 40;

        [Rpc(target: SendTo.Server)]
        public void EndAnimRpc()
        {
            NetworkObject.Despawn();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                Debug.Log(message: "Player hit!");
                other.gameObject.GetComponent<HealthNetScript>().DamageObjRpc(_damage);
            }
        }
    }
}
    