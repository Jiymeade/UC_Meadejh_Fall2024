using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace App.Resource.Scripts.Player

{


    public class playmovement : NetworkBehaviour
    {
        // Start is called before the first frame update
        public void Update()
        {
            if (!IsOwner) return;

            Vector3 moveDirection = new Vector3(x:0, y:0, z:0);
            if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
            if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
            if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

            float moveSpeed = 3f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);

        }


    }
}
