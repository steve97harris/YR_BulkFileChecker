using UnityEngine;

namespace DefaultNamespace
{
    public class Error101 : MonoBehaviour
    {
        public static Error101 Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}