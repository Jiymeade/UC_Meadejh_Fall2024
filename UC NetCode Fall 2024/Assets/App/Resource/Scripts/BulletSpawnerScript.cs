using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.obj
{
    public class BulletSpawnerScript : NetworkBehaviour
    {
        [SerializeField] public NetworkVariable<int> _ammo = new NetworkVariable<int>(4);
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private NetworkObject _ProjectilePrefab;

        [Rpc(target: SendTo.Server, RequireOwnership = false)]
        public void FireProjectileRpc(RpcParams rpcParams = default)
        {
            if (_ammo.Value > 0)
            {
                NetworkObject newProjectile =
                    NetworkManager.Instantiate(_ProjectilePrefab, _startingPoint.position, _startingPoint.rotation);

                newProjectile.SpawnWithOwnership(rpcParams.Receive.SenderClientId);

                _ammo.Value--;
            }
        }
    }

}
