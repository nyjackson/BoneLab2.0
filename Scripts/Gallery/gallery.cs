using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Aubrey Simonson
// asimonso@wellesley.edu
// August 2017

public class gallery: MonoBehaviour {
	
	//this creates the array.  More items can be added to the array in the inspector
	public Material[] images;

	//this creates the renderer who's material you're changing
	public Renderer rend;

	//this is the index number of the image that we are currently displaying
	public int num;


	void Start ()
	{
		rend = GetComponent<Renderer> ();
		rend.enabled = true;
	}
		

	void Update()
	{
		//this makes it so that if you hit the previous button too many times, you go up to the top of the array
		if (num < 0)
			num = 1000;

		// we want this material index now
		int index = Mathf.FloorToInt(num);

		// take a modulo with materials count so that animation repeats
		index = index % images.Length;
		num = index;

		// assign it to the renderer
		rend.sharedMaterial = images[index];
	}
		
}
