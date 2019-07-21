using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

// This class checks if the bones are in the right boxes, and changes
// the timeline ui window, accordingly. 


// Nyala Jackson
// njackson@vassar.edu
// Vassar College '21

public class UI_Timeline : MonoBehaviour
{
    //public GameObject[] timeBoxes;
    private Dictionary<string, string[]> timeKeys;
    public GameObject hintBox;
    public GameObject instructBox;
    public GameObject tipBox;
    private bool m_Started;
    private bool bonesMissing;
    private bool checkBoneBox;
    private bool buttonPressed;
    public GameObject[] testArray;

    void Awake() {
        //hintBox.SetActive(false); // disables hintbox
    }
    void Start()
    {
        m_Started = true;
        timeKeys = new Dictionary<string, string[]>();
        timeKeys.Add("A.afarensis", new[] { "LH 5 Teeth"});
        timeKeys.Add("A.boisei", new[] {"KNM-ER 13750","KNM-ER 729", "KNM-ER 1805","KNM-ER 3230", "KNM-WT 17400",
            "KNM-ER 23000 Frontal", "KNM-ER 23000 Occipital", "KNM-ER 23000 Parietals",
            "KNM-ER 23000 Right Temporal", "KNM-ER 23000 Left Temporal"});
        timeKeys.Add("H.erectus", new[] {"KNM-ER 992a","KNM-ER 820","KNM-ER 3732","KNM-ER 42700","KNM-ER 15000"});
        timeKeys.Add("H.neanderthalensis",new[] {"STL Teshik Tash","Krapina 49", "Krapina C"});
        timeKeys.Add("TestQuad" , new[] { "KNM-ER 23000 Frontal", "Cubious"});
        timeKeys.Add("TestedQuad", new[] { "dubious" });
        // i need to find a way to ensure to check when all boxes are correct and all bones are in, timeline is complete 
        // i can do it in updatye, but I have to make sure everything with this script
        // clears the tests. 
    }

    void CheckBones() {
        // repeatedly check? Have button? Or only check when all boxes are filled?
        // in hints, if original pre-placed fossils are moved, notify player that it must be placed back if missing,
        // get checkall bones and correct box to determine if all are correct and all are included. 
        // add/compose the hint list. 

    }

    public void OnButtonClick() {
        // if this is pressed and all boxes contain the correct bones, then fintimeline
        // will finish. 
        buttonPressed = true;
       
    }
    private bool CorrectBox(string boneVal)
    {
        checkBoneBox = false;
        //string[] boxBones;
        string correctBox = "";
        foreach (KeyValuePair<string, string[]> kvp in timeKeys) {
            if (this.name == kvp.Key) {
                if (kvp.Value.Contains(boneVal))
                { // if the current key's array of values contains the correct bone
                    correctBox = this.name;
                    Debug.Log("Yes, " + correctBox + "s the correct box.");
                    checkBoneBox = true;
                }
            }
            else if (kvp.Value.Contains(boneVal)) { // get correct box
                correctBox = kvp.Key;
                PromptHint(this.name,correctBox,boneVal);
            }
        }
        return checkBoneBox;


    }

    void PromptHint(string currentBox, string correctBox, string bone) {
        // call when button pressed and hints need to spawn
        hintBox.SetActive(true);
        string hintCombo = currentBox + "+" + correctBox;
        Debug.Log(hintCombo);
        Dictionary<string, string> hintCombos = new Dictionary<string, string>();
        foreach (KeyValuePair<string,string> kvp in hintCombos) {
            if (hintCombo == kvp.Key) {
                hintBox.GetComponent<Text>().text = kvp.Value;
            }
        }
    }

    void FinTimeline() {
        Debug.Log("Fin Timeline has been reached.");
        instructBox.GetComponent<Text>().text = "Timeline Complete! All fossils are in the correct order.";
        hintBox.SetActive(false);
        tipBox.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        MyCollisions();
    }

    void MyCollisions() {
        bool boneCorrect = false;
        bool missingBones = false;
        Collider[] hitColliders = Physics.OverlapBox(this.transform.position,transform.localScale/2,Quaternion.identity); //, layer Mask (but not needed)
        string[] bonesInBox = new string[hitColliders.Length]; 
        int i = 0;
        while (i < hitColliders.Length) {
            if (hitColliders[i].transform.parent != null) { // if the object has a hull/ tis a bone
                Debug.Log("Hit: " + hitColliders[i].transform.parent.name);
                boneCorrect = CorrectBox(hitColliders[i].transform.parent.name); // test for correct box
                bonesInBox[i] = hitColliders[i].transform.parent.name;
            }
            else {
                Debug.Log("Hit: " +hitColliders[i]);
                boneCorrect = CorrectBox(hitColliders[i].transform.name); // test for correct box
                bonesInBox[i] = hitColliders[i].transform.name;
            }
            i++;
            missingBones = CheckAllBones(bonesInBox);
        }
    }

    private bool CheckAllBones(string[] bonesToCheck) {
        // checks if all correct bones are in the right box
        foreach (string str in timeKeys[this.name]) {
            Debug.Log("This is the collision dude: " + this.name);
            Debug.Log("This is the str needed: " + str);
            if (bonesToCheck.Contains(str))
            {
                Debug.Log("There are no missing bones, so far.");
                bonesMissing = false;
            }
            else {
                Debug.Log("Missing Bone: " + str);
                bonesMissing = true;
                //break;
            }
        }
        return bonesMissing;
        //check if the bones are in the correct box, and if all the correct bones are in the box.
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_Started) Gizmos.DrawWireCube(transform.position,transform.localScale);
    }
}
