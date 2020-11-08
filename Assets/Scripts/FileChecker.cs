using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using DefaultNamespace;
using DefaultNamespace.InputFields;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class FileChecker : MonoBehaviour
    {
        public static DropdownModule.Project ProjectSelected = DropdownModule.Project.Default;
        
        public static string RepoPath = "";
        public static string CsvFilePath = "";
        public static string ColumnHeader = "";
        public static string FileType = "";
        public static string DesignerContentPath = "";
        public static string RenderServerContentPath = "";

        private const string CANVAS_A_PATH = "Prefabs/CanvasA";
        private const string FILE_INFO_TEMPLATE = "Prefabs/ArtworkCheckerTextTemplate";
        
        private void Start()
        {
            var mainCanvas = Resources.Load<GameObject>(CANVAS_A_PATH);
            Instantiate(mainCanvas);
        }

        #region Input Field Values

        public void InputValueRepoPath(string value)
        {
            RepoPath = value;
        }

        public void InputValueCsvPath(string value)
        {
            CsvFilePath = value;
        }
        
        public void InputValueColumnHeader(string value)
        {
            ColumnHeader = value;
        }

        public void InputValueFileType(string value)
        {
            FileType = value;
        }

        public void InputValueDesignerPath(string value)
        {
            DesignerContentPath = value;
        }

        public void InputValueRenderServerPath(string value)
        {
            RenderServerContentPath = value;
        }
        
        #endregion

        #region Regular File Check
        
        public void CheckFiles()
        {
            PlayerPreferenceModule.Instance.SetPlayerPrefs(PlayerPreferenceModule.Function.Regular);
            
            var csvFileList = LoadCsvFileViaPath(CsvFilePath);
            
            if (csvFileList == null)
                return;

            if (csvFileList.Count == 0)
            {
                Debug.LogError("CsvFile returned empty");
                ErrorMessageFunctions.Instance.DisplayError("CsvFile returned empty, try another path");
                return;
            }

            var artworkIndex = GetColumnIndex(csvFileList, ColumnHeader);
            if (artworkIndex == -1)
            {
                Debug.LogError("No column found with header: " + ColumnHeader);
                ErrorMessageFunctions.Instance.DisplayError("No column found with header: " + ColumnHeader);
                return;
            }

            var subCategoryIndex = GetColumnIndex(csvFileList, "SUB_CATERGORY");
            
            var artworkNameList = GetArtworkNameList(csvFileList, artworkIndex, subCategoryIndex);

            var fileType = "*." + FileType;
            var files = GetRepositoryFileNames(RepoPath, fileType);

            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log(files[i]);
            }

            var content = ContentArtwork.Instance.gameObject;
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            var defaultImageTypes = new[]
            {
                "_MSK", "_PRN", "_SCR", "_THM"
            };

            var textGameObj = Resources.Load<GameObject>(FILE_INFO_TEMPLATE);
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
        
        #endregion

        #region Compare File Check
        
        public void CompareFiles()
        {
            PlayerPreferenceModule.Instance.SetPlayerPrefs(PlayerPreferenceModule.Function.Comparison);
            
            var content = ContentArtwork.Instance.gameObject;
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }
            
            var textGameObj = Resources.Load<GameObject>(FILE_INFO_TEMPLATE);
            
            // Create two identical or different temporary folders
            // on a local drive and change these file paths.  
            string pathA = DesignerContentPath;  
            string pathB = RenderServerContentPath;

            System.IO.DirectoryInfo designerDir = new System.IO.DirectoryInfo(pathA);  
            System.IO.DirectoryInfo renderDir = new System.IO.DirectoryInfo(pathB);  
  
            // Take a snapshot of the file system.  
            IEnumerable<System.IO.FileInfo> designerList = designerDir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);  
            IEnumerable<System.IO.FileInfo> renderList = renderDir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            //A custom file comparer defined below  
            FileCompare myFileCompare = new FileCompare();  
  
            // This query determines whether the two folders contain  
            // identical file lists, based on the custom file comparer  
            // that is defined in the FileCompare class.  
            // The query executes immediately because it returns a bool.  
            bool areIdentical = designerList.SequenceEqual(renderList, myFileCompare);  
  
            if (areIdentical)  
            {  
                Debug.LogError("the two folders are the same");  
                InstantiateTextObj(textGameObj, content, "The 2 folders are the same", null, null, null);
            }  
            else  
            {  
                Debug.LogError("The two folders are not the same");  
            }  
  
            // Find the common files. It produces a sequence and doesn't
            // execute until the foreach statement.  
            var queryCommonFiles = designerList.Intersect(renderList, myFileCompare);

            var commonFiles = queryCommonFiles as FileInfo[] ?? queryCommonFiles.ToArray();
            
            if (commonFiles.Any())  
            {  
                Debug.Log("The following files are in both folders:");  
                foreach (var v in commonFiles)  
                {  
                    Debug.Log(v.Name + Environment.NewLine + v.FullName); //shows which items end up in result list  
                }  
            }  
            else  
            {  
                Debug.LogError("There are no common files in the two folders.");  
            }

            // Find the set difference between the two folders.  
            // For this example we only check one way.  
            var queryList1Only = (from file in designerList  
                                  select file).Except(renderList, myFileCompare);

            var multipleFileNamesList = new List<string>();
  
            Debug.LogError("The following files are in the designer but not the render server:");  
            foreach (var fileInfo in queryList1Only)  
            {
                if (fileInfo.Name.EndsWith("_MSK.png") || fileInfo.Name.EndsWith("_THM.png"))
                    continue;
                
                if (fileInfo.Name.EndsWith("_SCR.png"))
                {
                    Debug.LogError("File not found, checking for PRN equivalent; " + fileInfo.Name);
                    CheckForPrnEquivalent("_SCR.png", fileInfo, renderList, multipleFileNamesList, textGameObj, content);
                }
                else if (fileInfo.Name.EndsWith("_PNT.png"))
                {
                    Debug.LogError("File not found, checking for PRN equivalent; " + fileInfo.Name);
                    CheckForPrnEquivalent("_PNT.png", fileInfo, renderList, multipleFileNamesList, textGameObj, content);
                }
                else if (fileInfo.Name.EndsWith("_PRN.png"))
                {
                    Debug.LogError("File not found: " + fileInfo.Name + Environment.NewLine + fileInfo.FullName);
                    
                    if (multipleFileNamesList.Contains(fileInfo.Name)) 
                        continue;
                    
                    InstantiateTextObj(textGameObj, content, "<color=red>File not found in render server: " + fileInfo.Name + "</color>", fileInfo.Name, null, null);
                    multipleFileNamesList.Add(fileInfo.Name);
                }
                else
                {
                    Debug.LogError("File not found: " + fileInfo.Name + Environment.NewLine + fileInfo.FullName);
                    
                    if (multipleFileNamesList.Contains(fileInfo.Name)) 
                        continue;
                    
                    InstantiateTextObj(textGameObj, content, "File not found in render server: " + fileInfo.Name, fileInfo.Name, null, null);
                    multipleFileNamesList.Add(fileInfo.Name);
                }
            }
        }
        
        #endregion

        private void CheckForPrnEquivalent(string yrFileType, FileInfo fileInfo, IEnumerable<FileInfo> renderList, List<string> multipleFileNamesList, GameObject textGameObj, GameObject content)
        {
            var printFileName = fileInfo.Name.Replace(yrFileType, "_PRN.png");
            var printFileList = renderList.Select(x => x)
                .Where(x => string.Equals(x.Name, printFileName, StringComparison.OrdinalIgnoreCase)).ToList();
                    
            if (printFileList.Count == 0)
            {
                Debug.LogError("File not found: " + printFileName);
                InstantiateTextObj(textGameObj, content, "<color=red>PRN file not found: " + printFileName + "</color>", printFileName, null, fileInfo.Name);
            }

            if (printFileList.Count == 1)
            {
                Debug.Log("File found: " + printFileName);
                // InstantiateTextObj(textGameObj, content, "<color=green>PRN equivalent file found: " + printFileName + " (original file: " + fileInfo.Name + ")</color>");
            }

            if (printFileList.Count <= 1) 
                return;
                    
            if (multipleFileNamesList.Contains(printFileName)) 
                return;
                        
            Debug.LogError("Multiple files found with following name in RenderServer: " + printFileName);
            multipleFileNamesList.Add(printFileName);
            InstantiateTextObj(textGameObj, content, "<color=yellow>Multiple files found: " + printFileName + " [" + printFileList.Count + "]" + "</color>", printFileName, printFileList, fileInfo.Name);
        }

        private void InstantiateTextObj(GameObject textGameObject, GameObject parent, string text, string fileName, List<FileInfo> printFileList, string originalFileName)
        {
            textGameObject.GetComponent<TMP_Text>().text = text;
            textGameObject.GetComponent<ArtworkCheckerTextTemplate>().fileName = fileName;
            textGameObject.GetComponent<ArtworkCheckerTextTemplate>().originalFileNamePath = originalFileName;
            textGameObject.GetComponent<ArtworkCheckerTextTemplate>().FilePathList = printFileList;

            Instantiate(textGameObject, parent.transform);
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
    
                if (artworkName == ColumnHeader)
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
                ErrorMessageFunctions.Instance.DisplayError("CSV File Not Found.");
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
                    "<color=yellow>" + imageType + "</color> " + "["+ filesWithCurrentArtworkName.Count + "], ";
            }
        }
    }
}

