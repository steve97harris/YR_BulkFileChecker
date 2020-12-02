using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class FileInfoPanel : MonoBehaviour
    {
        public static FileInfoPanel Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void DestroyFileInfoPanel()
        {
            Destroy(this.gameObject);
        }
    }
}