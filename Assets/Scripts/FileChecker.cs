using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class FileChecker : MonoBehaviour
    {
        public static DropdownModule.Project ProjectSelected = DropdownModule.Project.Default;
        
        private string _repoPath = "";
        private string _csvFilePath = "";
        private string _columnHeader = "";
        private string _fileType = "";

        private static GameObject _mainCanvas = null;

        private void Start()
        {
            _mainCanvas = Resources.Load<GameObject>(@"Prefabs\CanvasA");
            _mainCanvas = Instantiate(_mainCanvas);
            GetPlayerPrefs();
        }

        public void InputValueRepoPath(string path)
        {
            _repoPath = path;
        }

        public void InputValueCsvPath(string path)
        {
            _csvFilePath = path;
        }
        
        public void InputValueColumnHeader(string columnHeader)
        {
            _columnHeader = columnHeader;
        }

        public void InputValueFileType(string fileType)
        {
            _fileType = fileType;
        }

        private void DisplayError(string error)
        {
            var errorObj = FindSingleObjectByName("error101");
            if (errorObj == null)
            {
                errorObj = Resources.Load<GameObject>(@"Prefabs\error101");
                errorObj = Instantiate(errorObj, _mainCanvas.transform);
            }

            if (errorObj.GetComponent<TMP_Text>().text == "_error_")
                errorObj.GetComponent<TMP_Text>().text = error;
            else
                errorObj.GetComponent<TMP_Text>().text += Environment.NewLine + error;

            errorObj.SetActive(true);
        }

        public void ExitErrorMessage()
        {
            var errorObj = FindSingleObjectByName("error101");
            DestroyImmediate(errorObj, true);
        }

        public void CheckFiles()
        {
            SetPlayerPrefs();
            
            var csvFileList = LoadCsvFileViaPath(_csvFilePath);

            if (csvFileList.Count == 0)
            {
                Debug.LogError("CsvFile returned empty");
                DisplayError("CsvFile returned empty, try another path");
                return;
            }

            var artworkIndex = GetColumnIndex(csvFileList, _columnHeader);
            if (artworkIndex == -1)
            {
                Debug.LogError("No column found with header: " + _columnHeader);
                DisplayError("No column found with header: " + _columnHeader);
                return;
            }

            var subCategoryIndex = GetColumnIndex(csvFileList, "SUB_CATERGORY");
            
            var artworkNameList = GetArtworkNameList(csvFileList, artworkIndex, subCategoryIndex);

            var fileType = "*." + _fileType;
            var files = GetRepositoryFileNames(_repoPath, fileType);

            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log(files[i]);
            }
            
            var content = FindSingleObjectByName("ContentArtwork");
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            var defaultImageTypes = new[]
            {
                "_MSK", "_PRN", "_SCR", "_THM"
            };

            var textGameObj = Resources.Load<GameObject>("Prefabs/ArtworkCheckerTextTemplate");
            for (int i = 0; i < artworkNameList.Count; i++)
            {
                var artworkName = artworkNameList[i].Split(',')[0];
                textGameObj.GetComponent<TMP_Text>().text = artworkName + ": ";

                if (ProjectSelected == DropdownModule.Project.Default)
                {
                    for (int j = 0; j < defaultImageTypes.Length; j++)
                    {
                        CheckArtwork(ProjectSelected, files, artworkNameList, i, textGameObj, defaultImageTypes[j]);
                    }
                }
                if (ProjectSelected == DropdownModule.Project.Levis)
                {
                    CheckArtwork(ProjectSelected, files, artworkNameList, i, textGameObj, ".PNG");
                }

                var subCategory = artworkNameList[i].Split(',')[1];
                textGameObj.GetComponent<TMP_Text>().text += " " + "(" + subCategory + ")";

                Instantiate(textGameObj, content.transform);
            }
        }

        

        private string[] GetRepositoryFileNames(string repoPath, string fileType)
        {
            var repoFiles = Directory.GetFiles(repoPath, fileType, SearchOption.AllDirectories);
            
            fileType = fileType.Replace("*", "");
            
            var fileNames = new List<string>();
            for (int i = 0; i < repoFiles.Length; i++)
            {
                var fileName = Path.GetFileName(repoFiles[i]);
                
                if (fileName.EndsWith(fileType))
                    fileName = fileName.Replace(fileType, "");
                
                fileNames.Add(fileName);
            }
            
            return fileNames.ToArray();
        }

        private List<string> GetArtworkNameList(List<string> csvFileList, int artworkIndex, int subCategoryIndex)
        {
            var artworkNameList = new List<string>();
            for (int i = 0; i < csvFileList.Count; i++)
            {
                var split = csvFileList[i].Split(',');
                var artworkName = split[artworkIndex];
                var subCategory = "";
                if (subCategoryIndex != -1)
                    subCategory = split[subCategoryIndex];
    
                if (artworkName == _columnHeader)
                    continue;

                if (!artworkNameList.Contains(artworkName))
                {
                    var str = artworkName + "," + subCategory;
                    artworkNameList.Add(str);
                }
                else
                {
                    Debug.LogError("ArtworkNameList already contains artwork name, continuing...");
                }
            }

            return artworkNameList;
        }

        private int GetColumnIndex(List<string> csvFileList, string columnHeader)
        {
            var headers = csvFileList[0];
            var headerSplit = headers.Split(',');
            var columnIndex = -1;

            for (int i = 0; i < headerSplit.Length; i++)
            {
                if (headerSplit[i] == columnHeader)
                {
                    columnIndex = i;
                }
            }

            return columnIndex;
        }

        private List<string> LoadCsvFileViaPath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                DisplayError("CSV File Not Found.");
                return null;
            }
            
            var reader = new StreamReader(File.OpenRead(filePath));
            List<string> searchList = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                searchList.Add(line);
            }

            return searchList;
        }
        
        public static GameObject FindSingleObjectByName(string objectName)
        {
            var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == objectName).ToArray();
            return objects.Length == 0 ? null : objects[0];
        }

        private void CheckArtwork(DropdownModule.Project project, string[] files, List<string> artworkNameList, int i, GameObject textGameObj, string imageType)
        {
            var artworkNameSplit = artworkNameList[i].Split(',');
            var artworkName = artworkNameSplit[0];
            var filesWithCurrentArtworkName = new List<string>();
            switch (project)
            {
                case DropdownModule.Project.Default:
                    filesWithCurrentArtworkName = files.Select(x => x).Where(x => String.Equals(x, artworkName + imageType, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case DropdownModule.Project.Levis:
                    filesWithCurrentArtworkName = files.Select(x => x).Where(x => String.Equals(x, artworkName, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
            }

            switch (filesWithCurrentArtworkName.Count)
            {
                case 0:
                    // Debug.LogError("File could not be found with following artwork name: " + artworkName + imageType);
                    textGameObj.GetComponent<TMP_Text>().text +=
                        "<color=red>" + imageType + "</color>, ";
                    break;
                case 1:
                    // Debug.Log("Success, file found!" + Environment.NewLine + " " + _columnHeader + ": " + artworkName + imageType + Environment.NewLine + " FileName: " + filesWithCurrentArtworkName[0]);
                    textGameObj.GetComponent<TMP_Text>().text += "<color=green>"+ imageType +"</color>, ";
                    break;
            }
            if (filesWithCurrentArtworkName.Count > 1)
            {
                // Debug.LogError("Multiple files found with the following name: " + artworkName + imageType);
                textGameObj.GetComponent<TMP_Text>().text +=
                    "<color=yellow>" + imageType + "</color>, ";
            }
        }

        #region InputField

        private void SetPlayerPrefs()
        {
            PlayerPrefs.SetString("COLUMN_HEADER", _columnHeader);
            PlayerPrefs.SetString("FILE_TYPE", _fileType);
            PlayerPrefs.SetString("REPO_PATH", _repoPath);
            PlayerPrefs.SetString("CSV_PATH", _csvFilePath);
        }

        private void GetPlayerPrefs()
        {
            if (PlayerPrefs.GetString("COLUMN_HEADER") != null)
            {
                var columnHeader = PlayerPrefs.GetString("COLUMN_HEADER");
                RestorePreviousInputFieldValue("InputFieldC", columnHeader);
            }
            if (PlayerPrefs.GetString("FILE_TYPE") != null)
            {
                var fileType = PlayerPrefs.GetString("FILE_TYPE");
                RestorePreviousInputFieldValue("InputFieldD", fileType);
            }
            if (PlayerPrefs.GetString("REPO_PATH") != null)
            {
                var repoPath = PlayerPrefs.GetString("REPO_PATH");
                RestorePreviousInputFieldValue("InputFieldA", repoPath);
            }
            if (PlayerPrefs.GetString("CSV_PATH") != null)
            {
                var csvPath = PlayerPrefs.GetString("CSV_PATH");
                RestorePreviousInputFieldValue("InputFieldB", csvPath);
            }
        }
        

        private void RestorePreviousInputFieldValue(string inputFieldObjName, string storedValue)
        {
            var field = FindSingleObjectByName(inputFieldObjName);
            var textObj = field.transform.GetChild(0).gameObject;

            textObj.GetComponent<TMP_InputField>().text = storedValue;
        }

        #endregion
    }
}

