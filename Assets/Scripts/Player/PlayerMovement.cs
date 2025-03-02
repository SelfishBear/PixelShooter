using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _maxVelocityChange = 10.0f;
        [SerializeField] private float _sprintSpeed = 1.5f;
        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _speedBunnyModifier = 1.0f;
        [SerializeField] private float _timeBetweenBunnyHop = 0.5f;

        private bool _isTimerElapsed;
        private float _currentSpeed;
        private bool _isBunnyHopping;
        private float _lastLandTime;
        private Rigidbody _rigidbody;
        private bool _isSprinting;
        private bool _isGrounded;
        private Vector2 _playerInput;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            _isSprinting = (Input.GetKey(KeyCode.LeftShift) && _isGrounded);
            CheckGrounded();

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                Jump();
            }
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            Vector3 targetVelocity = new Vector3(_playerInput.x, 0f, _playerInput.y);
            targetVelocity = transform.TransformDirection(targetVelocity);
            _currentSpeed = _isSprinting ? _movementSpeed * _sprintSpeed : _movementSpeed;

            if (_isBunnyHopping)
            {
                _currentSpeed *= _speedBunnyModifier;
            }

            targetVelocity *= _currentSpeed;

            Vector3 velocity = _rigidbody.velocity;
            Vector3 velocityChange = targetVelocity - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
            velocityChange.y = 0f;

            _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        public void Jump()
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            if (_timeBetweenBunnyHop > _lastLandTime && _isGrounded)
            {
                _isBunnyHopping = true;
                _speedBunnyModifier = Mathf.Clamp(_speedBunnyModifier += 0.5f, _speedBunnyModifier, 5.0f);
            }
            else if (_timeBetweenBunnyHop < _lastLandTime && _isGrounded)
            {
                _isBunnyHopping = false;
                _speedBunnyModifier = 0.1f;
            }
        }

        private void CheckGrounded()
        {
            float raycastDistance = 1.2f;
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, raycastDistance, _groundLayer);
            CheckJumpTimer();
            if (_isGrounded && _timeBetweenBunnyHop < _lastLandTime)
            {
                _isBunnyHopping = false;
            }

            Debug.DrawRay(transform.position, Vector3.down * raycastDistance, Color.red);
            Debug.Log(_isGrounded);
        }

        private void CheckJumpTimer()
        {
            if (_isGrounded)
            {
                _lastLandTime += Time.deltaTime;
            }
            else
            {
                _lastLandTime = 0.0f;
            }
        }
    }
}