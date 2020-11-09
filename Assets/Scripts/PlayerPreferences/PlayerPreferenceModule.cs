using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.InputFields;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
    public class PlayerPreferenceModule : MonoBehaviour
    {
        public static PlayerPreferenceModule Instance { get; set; }

        private const string ColumnHeaderKey = "COLUMN_HEADER";
        private const string FileTypeKey = "FILE_TYPE";
        private const string RepoPathKey = "REPO_PATH";
        private const string CsvPathKey = "CSV_PATH";

        private const string DesignerPathKey = "DESIGNER_PATH";
        private const string RenderServerPathKey = "RENDER_SERVER_PATH";

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            StartCoroutine(RetrievePreviousInputValues());
        }

        private IEnumerator RetrievePreviousInputValues()
        {
            yield return new WaitForSeconds(0.1f);
            GetPlayerPrefs(Function.Regular);
        }

        public enum Function
        {
            Regular,
            Comparison
        }

        #region InputField

        public void SetPlayerPrefs(Function function)
        {
            switch (function)
            {
                case Function.Regular:
                    PlayerPrefs.SetString(ColumnHeaderKey, FileChecker.ColumnHeader);
                    PlayerPrefs.SetString(FileTypeKey, FileChecker.FileType);
                    PlayerPrefs.SetString(RepoPathKey, FileChecker.RepoPath);
                    PlayerPrefs.SetString(CsvPathKey, FileChecker.CsvFilePath);
                    break;
                case Function.Comparison:
                    PlayerPrefs.SetString(DesignerPathKey, FileChecker.DesignerContentPath);
                    PlayerPrefs.SetString(RenderServerPathKey, FileChecker.RenderServerContentPath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(function), function, null);
            }
            
        }

        public void GetPlayerPrefs(Function function)
        {
            switch (function)
            {
                case Function.Regular:
                    var inputMap = new Dictionary<string, GameObject>
                    {
                        {ColumnHeaderKey, ColumnHeaderInput.Instance.gameObject},
                        {FileTypeKey, FileTypeInput.Instance.gameObject},
                        {RepoPathKey, RepoInput.Instance.gameObject},
                        {CsvPathKey, CsvInput.Instance.gameObject}
                    };
                    foreach (var pair in inputMap)
                    {
                        GetPlayerPrefsHelper(pair.Key, pair.Value);
                    }
                    break;
                case Function.Comparison:
                    var comparisonInputMap = new Dictionary<string, GameObject>
                    {
                        {DesignerPathKey, DesignerInput.Instance.gameObject},
                        {RenderServerPathKey, RenderServerInput.Instance.gameObject}
                    };
                    foreach (var pair in comparisonInputMap)
                    {
                        GetPlayerPrefsHelper(pair.Key, pair.Value);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(function), function, null);
            }
        }

        private void GetPlayerPrefsHelper(string playerPrefsKey, GameObject inputField)
        {
            var preferenceValue = PlayerPrefs.GetString(playerPrefsKey);
            if (preferenceValue != null)
                RestoreInputValue(inputField, preferenceValue);
        }

        private void RestoreInputValue(GameObject inputFieldObj, string storedValue)
        {
            var textObj = inputFieldObj.transform.GetChild(0).gameObject;

            textObj.GetComponent<TMP_InputField>().text = storedValue;
            
            Debug.LogError(storedValue);
        }

        #endregion
    }
}