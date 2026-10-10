//
//SpingManager.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpingManager.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/24
//           Y.Ebata
//

using UnityEngine;

namespace CrossoverWorlds.Characters.Clothes___Hair_Physics
{
	public class SpringManager : MonoBehaviour
	{

		public string[] DynamicParts;

		//Kobayashi
		// DynamicRatio is paramater for activated level of dynamic animation 
		public float dynamicRatio = 1.0f;

		//Ebata
		public float			stiffnessForce;
		public AnimationCurve	stiffnessCurve;
		public float			dragForce;
		public AnimationCurve	dragCurve;
		public SpringBone[] springBones;

		void Start ()
		{
			UpdateParameters ();
		}
	
		void Update ()
		{
#if UNITY_EDITOR
		//Kobayashi
		if(dynamicRatio >= 1.0f)
			dynamicRatio = 1.0f;
		else if(dynamicRatio <= 0.0f)
			dynamicRatio = 0.0f;
		//Ebata
		UpdateParameters();
#endif
		}
	
		private void LateUpdate ()
		{
            //Kobayashi
            if (dynamicRatio != 0.0f) {
				for (int i = 0; i < springBones.Length; i++) {
					if (dynamicRatio > springBones [i].threshold) {
						springBones [i].UpdateSpring ();
					}
				}
			}
		}

		[ContextMenu("Obtain bones")]
		public void GetAllBones()
		{
            Transform[] transforms = GetComponentsInChildren<Transform>();
			foreach (Transform o in transforms)
			{

                // Destroy previous components to avoid duplicates
                SpringBone[] childs = o.gameObject.GetComponents<SpringBone>();
                foreach (SpringBone child in childs)
                {
                    DestroyImmediate(child);
                }

                if (startWithColumn(o.name, DynamicParts))
                {
                    if (o.childCount > 0)
					{
						// Create new components
                        o.gameObject.AddComponent<SpringBone>();
						o.gameObject.GetComponent<SpringBone>().child = o.GetChild(0);
						o.gameObject.GetComponent<SpringBone>().boneAxis = new Vector3(0, 1, 0);
					}
				}
			}
			springBones = GetComponentsInChildren<SpringBone>();
		}


		private void UpdateParameters ()
		{
			UpdateParameter ("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter ("dragForce", dragForce, dragCurve);
		}
	
		private void UpdateParameter (string fieldName, float baseValue, AnimationCurve curve)
		{
			var start = curve.keys[0].time;
			var end = curve.keys [curve.length - 1].time;
			var step	= (end - start) / (springBones.Length - 1);
		
			var prop = springBones [0].GetType ().GetField (fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
			
			for (int i = 0; i < springBones.Length; i++) {
				//Kobayashi
				if (!springBones [i].isUseEachBoneForceSettings) {
					var scale = curve.Evaluate (start + (end - start) * i / (springBones.Length - 1));
					prop.SetValue (springBones [i], baseValue * scale);
				}
			}
		}

        private bool startWithColumn(string toCheck, string[] words)
        {
            for (int i = 0; i < words.Length; i++)
                if (toCheck.StartsWith(words[i] + ""))
                {
                    return true;
                }
            return false;
        }

    }
}