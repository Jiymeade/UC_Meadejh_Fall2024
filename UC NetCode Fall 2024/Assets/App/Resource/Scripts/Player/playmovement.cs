using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace App.Resource.Scripts.Player

{


    public class playmovement : MonoBehaviour
    {
        // Start is called before the first frame update
        public void UPdate()
        {
            Vector3 moveDirection = new Vector3(x: 0, y: 0, z: 0);
            if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
            if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
            if (Input.GetKey(KeyCode.A)) moveDirection.z = -1f;
            if (Input.GetKey(KeyCode.D)) moveDirection.z = +1f;

            float moveSpeed = 3f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);

        }


    }
}
