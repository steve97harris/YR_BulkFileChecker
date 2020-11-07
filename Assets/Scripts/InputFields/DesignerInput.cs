using UnityEngine;

namespace DefaultNamespace.InputFields
{
    public class DesignerInput : MonoBehaviour
    {
        public static DesignerInput Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}