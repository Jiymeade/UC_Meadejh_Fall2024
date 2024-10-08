using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovements : NetworkBehaviour
{
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;
    public CharacterController _CC;
    private MyPlayerInputActions _playerInput;

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = new MyPlayerInputActions();
        _playerInput.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>();

        if (IsServer)
        {
            Move(moveInput);
        }
        else if (IsClient)
        {
            MoveServerRPC(moveInput);
        }
    }

    private void Move(Vector2 _input)
    {
        Vector3 _moveDirection = _input.x * _pTransform.right + _input.y * _pTransform.forward;

        _CC.Move(_moveDirection * _pSpeed * Time.deltaTime);
    }

    [Rpc(target:SendTo.Server)]
    private void MoveServerRPC(Vector2 _input)
    {
        Move(_input);
    }
}
