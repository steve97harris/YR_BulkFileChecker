using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class ErrorMessageFunctions : MonoBehaviour
    {
        public static ErrorMessageFunctions Instance { get; set; }

        private const string ERROR101_PREFAB = "Prefabs/error101";
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void DisplayError(string error)
        {
            var errorObj = Resources.Load<GameObject>(ERROR101_PREFAB);
            errorObj = Instantiate(errorObj, MainCanvas.Instance.transform);
            
            if (errorObj.GetComponent<TMP_Text>().text == "_error_")
                errorObj.GetComponent<TMP_Text>().text = error;
            else
                errorObj.GetComponent<TMP_Text>().text += Environment.NewLine + error;

            errorObj.SetActive(true);

            StartCoroutine(ExitErrorMessage());
        }

        private IEnumerator ExitErrorMessage()
        {
            yield return new WaitForSeconds(3f);
            
            var errorObj = Error101.Instance.gameObject;
            Destroy(errorObj);
        }
    }
}