using UnityEngine;
using UnityEngine.UI;
using System;

namespace UI {

	public class UI_DisplayScale : MonoBehaviour {

		public Transform canvas;
		private Transform objectHeld;
		private float  displayVal;




		void Update() 
		{
			if (canvas.parent.gameObject.transform.GetComponent<FixedJoint> () != null) {
				objectHeld = canvas.parent.gameObject.transform.GetComponent<FixedJoint> ().connectedBody.transform;
				displayVal = objectHeld.localScale.x * 1000.0f;
				canvas.Find ("ScaleDisplay").GetComponent<Text> ().text = displayVal.ToString ("0x");
				// 
			}
		}



	}
}



// DEFUNCT
// how to adjust scale using a slider
/* 
 
		public void AdjustScale(float scale) 
		{
			objectHeld = canvas.parent.gameObject.transform.GetComponent<FixedJoint> ().connectedBody.transform;
			displayVal = scale * 1000.0f;
			objectHeld.localScale = new Vector3 (scale, scale, scale);
			canvas.Find ("Text").GetComponent<Text> ().text = displayVal.ToString("0.00");
		}
*/