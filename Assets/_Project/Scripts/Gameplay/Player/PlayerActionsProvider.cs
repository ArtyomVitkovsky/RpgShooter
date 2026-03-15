using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Gameplay.Player
{
    public class PlayerActionsProvider : InputSystem_Actions.IPlayerActions
    {
        #region Events

        public Action<Vector2> OnMoveAction;
        public Action<Vector2> OnLookAction;
        public Action OnAttackAction;
        public Action OnAimStartAction;
        public Action OnAimEndAction;
        public Action OnInteractStartedAction;
        public Action OnInteractPerformedAction;
        public Action OnCrouchStartAction;
        public Action OnCrouchEndAction;
        public Action OnJumpAction;
        public Action OnSprintAction;
        public Action OnPreviousAction;
        public Action OnNextAction;

        #endregion

        #region Properties
        public Vector2 MoveVector { get; private set; }
        public Vector2 LookVector { get; private set; }
        public bool Attack { get; private set; }
        public bool Aim { get; private set; }
        public bool InteractStarted { get; private set; }
        public bool InteractPerformed { get; private set; }
        public bool Crouch { get; private set; }
        public bool Jump { get; private set; }
        public bool Sprint { get; private set; }
        public bool ToPrevious { get; private set; }
        public bool ToNext { get; private set; }

        #endregion
       
    
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveVector = context.ReadValue<Vector2>();
            OnMoveAction?.Invoke(MoveVector);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookVector = context.ReadValue<Vector2>();
            OnLookAction?.Invoke(LookVector);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            Attack = context.started || context.performed;
            OnAttackAction?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            InteractStarted = context.started;
            InteractPerformed = context.performed;
            
            if (InteractStarted) OnInteractStartedAction?.Invoke();
            if (InteractPerformed) OnInteractPerformedAction?.Invoke();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            Crouch = context.performed;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            Jump = context.performed;
        }
    
        public void OnSprint(InputAction.CallbackContext context)
        {
            Sprint = context.started || context.performed;
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            Aim = context.started || context.performed;
            
            if (context.started) OnAimStartAction?.Invoke();
            if (context.canceled) OnAimEndAction?.Invoke();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            ToPrevious = context.started;
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            ToNext = context.started;
        }
    }
}