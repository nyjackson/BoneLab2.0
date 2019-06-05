using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*	This script controls the behavior of the main menu, where a user selects the
 * bones that they want to have in the space. 
 * 
 * The input: Bone gameObjects (must be already in the scene but set INACTIVE) and
 * Coordinate transforms (some number N of them).
 * 
 * The bones will be appear in those positions. 
 * 
 * 
 * 
 */

// Kamile Lukosiute
// klukosiu@wellesley.edu
// January 2018

// Nyala Jackson
// njackson@vassar.edu
// June 2019







namespace UI {
	public class UI_SelectionMenu : MonoBehaviour {

		// Public Objects i.e. Game Objects and Transforms from the Editor of things in the scene
		public Transform checkBox; // prefab for your checkbozxes
		public Canvas pagesCanvas; // canvas for your menu
		public GameObject[] bones; // array of your bone game objects
		public Transform[] coordinates; // array of the coordinate locations for spawning

		public Transform page; // prefab for each individual page to be created for meny


		// Create Checklist
		private int totalPages; // total number of pages to be created
		private int bpp = 9; // bones per page

		private GameObject[] pages;
		private GameObject currentPage; // temporary holder



		float yTop = 180f; // y-location of topmost listing


		private float x = 0.0f;
		private float y = 0.0f;
		private float z = 0.0f;

		float ySeparation = 50.0f; // vertical separation of checkboxes

		// Counters
		private int p = 0;
		private int b = 0;

		private int numBonesListed;





		// Create Bones Selected
		private string bone_name;
		int n = 0;

		// Button Methods
		private int pageViewing = 0;


		// Counter
		public Text selectedBonesText;
		private int bonesSelected = 0;

		// initialization
		void Start () {

			CreateChecklist ();

		}
		
		// called once per frame
		void Update () {
			// updates the number of bones that have been selected
			UpdateCounter ();
		}

		private void CreateChecklist () {
			// Calculate total number of pages to create
			totalPages = (int)Mathf.Ceil ((float)bones.Length / (float)bpp);
			Debug.Log ("Total number of pages to be created: " + totalPages);
			// Create memory for those pages
			pages = new GameObject[totalPages];
			p = 0; // represents page
			while (p < totalPages) {
				// create page
				currentPage = Instantiate (page.gameObject, pagesCanvas.transform);
				currentPage.name = p.ToString ();

				// Display only 1st page
				if (p == 0) {
					currentPage.gameObject.SetActive (true);
				} else {
					currentPage.gameObject.SetActive (false);

				}

				// populate the page with bones
				b = 0; // represents bone
				y = yTop; // put first checkbox back on top

				// if the total number of bones to list is greater than bones per page...
				if ((bones.Length - numBonesListed) > bpp) { 

					while (b < bpp) {
						var tempCheckbox = Instantiate (checkBox);  //instantiate, set current page as parent
						tempCheckbox.position = new Vector3 (x, y, z);
						tempCheckbox.SetParent(currentPage.transform, worldPositionStays:false);
						Debug.Log (tempCheckbox.position);
						tempCheckbox.GetComponentInChildren<Text> ().text = bones [(p * (bpp)) + b].name;
						y = y - ySeparation;
						b++;
						numBonesListed++;
					}
				} 

				else {
					while (b < (bones.Length - numBonesListed)) {
						var tempCheckbox = Instantiate (checkBox);
						tempCheckbox.position = new Vector3 (x, y, z);

						tempCheckbox.SetParent(currentPage.transform, worldPositionStays:false);

						//, currentPage.transform); //instantiate, set current page as parent
						tempCheckbox.GetComponentInChildren<Text> ().text = bones [(p * (bpp)) + b].name;
						y = y - ySeparation;
						b++;
						numBonesListed++;
					}	
				}
				// Add that page to memory so it can be referenced later for bone creation
				pages [p] = currentPage;
				p += 1;
			}
		}



		// applied to "Create Bones" button
		public void CreateBonesSelected () {
			foreach (GameObject page in pages) {

				foreach (Toggle tog in page.GetComponentsInChildren<Toggle>()) {
					if (tog.isOn) {
						bone_name = tog.GetComponentInChildren<Text> ().text;

						for (int i = 0; i < bones.Length; i++) {
							if (bones [i].name == bone_name) {
								bones [i].SetActive (true);
								bones [i].transform.position = coordinates [n].position;
								n++;
								DontDestroyOnLoad (bones [i].gameObject);
							}
						}


					}
				}
			}

			SceneManager.LoadScene ("Main", LoadSceneMode.Single);
		}

		// applied to back page arrow
		public void BackClick () {
			if (pageViewing > 0) {
				pageViewing--;
				for (int i = 0; i < pagesCanvas.transform.childCount; i++) {
					var child = pagesCanvas.transform.GetChild (i).gameObject;
					if (child != null) {
						child.SetActive (false);
					}
				}
				pagesCanvas.gameObject.transform.Find (pageViewing.ToString ()).gameObject.SetActive (true);

			}
		
		}

		// applied to forward page arrow
		public void ForwardClick () {
			if (pageViewing < pagesCanvas.transform.childCount - 1) {
				pageViewing++;
				for (int i = 0; i < pagesCanvas.transform.childCount; i++) {
					var child = pagesCanvas.transform.GetChild (i).gameObject;
					if (child != null) {
						child.SetActive (false);
					}
				}
				pagesCanvas.gameObject.transform.Find (pageViewing.ToString ()).gameObject.SetActive (true);

			}
		}

		// updates the number of bones that have been selected
		private void UpdateCounter () {
			bonesSelected = 0;
			foreach (GameObject page in pages) {

				foreach (Toggle tog in page.GetComponentsInChildren<Toggle>()) {
					if (tog.isOn) {
						bonesSelected++;
					}
				}
			}

			selectedBonesText.text = bonesSelected.ToString () + " Selected (Max 20)";

		}



}
}