using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class ArtworkCheckerTextTemplate : MonoBehaviour
    {
        public static ArtworkCheckerTextTemplate Instance { get; set; }
        
        public string fileName;
        public string originalFileNamePath;
        public List<FileInfo> FilePathList;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}