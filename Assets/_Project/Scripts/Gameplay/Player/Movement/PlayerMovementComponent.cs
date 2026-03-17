using _Project.Scripts.Config.PlayerConfig;
using UnityEngine;
using Zenject;
using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Player.PlayerStats;

namespace _Project.Scripts.Gameplay.Player.Movement
{
    public class PlayerMovementComponent : IInitializable, ITickable, IFixedTickable
    {
        [Inject] private IInputService _inputService;
        [Inject] private PlayerMovementConfig _settings;
        [Inject] private IPlayerStatsService _playerStatsService;
        [Inject(Optional = true)] private SignalBus _signalBus;

        [Inject(Id = PlayerCharacterInstaller.PLAYER_RIGIDBODY)]
        private Rigidbody _rb;

        [Inject(Id = PlayerCharacterInstaller.PLAYER_TRANSFORM)]
        private Transform _transform;

        [Inject(Id = PlayerCharacterInstaller.CAMERA_HOLDER)]
        private Transform _cameraHolder;

        private PlayerActionsProvider _actions;
        private bool _isMovementEnabled = true;

        private float _verticalRotation = 0f;
        private bool _isGrounded;
        private RaycastHit _slopeHit;

        private float _jumpTimer;

        public void Initialize()
        {
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            _rb.useGravity = true;

            _actions = _inputService.GetActionsProvider();

            if (_signalBus != null)
            {
                _signalBus.Subscribe<PlayerStatsScreenOpenedSignal>(HandleStatsScreenOpened);
                _signalBus.Subscribe<PlayerStatsScreenClosedSignal>(HandleStatsScreenClosed);
            }
            
#if !UNITY_ANDROID && !UNITY_IPHONE
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
        }

        private void HandleStatsScreenOpened(PlayerStatsScreenOpenedSignal signal)
        {
            SetMovementEnabled(false);
        }

        private void HandleStatsScreenClosed(PlayerStatsScreenClosedSignal signal)
        {
            SetMovementEnabled(true);
        }

        public void Tick()
        {
            if (!_isMovementEnabled)
            {
                return;
            }

            HandleLook();
        }

        public void FixedTick()
        {
            if (!_isMovementEnabled)
            {
                return;
            }

            CheckGrounded();
            HandleJump();
            HandleMovement();

            _jumpTimer += Time.fixedDeltaTime;

            _rb.drag = _isGrounded ? _settings.LinearDamping : 0.05f;
        }

        public void SetMovementEnabled(bool enabled)
        {
            _isMovementEnabled = enabled;

            if (!enabled)
            {
                _rb.velocity = Vector3.zero;
            }
        }

        private void CheckGrounded()
        {
            if (_jumpTimer < 0.1f)
            {
                _isGrounded = false;
                return;
            }

            var origin = _transform.position + Vector3.up * 0.2f;

            _isGrounded = Physics.SphereCast(
                origin,
                _settings.GroundCheckRadius,
                Vector3.down,
                out _slopeHit,
                _settings.GroundCheckOffset,
                _settings.GroundLayer);

            if (_isGrounded)
            {
                var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                if (angle > _settings.MaxSlopeAngle) _isGrounded = false;
            }
        }

        private void HandleLook()
        {
            var lookInput = _actions.LookVector * _settings.MouseSensitivity;

            var deltaRotation = Quaternion.Euler(Vector3.up * lookInput.x);
            _rb.MoveRotation(_rb.rotation * deltaRotation);

            _verticalRotation -= lookInput.y;
            _verticalRotation = Mathf.Clamp(_verticalRotation, _settings.MinLookAngle, _settings.MaxLookAngle);
            _cameraHolder.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }

        private void HandleMovement()
        {
            var moveInput = new Vector3(_actions.MoveVector.x, 0, _actions.MoveVector.y);
            var moveDir = _transform.forward * moveInput.z + _transform.right * moveInput.x;

            var baseTargetSpeed = _actions.Sprint ? _settings.SprintSpeed : _settings.WalkSpeed;

            var speedMultiplier = 1f;
            if (_playerStatsService != null)
            {
                speedMultiplier = _playerStatsService.GetStatValue(CharacterStatType.Speed);
            }

            var targetSpeed = baseTargetSpeed * speedMultiplier;

            if (_isGrounded)
            {
                var slopeDir = Vector3.ProjectOnPlane(moveDir, _slopeHit.normal).normalized;

                var targetVelocity = slopeDir * targetSpeed;
                var currentVelocity = _rb.velocity;

                var accel = (moveInput.sqrMagnitude > 0.01f) ? _settings.Acceleration : _settings.Deceleration;

                var velocityChange = targetVelocity - currentVelocity;

                if (moveInput.sqrMagnitude < 0.01f && OnSlope())
                {
                    _rb.AddForce(-_slopeHit.normal * 10f, ForceMode.Force);
                }

                _rb.AddForce(velocityChange * accel, ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(moveDir * targetSpeed * _settings.AirControlMultiplier, ForceMode.Acceleration);

                var horizontalVel = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                if (horizontalVel.magnitude > targetSpeed)
                {
                    horizontalVel = horizontalVel.normalized * targetSpeed;
                    _rb.velocity = new Vector3(horizontalVel.x, _rb.velocity.y, horizontalVel.z);
                }
            }
        }

        private bool OnSlope()
        {
            if (!_isGrounded)
            {
                return false;
            }

            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle > 0 && angle <= _settings.MaxSlopeAngle;
        }

        private void HandleJump()
        {
            if (_actions.Jump)
            {
                if (_isGrounded)
                {
                    _jumpTimer = 0f;

                    _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
                    _rb.AddForce(Vector3.up * _settings.JumpForce, ForceMode.Impulse);
                }
            }
        }
    }
}