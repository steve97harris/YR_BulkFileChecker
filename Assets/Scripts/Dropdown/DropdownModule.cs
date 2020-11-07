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
        public string dropdownName;

        private const string PROJECT_DROPDOWN_NAME = "ProjectDropdown";
        private const string FUNCTION_DROPDOWN_NAME = "FunctionDropdown";

        private const string REGULAR_CHECKER_PATH = "Prefabs/RegularCheckerOptions";
        private const string COMPARISON_CHECKER_PATH = "Prefabs/ComparisonCheckerOptions";

        public enum Project    
        {
            Default,
            Levis
        }

        private void Start()
        {
            if (RegularCheckerOptions.Instance == null)
                InstantiateCheckerType(REGULAR_CHECKER_PATH);
            
            dropdownName = transform.name;
            
            var dropdown = transform.GetComponent<TMP_Dropdown>();
            dropdown.options.Clear();

            if (dropdownName == PROJECT_DROPDOWN_NAME)
            {
                var projectList = new string[]
                {
                    "Default", "Levis"
                };
                SetDropdownOptions(dropdown, projectList);

                dropdown.onValueChanged.AddListener(x => SetProjectSelection(dropdown.options[dropdown.value].text));
            }

            if (dropdownName == FUNCTION_DROPDOWN_NAME)
            {
                var checkerFunctions = new string[]
                {
                    "Regular", "ContentComparison"
                };
                SetDropdownOptions(dropdown, checkerFunctions);

                dropdown.onValueChanged.AddListener(x => SetFunctionSelection(dropdown.options[dropdown.value].text));
            }
        }

        private void SetDropdownOptions(TMP_Dropdown dropdown, string[] options)
        {
            dropdown.AddOptions(options.ToList());
            
            dropdown.onValueChanged.AddListener(x => DropdownItemSelected(dropdown));
        }

        private void DropdownItemSelected(TMP_Dropdown dropdown)
        {
            var index = dropdown.value;
            var optionSelected = dropdown.options[index].text;
            transform.GetChild(0).GetComponent<TMP_Text>().text = optionSelected;
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

        private void SetFunctionSelection(string optionSelected)
        {
            switch (optionSelected)
            {
                case "Regular":
                    if (ComparisonCheckerOptions.Instance != null)
                        Destroy(ComparisonCheckerOptions.Instance.gameObject);
                    InstantiateCheckerType(REGULAR_CHECKER_PATH);
                    PlayerPreferenceModule.Instance.GetPlayerPrefs(PlayerPreferenceModule.Function.Regular);
                    break;
                case "ContentComparison":
                    if (RegularCheckerOptions.Instance != null)
                        Destroy(RegularCheckerOptions.Instance.gameObject);
                    InstantiateCheckerType(COMPARISON_CHECKER_PATH);
                    PlayerPreferenceModule.Instance.GetPlayerPrefs(PlayerPreferenceModule.Function.Comparison);
                    break;
            }
        }

        private void InstantiateCheckerType(string objPath)
        {
            var obj = Resources.Load<GameObject>(objPath);
            obj = Instantiate(obj, MainCanvas.Instance.transform);
            obj.transform.SetSiblingIndex(2);
        }
    }
}