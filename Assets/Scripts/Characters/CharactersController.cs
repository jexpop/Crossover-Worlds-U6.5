using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrossoverWorlds
{
    public class CharactersController : Singleton<CharactersController>
    {

        private GameObject[] avaliableCharacters;
        
              
        private void Awake() 
        {

            ListCharacters();
            SetInitialDissolve();
        }

        private void ListCharacters()
        {
            // Obtain all playable characters
            GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Playable Character");

            avaliableCharacters = null;

            // Initialize array
            int NumberOfPlayableCharacter = playableCharacters.Length;
            Array.Resize(ref avaliableCharacters, NumberOfPlayableCharacter + 1); // Resize the array to add the Selected Character at startup

            // Place the Selected Character in the first position
            avaliableCharacters[0] = GameObject.FindGameObjectWithTag("Selected Character");

            // Add the Playable Character
            for (int i = 0; i < playableCharacters.Length; i++)
            {
                avaliableCharacters[i+1] = playableCharacters[i];
            }
        }

        private void SetInitialDissolve()
        {
            foreach (GameObject character in avaliableCharacters)
            {
                // Obtén todos los objetos relacionados (cuerpo y accesorios) como un array
                GameObject[] characterAndAccessories = GetCharacterAndAccessories(character);

                if (character.tag == "Selected Character")
                {
                    // Aplica el efecto de disolución al conjunto de objetos
                    SetDissolveEffect(character, 1.0f);
                    CallDissolveEffect(character, false, 0.1f);                    
                }
                else
                {
                    SetDissolveEffect(character, 1.0f);
                }
            }
        }

        public GameObject[] GetAvaliableCharacters()
        {
            return avaliableCharacters;
        }  

        private void SetDissolveEffect(GameObject objectsToDissolve, float value)
        {
            objectsToDissolve.GetComponentInChildren<Dissolve>().SetEffect(value);

        }

        public void CallDissolveEffect(GameObject objectsToDissolve, bool isDirectionDissolve, float delay)
        {
            objectsToDissolve.GetComponentInChildren<Dissolve>().StartEffect(isDirectionDissolve);
        }

        private GameObject[] GetCharacterAndAccessories(GameObject character)
        {
            List<GameObject> objectsToDissolve = new List<GameObject> { character };

            foreach (Transform child in character.transform)
            {
                objectsToDissolve.Add(child.gameObject);
            }

            return objectsToDissolve.ToArray();
        }

    }
}
