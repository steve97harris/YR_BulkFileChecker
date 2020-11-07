using UnityEngine;

namespace DefaultNamespace
{
    public class ContentArtwork : MonoBehaviour
    {
        public static ContentArtwork Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}