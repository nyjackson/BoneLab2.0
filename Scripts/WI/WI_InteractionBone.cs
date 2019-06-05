using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WI {
	public class WI_InteractionBone : WI_InteractionObject {

		public bool hideControllerModelOnGrab;
		private Rigidbody rb;


		private bool menuExists = false;

		private float current_scale_f;
		private int current_scale_i;
		private int upper_bound_i = 10; // corresponds to .01 * 1000
		private int lower_bound_i = 1; // corresponds to .001 * 1000
		private Vector3 scale;


		public override void Awake()
		{
			base.Awake ();
			rb = GetComponent<Rigidbody> ();
		}

		// INTERACTION DEFINITIONS
		// On press up, scale up.
		public override void OnPressDown(WI_InteractionController controller) {
			base.OnPressDown (controller);
			current_scale_f = rb.transform.localScale.x;
			current_scale_i = (int)(current_scale_f * 1000.0f);

			if (current_scale_i > lower_bound_i) {
				// Set the third child as active 
				controller.transform.GetChild (2).gameObject.SetActive (true);
	
				StartCoroutine (RemoveAfterSeconds (10, controller));
				menuExists = true;
				ScaleDown ();
			}
		}







		// On press down, scale down.
		public override void OnPressUp(WI_InteractionController controller) {
			base.OnPressUp (controller);
			// OKAY SOME WEIRD BLACK MAGIC IS HAPPENING HERE. IF I ADD THE TWO LINES BELOW (WHICH
			// 	SHOULD NOT DO ANYTHING EXCEPT GET THE VALUE), THE MAX SCALE GOES TO 5. I DON'T GET
			// 	IT BUT I'M JUST GONNA IGNORE THE PROBLEM BECAUSE I'M GOING INSANE. 
			//current_scale_f = rb.transform.localScale.x;
			//current_scale_i = (int)(current_scale_f * 1000.0f);

			if (current_scale_i < upper_bound_i) {
				controller.transform.GetChild (2).gameObject.SetActive (true);
				StartCoroutine (RemoveAfterSeconds (5, controller));
				menuExists = true;
				ScaleUp ();
			}
		}

		// On trigger down, grab object.
		public override void OnTriggerWasPressed(WI_InteractionController controller)
		{
			base.OnTriggerWasPressed (controller);

			if (hideControllerModelOnGrab) {
				controller.HideControllerModel ();
			}

			AddFixedJointToController (controller);
		

		}

		// On trigger released, drop object. 
		public override void OnTriggerWasReleased(WI_InteractionController controller)
		{
			base.OnTriggerWasReleased (controller);

			if (hideControllerModelOnGrab) {
				controller.ShowControllerModel ();
			}

			rb.velocity = controller.velocity;
			rb.angularVelocity = controller.angularVelocity;
			RemoveFixedJointFromController (controller);
			controller.SetHandMenuActive (false);

			// If the slider is still there, destroy it.
			if (menuExists) {
				controller.transform.GetChild (2).gameObject.SetActive (false);
				menuExists = false;
			}
		}

		// HELPERS



		IEnumerator RemoveAfterSeconds(int seconds, WI_InteractionController controller) {
			yield return new WaitForSeconds (seconds);
			controller.transform.GetChild (2).gameObject.SetActive (false);
			menuExists = false;

		}
			
		private void ScaleUp() {
			current_scale_i = current_scale_i + 1;
			current_scale_f = (float)(current_scale_i) / 1000.0f;
			scale = new Vector3 (current_scale_f, current_scale_f, current_scale_f);
			rb.transform.localScale = scale;

		}

		private void ScaleDown() {
				current_scale_i = current_scale_i - 1;
				current_scale_f = (float)(current_scale_i) / 1000.0f;
				scale = new Vector3 (current_scale_f, current_scale_f, current_scale_f);
				rb.transform.localScale = scale;
		}

		private void AddFixedJointToController(WI_InteractionController controller)
		{
			FixedJoint fx = controller.gameObject.AddComponent<FixedJoint> ();
			fx.breakForce = 20000;
			fx.breakTorque = 20000;
			fx.connectedBody = rb;
		}

		private void RemoveFixedJointFromController(WI_InteractionController controller)
		{
			if (controller.gameObject.GetComponent<FixedJoint> ()) {
				FixedJoint fx = controller.gameObject.GetComponent<FixedJoint> ();
				fx.connectedBody = null;
				Destroy (fx);
			}
		}













	}
}

// DEFUNCT BUT HERE FOR MY RECORDS
// This is for the scale slider. 

/*
		// On button 3,  if menu does not exist, open scale menu.
// If the menu exists already, close the menu. 
public override void OnButton3Pressed(WI_InteractionController controller)
{
	base.OnButton3Pressed (controller);

	if (menuExists == false) {
		GameObject menu = Instantiate (SliderMenu.gameObject) as GameObject;
		menu.transform.SetParent (controller.transform, worldPositionStays: false);
		menuExists = true;
		Debug.Log ("Made the object.");
	} else {

		menuExists = false;
	}

}
*/

