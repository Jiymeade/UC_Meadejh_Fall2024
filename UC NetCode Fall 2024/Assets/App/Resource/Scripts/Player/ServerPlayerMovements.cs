using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovements : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetAnimator;
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;
    public CharacterController _CC;
    private MyPlayerInputActions _playerInput;

    // Start is called before the first frame update
    void Start()
    {

        if (_myAnimator == null)
        {
            _myAnimator = gameObject.GetComponent<Animator>();
        }
        if (_myNetAnimator == null)
        {
            _myNetAnimator = gameObject.GetComponent<NetworkAnimator>();
        }
        _playerInput = new MyPlayerInputActions();
        _playerInput.Enable();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;
        Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>();

        bool isJumping = _playerInput.Player.Jumping.triggered;
        bool isPunching = _playerInput.Player.Punching.triggered;
        bool isSprinting = _playerInput.Player.Sprinting.triggered;

        if (IsServer)
        {
            Move(moveInput, isPunching, isSprinting, isJumping);
        }
        else if (IsClient)
        {
            MoveServerRPC(moveInput, isPunching, isSprinting, isJumping);
        }
    }

    private void Move(Vector2 _input, bool isPunching, bool isSprinting, bool isJumping)
    {
        Vector3 _moveDirection = _input.x * _pTransform.right + _input.y * _pTransform.forward;

        _myAnimator.SetBool(name: "isWalking", value: _input.x != 0 || _input.y != 0);

        if (isJumping) { _myNetAnimator.SetTrigger("JumpTrigger"); }
        if(isPunching){_myNetAnimator.SetTrigger("PunchTrigger"); }

       _myAnimator.SetBool(name: "isSprinting", isSprinting);

        if (isSprinting)
        {

            _CC.Move(_moveDirection * (_pSpeed * 1.3f) * Time.deltaTime);
        }
        else
        {

            _CC.Move(_moveDirection * _pSpeed * Time.deltaTime);
        }
      

    }

    [Rpc(target:SendTo.Server)]
    private void MoveServerRPC(Vector2 _input, bool isPunching, bool isSprinting, bool isJumping)
    {
        Move(_input, isPunching, isSprinting, isJumping);
    }
}
