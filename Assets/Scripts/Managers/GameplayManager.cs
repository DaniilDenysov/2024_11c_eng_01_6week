using DesignPatterns.Singleton;
using System;
using System.Collections;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;
using Steamworks;
using UnityEngine.Events;

namespace Managers
{
    public class GameplayManager : NetworkSingleton<GameplayManager>
    {
        public static Action OnFinish;

        private DefaultInputActions inputActions;

        [SerializeField] private ViewAnimationHandler pause, info;
        [SerializeField] private AudioClip gameFinishedSound;
        [SerializeField] private AudioEventChannel eventChannel;

        private bool pauseMenuOpen, characterInfoMenuOpen;
        [SerializeField] private Transform finishWindow;




        public override void Awake()
        {
            base.Awake();
            inputActions = new DefaultInputActions();
            inputActions.Enable();
            inputActions.Player.PauseMenu.performed += OnPause;
            inputActions.Player.CharacterInfo.performed += OnCharacterInfo;
            OnFinish += OnGameFinished;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnFinish -= OnGameFinished;
            inputActions.Player.PauseMenu.performed -= OnPause;
            inputActions.Player.CharacterInfo.performed -= OnCharacterInfo;
        }

        private void OnPause(InputAction.CallbackContext obj)
        {
            HandleAnimation(pause);
        }

        private void OnCharacterInfo(InputAction.CallbackContext obj)
        {
            HandleAnimation(info);
        }

        private void HandleAnimation (ViewAnimationHandler handler)
        {
            if (handler.IsOpened())
            {
                handler.CloseWindow();
            }
            else
            {
                handler.OpenWindow();
            }
        }

        public void OnGameFinished()
        {
            eventChannel.RaiseEvent(gameFinishedSound);
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