using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class DropdownModule : MonoBehaviour
    {
        public enum Project    
        {
            Default,
            Levis
        }

        private void Start()
        {
            var dropdown = transform.GetComponent<TMP_Dropdown>();
            dropdown.options.Clear();
            
            var projectList = new string[]
            {
                "Default", "Levis"
            };
            dropdown.AddOptions(projectList.ToList());
            
            dropdown.onValueChanged.AddListener(x => DropdownItemSelected(dropdown));
        }

        private void DropdownItemSelected(TMP_Dropdown dropdown)
        {
            var index = dropdown.value;
            var optionSelected = dropdown.options[index].text;
            transform.GetChild(0).GetComponent<TMP_Text>().text = optionSelected;
            SetProjectSelection(optionSelected);
        }

        private void SetProjectSelection(string optionSelected)
        {
            switch (optionSelected)
            {
                case "Default":
                    FileChecker.ProjectSelected = Project.Default;
                    break;
                case "Levis":
                    FileChecker.ProjectSelected = Project.Levis;
                    break;
            }
        }
    }
}