using UnityEngine;

namespace DefaultNamespace.InputFields
{
    public class FileTypeInput : MonoBehaviour
    {
        public static FileTypeInput Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}