using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class TntSpawner : NetworkBehaviour
{
    public NetworkObject Tnt;

    [SerializeField] private float tickTime = 6f;
    [SerializeField] private float currentTime = 6f;

    public void FixedUpdate()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            SpawnTntRpc();

            currentTime = Random.Range(tickTime, tickTime + 5f);
        }
    }

    [ContextMenu(itemName:"SpawnTNT")]
    [Rpc(target:SendTo.Server)]

    public void SpawnTntRpc()
    {
        NetworkObject tnt = NetworkManager.Instantiate(Tnt, transform.position, transform.rotation);
        tnt.Spawn(true);
    }
        
}
