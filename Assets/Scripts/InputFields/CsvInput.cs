using UnityEngine;

namespace DefaultNamespace.InputFields
{
    public class CsvInput : MonoBehaviour
    {
        public static CsvInput Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}