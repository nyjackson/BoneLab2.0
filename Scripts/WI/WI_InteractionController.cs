using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the script you need to apply to the controllers. The snapCollider is a sphere at
// position (0,-0.045, 0.001) with scale (0.1, 0.1, 0.1) that is located in the Controller's
// hierarchy.

// LOL also add rigidbodies to the controllers 



namespace WI {
public class WI_InteractionController : MonoBehaviour {
		public Transform snapColliderOrigin;
		public GameObject ControllerModel;

		[HideInInspector]
		public Vector3 velocity;
		[HideInInspector]
		public Vector3 angularVelocity; 



		// Teleportation Items
		public Transform cameraRigTransform;
		public Transform headTransform; // The camera rig's head
		public LayerMask teleportMask; // Mask to filter out areas where teleports are allowed

		private Vector3 hitPoint; // Point where the raycast hits
		private bool shouldTeleport; // True if there's a valid teleport target


		public GameObject teleportLaser; // A reference to the spawned laser
		public GameObject teleportHitIndicator;


		private WI_InteractionObject objectBeingInteractedWith;
		private SteamVR_TrackedObject trackedObj;

		// Shortcut to the SteamVR_Controller class
		public SteamVR_Controller.Device Controller
		{
			get { return SteamVR_Controller.Input ((int)trackedObj.index); }
		}

		public WI_InteractionObject InteractionObject {
			get { return objectBeingInteractedWith; }
		}

		// Gets the component we need upon wake
		void Awake()
		{
			trackedObj = GetComponent<SteamVR_TrackedObject>();
			teleportHitIndicator.SetActive (false);
			teleportLaser.SetActive (false);
		}



		// Runs once per frame and checks for interactions
		// Certain interactions indicate object interactions
		// Certain others don't, like teleportation.
		void Update()
		{
			// Checks for Trigger Interactions
			if (Controller.GetHairTriggerDown ()) {
				CheckForInteractionObject ();
			}

			if (Controller.GetHairTrigger ()) {
				if (objectBeingInteractedWith) {
					objectBeingInteractedWith.OnTriggerIsBeingPressed (this);
				}
			}

			if (Controller.GetHairTriggerUp ()) {
				if (objectBeingInteractedWith) {
					objectBeingInteractedWith.OnTriggerWasReleased (this);
					objectBeingInteractedWith = null;
				}
			}


		
			// Checks for Button Interactions
			if ( PressDown() ) {
				if (objectBeingInteractedWith) {
					objectBeingInteractedWith.OnPressDown (this);
				}
			}

			if ( PressUp() ) {
				if (objectBeingInteractedWith) {
					objectBeingInteractedWith.OnPressUp (this);
				}
			}
				


			// Teleportation
			if (PressLeft ()) {
				Debug.Log ("Button Pressed.");
				RaycastHit hit;

				if (Physics.Raycast (trackedObj.transform.position, transform.forward, out hit, 100, teleportMask)) {
					hitPoint = hit.point;

					// Laser
					teleportLaser.SetActive (true);
					teleportLaser.transform.localPosition = new Vector3 (0.0f, 0.0f, hit.distance * 0.5f);
					//laser.transform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); 
					//laser.transform.LookAt(hitPoint); // Rotate laser facing the hit point
					teleportLaser.transform.localScale = new Vector3(teleportLaser.transform.localScale.x, teleportLaser.transform.localScale.y,
						hit.distance);

					teleportHitIndicator.SetActive (true);

					teleportHitIndicator.transform.position = hitPoint;

					shouldTeleport = true;
					Debug.Log ("Should teleport now...");

				} else {
					teleportLaser.SetActive (false);
					teleportHitIndicator.SetActive (false);
				}
			}

			if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport) {
				Debug.Log("Teleport in progress.");
				Teleport ();
			}

			if (PressRight()) {
				if (this.GetComponent<FixedJoint> ()) {
					SetHandMenuActive (true);
				}
			}



		}

		void FixedUpdate()
		{
			UpdateVelocity();
		}



		// INTERACTION DEFINITIONS 
		public bool PressLeft () {
			if (Controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)
			    && Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [1] > -0.5f
			    && Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [1] < 0.5f
			    && Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [0] < 0.0f) {
				return true;

			} else {
				return false;
			}
		}

		public bool PressRight () {
			if (Controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)
				&& Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [1] > -0.5f
				&& Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [1] < 0.5f
				&& Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [0] > 0.0f) {
				return true;

			} else {
				return false;
			}
		}

		public bool PressUp () {
			if (Controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)
			    && Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [0] > -0.5f
			    && Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [0] < 0.5f
			    && Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [1] > 0.0f) {
				return true;

			} else {
				return false;
			}
		}


		public bool PressDown () {
			if (Controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)
				&& Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [0] > -0.5f
				&& Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [0] < 0.5f
				&& Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0) [1] < 0.0f) {

				return true;

			} else {
				return false;
			}
		}

		// HELPERS
		private void CheckForInteractionObject()
		{

			Collider[] overlappedColliders = Physics.OverlapSphere (snapColliderOrigin.position, snapColliderOrigin.lossyScale.x / 2f);
			foreach (Collider overlappedCollider in overlappedColliders) {
				if (overlappedCollider.CompareTag ("InteractionObject") && overlappedCollider.GetComponentInParent<WI_InteractionObject> ().IsFree ()) {
					objectBeingInteractedWith = overlappedCollider.GetComponentInParent<WI_InteractionObject> ();
					objectBeingInteractedWith.OnTriggerWasPressed (this);
					Debug.Log ("object: " + objectBeingInteractedWith.name);
					return;
				}
			}
		}

		private void UpdateVelocity()
		{
			velocity = Controller.velocity;
			angularVelocity = Controller.angularVelocity;
		}


		public void HideControllerModel()
		{
			ControllerModel.SetActive (false);
		}

		public void ShowControllerModel()
		{
			ControllerModel.SetActive (true);
		}
			
		public void SwitchInteractionObjectTo(WI_InteractionObject interactionObject)
		{
			objectBeingInteractedWith = interactionObject;
			objectBeingInteractedWith.OnTriggerWasPressed (this);
		}

		private void Teleport()
		{
			shouldTeleport = false; // Teleport in progress, no need to do it again until the next touchpad release
			teleportLaser.SetActive(false); // Hide reticle
			teleportHitIndicator.SetActive (false);
			Vector3 difference = cameraRigTransform.position - headTransform.position; // Calculate the difference between the center of the virtual room & the player's head
			difference.y = 0; // Don't change the final position's y position, it should always be equal to that of the hit point

			cameraRigTransform.position = hitPoint + difference; // Change the camera rig position to where the the teleport reticle was. Also add the difference so the new virtual room position is relative to the player position, allowing the player's new position to be exactly where they pointed. (see illustration)
		}

		public void SetHandMenuActive(bool setActive) {
			this.transform.GetChild (3).gameObject.SetActive (setActive);
			// Set main active
			this.transform.GetChild (3).gameObject.transform.GetChild (0).gameObject.SetActive (true);
			// Set appearnace and measure not active
			this.transform.GetChild (3).gameObject.transform.GetChild (1).gameObject.SetActive (false);
			this.transform.GetChild (3).gameObject.transform.GetChild (2).gameObject.SetActive (false);
		}

	}

}