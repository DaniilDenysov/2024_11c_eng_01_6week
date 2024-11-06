using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InputInstaller : MonoInstaller
{
    private DefaultInputActions inputActions;

    public override void InstallBindings()
    {
        inputActions = new DefaultInputActions();
        inputActions.Enable();
        Container.Bind<DefaultInputActions>().To<DefaultInputActions>().FromInstance(inputActions).AsSingle();
    }
}
