using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class RegularCheckerOptions : MonoBehaviour
    {
        public static RegularCheckerOptions Instance
        {
            get;
            set;
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}