using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class MainCanvas : MonoBehaviour
    {
        public static MainCanvas Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}