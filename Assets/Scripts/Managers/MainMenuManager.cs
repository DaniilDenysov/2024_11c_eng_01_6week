using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MainMenuManager : Manager
    {
        public override void InstallBindings()
        {
            Container.Bind<MainMenuManager>().To<MainMenuManager>().AsSingle();
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
