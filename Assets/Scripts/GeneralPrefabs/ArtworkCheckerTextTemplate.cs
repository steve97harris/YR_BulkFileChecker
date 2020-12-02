using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class ArtworkCheckerTextTemplate : MonoBehaviour
    {
        public static ArtworkCheckerTextTemplate Instance { get; set; }
        
        public string fileName;
        public string fullFilePath;
        public List<FileInfo> FilePathList;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void OnClickArtworkText()
        {
            var fileInfoPanel = Resources.Load<GameObject>(FileChecker.FILE_INFO_PANEL);
            fileInfoPanel.transform.GetChild(0).GetComponent<TMP_Text>().text =
                $"This file was not found on the render server: {fullFilePath}";

            foreach (var obj in GameObject.FindGameObjectsWithTag("FileInfoPanel"))
            {
                Destroy(obj);
            }
            
            Instantiate(fileInfoPanel, MainCanvas.Instance.transform);
        }
    }
}