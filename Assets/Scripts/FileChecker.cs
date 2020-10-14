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
        private string _repoPath = "";
        private string _csvFilePath = "";

        public void InputValueRepoPath(string path)
        {
            _repoPath = path;
        }

        public void InputValueCsvPath(string path)
        {
            _csvFilePath = path;
        }

        public void CheckFiles()
        {
            var csvFileList = LoadCsvFileViaPath(_csvFilePath);

            if (csvFileList.Count == 0)
            {
                Debug.LogError("CsvFile returned empty");
                return;
            }

            var artworkIndex = GetArtworkIndex(csvFileList);
            if (artworkIndex == -1)
            {
                Debug.LogError("No column found with header: ARTWORK_NAME");
                return;
            }
            
            var artworkNameList = GetArtworkNameList(csvFileList, artworkIndex);
            var files = GetRepositoryFileNames(_repoPath, "*.png");

            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log(files[i]);
            }
            
            var content = FindSingleObjectByName("Content");
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            var textGameObj = Resources.Load<GameObject>("Prefabs/ArtworkCheckerTextTemplate");
            for (int i = 0; i < artworkNameList.Count; i++)
            {
                textGameObj.GetComponent<TMP_Text>().text = artworkNameList[i] + ": ";
                
                CheckArtwork(files, artworkNameList, i, textGameObj, "_MSK");
                CheckArtwork(files, artworkNameList, i, textGameObj, "_PRN");
                CheckArtwork(files, artworkNameList, i, textGameObj, "_SCR");
                CheckArtwork(files, artworkNameList, i, textGameObj, "_THM");

                Instantiate(textGameObj, content.transform);
            }
        }

        private string[] GetRepositoryFileNames(string repoPath, string fileType)
        {
            var repoFiles = Directory.GetFiles(repoPath, fileType, SearchOption.AllDirectories);
            var fileNames = new List<string>();
            for (int i = 0; i < repoFiles.Length; i++)
            {
                var fileName = Path.GetFileName(repoFiles[i]);
                if (fileName.EndsWith(".png"))
                    fileName = fileName.Replace(".png", "");
                
                fileNames.Add(fileName);
            }
            return fileNames.ToArray();
        }

        private List<string> GetArtworkNameList(List<string> csvFileList, int artworkIndex)
        {
            var artworkNameList = new List<string>();
            for (int i = 0; i < csvFileList.Count; i++)
            {
                var split = csvFileList[i].Split(',');
                var artworkName = split[artworkIndex];
                if (artworkName == "ARTWORK_NAME")
                    continue;

                if (!artworkNameList.Contains(artworkName))
                {
                    artworkNameList.Add(artworkName);
                }
                else
                {
                    Debug.LogError("ArtworkNameList already contains artwork name, continuing...");
                }
            }

            return artworkNameList;
        }

        private int GetArtworkIndex(List<string> csvFileList)
        {
            var headers = csvFileList[0];
            var headerSplit = headers.Split(',');
            var artworkIndex = -1;

            for (int i = 0; i < headerSplit.Length; i++)
            {
                if (headerSplit[i] == "ARTWORK_NAME")
                {
                    artworkIndex = i;
                }
            }

            return artworkIndex;
        }
        
        private List<string> LoadCsvFileViaPath(string filePath)
        {
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

        private void CheckArtwork(string[] files, List<string> artworkNameList, int i, GameObject textGameObj, string imageType)
        {
            var filesWithCurrentArtworkName = files.Select(x => x).Where(x => x == artworkNameList[i] + imageType).ToArray();

            switch (filesWithCurrentArtworkName.Length)
            {
                case 0:
                    Debug.LogError("File could not be found with following artwork name: " + artworkNameList[i] + imageType);
                    textGameObj.GetComponent<TMP_Text>().text +=
                        "<color=red>" + imageType + "</color>, ";
                    break;
                case 1:
                    Debug.Log("Success, file found!" + Environment.NewLine + " ARTWORK_NAME: " + artworkNameList[i] + imageType + Environment.NewLine + " FileName: " + filesWithCurrentArtworkName[0]);
                    textGameObj.GetComponent<TMP_Text>().text += "<color=green>"+ imageType +"</color>, ";
                    break;
            }
            if (filesWithCurrentArtworkName.Length > 1)
            {
                Debug.LogError("Multiple files found with the following name: " + artworkNameList[i] + imageType);
                textGameObj.GetComponent<TMP_Text>().text +=
                    "<color=orange>" + imageType + "</color>, ";
            }
        }
    }
}

