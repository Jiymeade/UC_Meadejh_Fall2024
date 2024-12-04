using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Script.Obj
{
    public class TntScript : NetworkBehaviour
    {
        public float _fuseTimer = 4f;
        public NetworkObject ExplosionPrefab;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                StartCoroutine("TriggerFuse");
            }
        }

        private IEnumerator TriggerFuse()
        {
            Debug.Log("Fuse List");
            yield return new WaitForSeconds(_fuseTimer);
            TriggerExplosionRpc();
        }

        [Rpc(target: SendTo.Server)]
        public void TriggerExplosionRpc()
        {
            NetworkObject explosive = NetworkManager.Instantiate(ExplosionPrefab, transform.position, transform.rotation);

            explosive.Spawn(true);
            this.NetworkObject.Despawn();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag.Equals("Projectile") || other.gameObject.tag.Equals("Explosion"))
            {
                TriggerExplosionRpc();
            }
        }
    }
}
