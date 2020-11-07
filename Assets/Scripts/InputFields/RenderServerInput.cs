using UnityEngine;

namespace DefaultNamespace.InputFields
{
    public class RenderServerInput : MonoBehaviour
    {
        public static RenderServerInput Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}