using DesignPatterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    public override InputManager GetInstance()
    {
        return this;
    }

    private InputActions inputActions;

    public override void Awake()
    {
        base.Awake();
        inputActions = new InputActions();
        inputActions.Enable();
    }

    public void Subscribe(Action<InputAction.CallbackContext> func)
    {
        inputActions.Player.LeftMouseButton.performed += func;
    }

    public void Unsubscribe(Action<InputAction.CallbackContext> func)
    {
        inputActions.Player.LeftMouseButton.performed -= func;
    }

    /*  public void SubscribeOneTime(Action func)
      {
          inputActions.Player.LeftMouseButton.performed += new TemporarySubsciber(func,Unsubscribe).OnTriggered;
      }

      public void Unsubscribe (TemporarySubsciber subsciber)
      {
          inputActions.Player.LeftMouseButton.performed -= subsciber.OnTriggered;
      }

      public class TemporarySubsciber
      {
          private Action func;
          private Action<TemporarySubsciber> triggered;

          public TemporarySubsciber(Action func, Action<TemporarySubsciber> triggered)
          {
              this.func = func;
              this.triggered = triggered;
          }

          public void OnTriggered(InputAction.CallbackContext obj)
          {
              func?.Invoke();
              triggered?.Invoke(this);
          }
      }*/
}
