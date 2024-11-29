using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace YaGamesSDK
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        public static MainThreadDispatcher _instance;
        private readonly ConcurrentQueue<Action> _actions = new();

        public static MainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("MainThreadDispatcher");
                    _instance = go.AddComponent<MainThreadDispatcher>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public void Enqueue(Action action)
        {
            _actions.Enqueue(action);
        }

        private void Update()
        {
            while (_actions.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }
}
