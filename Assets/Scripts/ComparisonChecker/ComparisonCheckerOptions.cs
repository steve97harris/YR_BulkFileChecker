using UnityEngine;

namespace DefaultNamespace
{
    public class ComparisonCheckerOptions : MonoBehaviour
    {
        public static ComparisonCheckerOptions Instance
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