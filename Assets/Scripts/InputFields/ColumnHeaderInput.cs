using System;
using UnityEngine;

namespace DefaultNamespace.InputFields
{
    public class ColumnHeaderInput : MonoBehaviour
    {
        public static ColumnHeaderInput Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}