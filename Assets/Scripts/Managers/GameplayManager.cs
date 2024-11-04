using DesignPatterns.Singleton;
using System;
using System.Collections;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameplayManager : NetworkSingleton<GameplayManager>
    {
        public static Action OnFinish;

        private DefaultInputActions inputActions;

        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Transform finishWindow;


        public override void Awake()
        {
            base.Awake();
            OnFinish += OnGameFinished;
            inputActions = new DefaultInputActions();
            inputActions.Enable();
            inputActions.Player.PauseMenu.performed += OnPause;
        }

        private void OnDestroy()
        {
            OnFinish -= OnGameFinished;
            inputActions.Player.PauseMenu.performed -= OnPause;
        }

        private void OnPause(InputAction.CallbackContext obj)
        {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }

        [ClientRpc]
        public void OnGameFinished()
        {
            finishWindow.gameObject.SetActive(true);
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }



        public override GameplayManager GetInstance()
        {
            return this;
        }
    }
}