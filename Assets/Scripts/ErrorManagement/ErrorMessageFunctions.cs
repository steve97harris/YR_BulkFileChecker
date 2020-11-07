using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class ErrorMessageFunctions : MonoBehaviour
    {
        public static ErrorMessageFunctions Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void DisplayError(string error)
        {
            var errorObj = Error101.Instance.gameObject;
            if (errorObj == null)
            {
                errorObj = Resources.Load<GameObject>(@"Prefabs\error101");
                errorObj = Instantiate(errorObj, MainCanvas.Instance.transform);
            }

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