using UnityEngine;

namespace CrossoverWorlds.UI.CharacterSelectionMenu
{
   [System.Serializable]
   public class CharacterPiece {
      public Sprite icon ;
      public string label ;
        public string buttonReference;

      [Tooltip ("Probability in %")] 
      [Range (0f, 100f)] 
      public float chance = 100f ;

      [HideInInspector] public int Index ;
      [HideInInspector] public double _weight = 0f ;
   }
}
