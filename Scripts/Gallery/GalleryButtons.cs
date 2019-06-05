using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GalleryButtons : MonoBehaviour {
	public gallery Gallery;
	public Text textBox; 

	private int photoNum;
	private string imageName;
	private TextAsset textFile;



	void Update() {
		photoNum = Gallery.num;
		imageName = Gallery.images [photoNum].name;
		textFile = (TextAsset)Resources.Load (imageName);

		textBox.text = textFile.text;

	}

	public void LeftClick() {
		photoNum--;
		Gallery.num = photoNum;
	}


	public void RightClick() {
		photoNum++;
		Gallery.num = photoNum;
	
	}



}
