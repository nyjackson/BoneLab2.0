using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Aubrey Simonson
// asimonso@wellesley.edu
// August 2017

public class next : MonoBehaviour {

	//a reference to num, which corresponds to the index numbers of images in the array
	public int num;

	//both of these lines tell this script how to find the gallery script
	public GameObject gallery_wall;
	private gallery button;

	public LayerMask nextMask; // Mask to filter out what is and is not the next button

	private SteamVR_TrackedObject trackedObj;

	public GameObject laserPrefab; // The laser prefab
	private GameObject laser; // A reference to the spawned laser
	private Transform laserTransform; // The transform component of the laser for ease of use

	private Vector3 hitPoint; // Point where the raycast hits

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	private bool shouldClick; // True if the laser hits the next button



	void Awake()
	{
		//this also just reads the gallery script, and finds the things it needs there
		button = gallery_wall.GetComponent<gallery> ();

		//reference to the controller?
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	void Start()
	{
		laser = Instantiate(laserPrefab);
		laserTransform = laser.transform;
	}


	void Update()
	{

		// Is the touchpad held down?

		Vector2 touchpad = (Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0));

		if (touchpad.y < 0.0f && touchpad.x > -0.5f && touchpad.x < 0.5f && Controller.GetPress (SteamVR_Controller.ButtonMask.Touchpad)) {

			RaycastHit hit;

			// Send out a raycast from the controller
			if (Physics.Raycast (trackedObj.transform.position, transform.forward, out hit, 100, nextMask)) {
				hitPoint = hit.point;

				ShowLaser (hit);

			
				shouldClick = true;
			}
		}
		if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad) && shouldClick) {
			buttonPress ();

		} else if (touchpad.y < -0.5f && touchpad.x < -0.5f) {// Touchpad not held down, hide laser
			laser.SetActive (false);
		}

	}
		

	private void ShowLaser(RaycastHit hit)
	{
		laser.SetActive(true); //Show the laser
		laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
		laserTransform.LookAt(hitPoint); // Rotate laser facing the hit point
		laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
			hit.distance); // Scale laser so it fits exactly between the controller & the hit point
	}

	//this function makes clicking the box activate the gallery script, and flip to the next image

	void buttonPress()
	{
		//this increments the variable num.  Num is the number in the array of images, so this goes to the next image
		 
		button.num++;

		shouldClick = false;

		//turns laser off
		laser.SetActive(false);


		//this isn't strictly necessary, but it makes me happy/ is good for troubleshooting
		Debug.Log("num has increased!");
	}
}
