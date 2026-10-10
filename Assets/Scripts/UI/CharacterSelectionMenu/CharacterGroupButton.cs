using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using CrossoverWorlds.Characters.FacialExpression;
using CrossoverWorlds.Characters.Movement.Player;
using CrossoverWorlds.UI.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;


namespace CrossoverWorlds.UI.CharacterSelectionMenu
{
    public class CharacterGroupButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {

        public Button thisButton;
        
        private CharacterWheel wheel;

        private bool isInitial = true;

        private Player player;


        void Start()
        {
            wheel = GameObject.Find("PickerWheel").GetComponent<CharacterWheel>();
            player= GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        public void SetNavigationOrder()
        {            
            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Explicit;

            int NumberOfButtonsFromContent=transform.parent.transform.childCount;
            int buttonNumber = int.Parse(this.transform.name[^1..], NumberStyles.Integer, CultureInfo.InvariantCulture);

            int idxSelectOnLeft = buttonNumber == 1 ? NumberOfButtonsFromContent - 1 : buttonNumber - 2;
            customNav.selectOnLeft = transform.parent.transform.GetChild(idxSelectOnLeft).GetComponent<Button>();

            int idxSelectOnRight = buttonNumber == NumberOfButtonsFromContent ? 0 : buttonNumber;
            customNav.selectOnRight = transform.parent.transform.GetChild(idxSelectOnRight).GetComponent<Button>();
            
            thisButton.navigation = customNav;
        }

        public void ChangeCharacter()
        {
            if(!isInitial)
            {
                GameObject currentPiece = null;

                // Characters no selected, opacity lower
                foreach (GameObject piece in GameObject.FindGameObjectsWithTag("PieceCharacterWheel"))
                {
                    if (piece.name == transform.name)
                    {
                        Color color = piece.GetComponentInChildren<Image>().color;                        
                        piece.GetComponentInChildren<Image>().color = ColorUtilities.SetAlpha(color, 1f);
                        currentPiece = piece;
                    }
                    else
                    {
                        Color color = piece.GetComponentInChildren<Image>().color;
                        piece.GetComponentInChildren<Image>().color = ColorUtilities.SetAlpha(color, 0.5f);
                    }
                }      

                // Change the active character
                string nameCharacterObject = currentPiece.GetComponentInChildren<Image>().sprite.name;
                foreach (GameObject character in CharactersController.Instance.GetAvaliableCharacters())
                {
                    if (character.name == nameCharacterObject)
                    {

                        // Ahora actiar el Animator y el GameObject
                        //character.GetComponent<Animator>().enabled = true;

                        // Ahora cambiar el Avatar y activar el GameObject
                        character.transform.parent.GetComponent<Animator>().avatar = character.GetComponent<Animator>().avatar;                        
                        character.transform.GetChild(0).gameObject.SetActive(true);

                        // En caso de ser clase Martial Weapon
                        GameObject DrawedWeapon = character.transform.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(t => t.CompareTag("DrawedWeapon")).gameObject;
                        GameObject SheathedWeapon = character.transform.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(t => t.CompareTag("SheathedWeapon")).gameObject;
                        //player.SetNewWeapons(DrawedWeapon, SheathedWeapon);

                        CharactersController.Instance.CallDissolveEffect(character, false, 0.1f);
                    }
                    else
                    {
                        CharactersController.Instance.CallDissolveEffect(character, true, 0.1f);
                        StartCoroutine(DisableCharacterAfterDissolve(character, 1f));
                    }
                }

            }

            isInitial = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (wheel != null)
            {
                int currentIdxButton = SpinButton(100f);
                wheel.Spin(currentIdxButton);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (wheel != null)
            {
                SpinButton(0f);
            }
        }

        private int SpinButton(float chance)
        {
            int buttonNumber = int.Parse(this.transform.name[^1..], NumberStyles.Integer, CultureInfo.InvariantCulture);
            wheel.characterPieces[buttonNumber - 1].chance = chance;
            return buttonNumber - 1;
        }

        private IEnumerator DisableCharacterAfterDissolve(GameObject character, float delay)
        {
            yield return new WaitForSeconds(delay);

            // Ahora desactivar el Animator y el GameObject
            //character.GetComponent<Animator>().enabled = false;

            // Ahora desactivar el GameObject
            character.transform.GetChild(0).gameObject.SetActive(false);
        }

        /*private GameObject[] GetCharacterAndAccessories(GameObject character)
        {
            // Asume que los accesorios están agrupados como hijos del personaje
            List<GameObject> objectsToDissolve = new List<GameObject> { character };

            foreach (Transform child in character.transform)
            {
                objectsToDissolve.Add(child.gameObject);
            }

            return objectsToDissolve.ToArray();
        }*/

    }
}
