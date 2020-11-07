using System;
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
        }

        private void Start()
        {
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
            }
            
        }

        public void GetPlayerPrefs(Function function)
        {
            switch (function)
            {
                case Function.Regular:
                    var columnHeader = PlayerPrefs.GetString(ColumnHeaderKey);
                    var fileType = PlayerPrefs.GetString(FileTypeKey);
                    var repoPath = PlayerPrefs.GetString(RepoPathKey);
                    var csvPath = PlayerPrefs.GetString(CsvPathKey);
                    
                    if (columnHeader != null)
                    {
                        if (ColumnHeaderInput.Instance != null)
                            RestorePreviousInputFieldValue(ColumnHeaderInput.Instance.gameObject, columnHeader);
                    }
                    if (fileType != null)
                    {
                        if (FileTypeInput.Instance != null)
                            RestorePreviousInputFieldValue(FileTypeInput.Instance.gameObject, fileType);
                    }
                    if (repoPath != null)
                    {
                        if (RepoInput.Instance != null)
                            RestorePreviousInputFieldValue(RepoInput.Instance.gameObject, repoPath);
                    }
                    if (csvPath != null)
                    {
                        if (CsvInput.Instance != null)
                            RestorePreviousInputFieldValue(CsvInput.Instance.gameObject, csvPath);
                    }
                    break;
                case Function.Comparison:
                    var designerPath = PlayerPrefs.GetString(DesignerPathKey);
                    var renderPath = PlayerPrefs.GetString(RenderServerPathKey);

                    if (designerPath != null)
                    {
                        if (DesignerInput.Instance != null)
                        {
                            RestorePreviousInputFieldValue(DesignerInput.Instance.gameObject, designerPath);
                        }
                    }
                    if (renderPath != null)
                    {
                        if (RenderServerInput.Instance != null)
                        {
                            RestorePreviousInputFieldValue(RenderServerInput.Instance.gameObject, renderPath);
                        }
                    }
                    break;
            }
        }

        private void RestorePreviousInputFieldValue(GameObject inputFieldObj, string storedValue)
        {
            var textObj = inputFieldObj.transform.GetChild(0).gameObject;

            textObj.GetComponent<TMP_InputField>().text = storedValue;
        }

        #endregion
    }
}