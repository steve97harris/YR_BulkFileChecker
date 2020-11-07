using UnityEngine;

namespace DefaultNamespace.InputFields
{
    public class RepoInput : MonoBehaviour
    {
        public static RepoInput Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}