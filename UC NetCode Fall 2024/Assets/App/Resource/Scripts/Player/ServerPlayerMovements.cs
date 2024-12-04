using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using App.Resource.Scripts.obj;

[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovements : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetAnimator;
    [SerializeField] private float _pSpeed;
    [SerializeField] private BulletSpawnerScript _bulletSpawner;
    public CharacterController _CC;
    private MyPlayerInputActions _playerInput;
    private static readonly int IsSprinting = Animator.StringToHash("isSprinting");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    Vector3 _moveDirection = new Vector3(x: 0, y: 0f, z: 0);

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

        if (isPunching)
        {
            _bulletSpawner.FireProjectileRpc();
        }
    }

    private void Move(Vector2 _input, bool isPunching, bool isSprinting, bool isJumping)
    {
      
        _moveDirection = new Vector3(_input.x, y: 0f, z: _input.y);
       

        _myAnimator.SetBool(id:IsWalking, value: _input.x != 0 || _input.y != 0);

        if (isJumping) { _myNetAnimator.SetTrigger("JumpTrigger"); }
        if(isPunching){_myNetAnimator.SetTrigger("PunchTrigger"); }

       _myAnimator.SetBool(IsSprinting, isSprinting);

        if (_input.x == 0f && _input.y == 0f) return;

        if (isSprinting)
        {

            _CC.Move(_moveDirection * (_pSpeed * 1.3f * Time.deltaTime));
        }
        else
        {

            _CC.Move(_moveDirection * (_pSpeed * Time.deltaTime));
        }
      
        transform.forward = _moveDirection;
    }

    [Rpc(target:SendTo.Server)]
    private void MoveServerRPC(Vector2 _input, bool isPunching, bool isSprinting, bool isJumping)
    {
        Move(_input, isPunching, isSprinting, isJumping);
    }
}
