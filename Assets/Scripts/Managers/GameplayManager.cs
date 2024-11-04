using DesignPatterns.Singleton;
using System;
using System.Collections;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GameplayManager : NetworkSingleton<GameplayManager>
    {
        public static Action OnFinish;


        [SerializeField] private Transform finishWindow;


        public override void Awake()
        {
            base.Awake();
            OnFinish += OnGameFinished;
        }



        [ClientRpc]
        public void OnGameFinished()
        {
            finishWindow.gameObject.SetActive(true);
        }

        public override GameplayManager GetInstance()
        {
            return this;
        }
    }
}