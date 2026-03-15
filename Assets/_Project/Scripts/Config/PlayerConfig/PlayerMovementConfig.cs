using UnityEngine;

namespace _Project.Scripts.Config.PlayerConfig
{
    [CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "Configs/Player/Movement Config")]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("Walking & Sprinting")]
        public float WalkSpeed = 6f;
        public float SprintSpeed = 10f;
        public float LinearDamping = 6f;
        public float Acceleration = 10f;
        public float Deceleration = 100f;
        
        [Header("Jumping & Air")]
        public float JumpForce = 6f;
        public float JumpCooldown = 0.2f;
        public float AirControlMultiplier = 0.25f;

        [Header("Physics/Environment")]
        public LayerMask GroundLayer;
        public float GroundCheckRadius = 0.3f;
        public float GroundCheckOffset = 0.1f;
        public float MaxSlopeAngle = 45f;

        [Header("Mouse Look")]
        public float MouseSensitivity = 0.1f;
        public float MinLookAngle = -85f;
        public float MaxLookAngle = 85f;
    }
}