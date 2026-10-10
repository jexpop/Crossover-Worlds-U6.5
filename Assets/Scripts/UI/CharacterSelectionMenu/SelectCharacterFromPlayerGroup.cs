using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrossoverWorlds.UI.CharacterSelectionMenu
{  
    public class SelectCharacterFromPlayerGroup : MonoBehaviour
    {

        public Transform content;
        public Button selectCharacterButtonPrefab;

        private CharacterWheel wheel;

        void Start()
        {
            ListCharacters();

            // Call the wheel
            wheel = GameObject.Find("PickerWheel").GetComponent<CharacterWheel>();
            wheel.StartWheel(CharactersController.Instance.GetAvaliableCharacters());

        }

        /// <summary>
        /// Radial character selection
        /// </summary>
        private void ListCharacters()
        {
            bool shouldFirstButtonSelected = true; // The first selected button
            int numButton = 1; // Counter for the buttons

            // Buttons by characters
            foreach (GameObject character in CharactersController.Instance.GetAvaliableCharacters())
            {  
                // Instantiate a new button
                Button selectCharacterButton = Instantiate(selectCharacterButtonPrefab);

                // This button has not any character
                if (character.name != "NoCharacter")
                {
                    selectCharacterButton.GetComponentInChildren<TextMeshProUGUI>().text = character.name;
                }

                // Name and parent of the button
                selectCharacterButton.name = "characterButton"+ numButton;
                selectCharacterButton.transform.SetParent(content.transform, false);

                // Select the first button
                if (shouldFirstButtonSelected)
                {
                    selectCharacterButton.Select();
                    shouldFirstButtonSelected = false;                    
                }

                // Next button
                numButton++;
            }

            // Invoke button listeners
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).GetComponent<Button>().onClick.Invoke();
            }

        }

    }
}
