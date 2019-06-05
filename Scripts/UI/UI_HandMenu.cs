using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/* This script controls the behavior of the menu attached to the controller.
 * 
 * Kamile Lukosiute
 * klukosiu@wellesley.edu
 * August 2017
 */

namespace UI {
	public class UI_HandMenu : MonoBehaviour {

		public Toggle float_tog; 
		public Toggle transparent_tog;

		public Text[] text_boxes = new Text[4];

		public Material opaqueMaterial;
		public Material transparentMaterial; 

		private Transform attached_bone;
		private float current_drag;
		private Color current_color;
	
		private Renderer[] all_mesh_renderers;

		private TextAsset textFile;
		private int n = 0;


		static Color[] color_choices = new Color[6] {
			new Color(0.57f, 0.00f, 0.72f, 1.0f), //purple
			new Color(0.05f, 0.77f, 0.04f, 1.0f), //green
			new Color(0.81f, 0.45f, 0.00f, 1.0f), //orange
			Color.white,
			Color.black,
			new Color(0.93f, 0.90f, 0.83f, 1.0f) // natural bone color
		};


		// When the menu appears, this function runs
		void Update() {
			if (GetComponentInParent<FixedJoint> () != null) {
				attached_bone = GetComponentInParent<FixedJoint> ().connectedBody.transform;
				Debug.Log ("Currently Attached: " + attached_bone.name);

				GetObjectInfo ();

				current_drag = attached_bone.gameObject.GetComponent<Rigidbody> ().drag;
				if (attached_bone) {
					if (current_drag == 5000) {
						float_tog.isOn = true;
					} else {
						float_tog.isOn = false;
					}

					if (attached_bone.gameObject.GetComponentsInChildren<Renderer> () [0].material.color.a == 1.0f) {
						transparent_tog.isOn = false;
					} else {
						transparent_tog.isOn = true;
					}
				}
			}
		}

		// This function makes the object "float" by making the drag and
		// angular drag really high
		public void ToggleFloat(bool setFloating) {

			if (setFloating == true) {
				attached_bone.gameObject.GetComponent<Rigidbody> ().angularDrag = 5000;
				attached_bone.gameObject.GetComponent<Rigidbody> ().drag = 5000;
			} else {
				attached_bone = GetComponentInParent<FixedJoint> ().connectedBody.transform;

				attached_bone.gameObject.GetComponent<Rigidbody> ().angularDrag = 0;
				attached_bone.gameObject.GetComponent<Rigidbody> ().drag = 0;
			}
		}


		// This function changes the shader on the obect to make it transparent
		public void MakeTransparent(bool setTransparent) {

			all_mesh_renderers = attached_bone.gameObject.GetComponentsInChildren<Renderer> ();
			current_color = all_mesh_renderers [0].material.color;
			if (setTransparent) {
				foreach (Renderer r in all_mesh_renderers) {
					r.material = transparentMaterial;

				}
			} else {
				foreach (Renderer r in all_mesh_renderers) {
					r.material = opaqueMaterial;

					//r.material.color = new Color (current_color.r, current_color.g, current_color.b, 1.0f); 
				}
			}
		}



		// This function changes the color of an object by changing albedo of all the sub meshes
		public void ChangeColor(int color_index) {
			all_mesh_renderers = attached_bone.gameObject.GetComponentsInChildren<Renderer> ();
			current_color = all_mesh_renderers[0].material.color;
			foreach (Renderer r in all_mesh_renderers) {
				r.material.color = new Color(color_choices [color_index].r, color_choices [color_index].g, color_choices [color_index].b, 
					current_color.a);
			}
		}

		public void GetObjectInfo() {
			n = 0;
			Debug.Log ("GetObjectInfo: " + attached_bone.name);

			textFile = (TextAsset)Resources.Load (attached_bone.name);
			string[] linesInFile = textFile.text.Split ('\n');

			foreach (string line in linesInFile) {

				text_boxes [n].text = line;
				n += 1;

			}

		}



	}
}
