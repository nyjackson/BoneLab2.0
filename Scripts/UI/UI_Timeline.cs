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
    private Dictionary<string, string[]> timeKeys;

    public GameObject instructBox;
    public GameObject tipBox;
    public GameObject hintBox;
    public Button checkTimeButton;

    private bool m_Started; //might remove
    private bool bonesMissing = false;
    private bool checkBoneBox;

    public GameObject[] boneBoxes;
    private GameObject chosenBox;
    private List<string> incorrectBones;

    [HideInInspector]
    public bool finTimeline;

    void Start()
    {
        finTimeline = false;
        hintBox.SetActive(false);
        m_Started = true;
        incorrectBones = new List<string>();
        timeKeys = new Dictionary<string, string[]>();
        // actual
        timeKeys.Add("A.afarensis", new[] { "LH 5 Teeth" });
        timeKeys.Add("A.boisei", new[] {"KNM-ER 13750","KNM-ER 729", "KNM-ER 1805","KNM-ER 3230", "KNM-WT 17400",
           "KNM-ER 23000 Frontal", "KNM-ER 23000 Occipital", "KNM-ER 23000 Parietals",
          "KNM-ER 23000 Right Temporal", "KNM-ER 23000 Left Temporal"});
        timeKeys.Add("H.erectus", new[] { "KNM-ER 992a", "KNM-ER 820", "KNM-ER 3732", "KNM-ER 42700", "KNM-WT 15000m" });
        timeKeys.Add("H.neanderthalensis", new[] { "Krapina 49", "Krapina C" });

        //testing
        //timeKeys.Add("TestQuad", new[] { "KNM-ER 23000 Frontal", "Cubious" });
        //timeKeys.Add("TestedQuad", new[] { "dubious" });
    }

    public void OnButtonClick()
    {
        // if this is pressed and all boxes contain the correct bones, then fintimeline
        // will finish. 
        hintBox.GetComponentInChildren<Text>().text = "Incorrectly Placed/Missing Bones: ";
        for (int i = 0; i < boneBoxes.Length; i++)
        {
            chosenBox = boneBoxes[i];
            Debug.Log("Box: " + chosenBox.name);
            MyCollisions();
        }

        foreach (string str in incorrectBones)
        {
            Debug.Log("Incorrectly Placed: " + str);
        } 

        if(incorrectBones.Any())
        {
            PromptHint();
        }
        else
        {
            FinTimeline();
            // all bones are correct and in the right boxes
        }

    }
    private bool CorrectBox(string boneVal)
    {
        //Debug.Log("You have entered CorrectBox");
        checkBoneBox = false;
        foreach (KeyValuePair<string, string[]> kvp in timeKeys)
        {
            if (chosenBox.name == kvp.Key)
            {
               // Debug.Log("Tis the correct box");
                if (kvp.Value.Contains(boneVal))
                { // if the current key's array of values contains the correct bone
                    checkBoneBox = true;
                }
            }
        }
        return checkBoneBox;
    }

    void PromptHint()
    {
        hintBox.SetActive(true);
        foreach (string wrongPlace in incorrectBones)
        {
            hintBox.GetComponentInChildren<Text>().text += wrongPlace + "\n";
        }
    }

    public void FinTimeline()
    {
        Debug.Log("Timeline has been reached.");
        instructBox.GetComponentInChildren<Text>().text = "Timeline Complete! All fossils are in the correct order. New information unlocked!";
        hintBox.SetActive(false);
        tipBox.SetActive(false);
        checkTimeButton.gameObject.SetActive(false);
        finTimeline = true;
    }

    void MyCollisions()
    {
        bool boneCorrect = false;
        bool missingBones = false;
        Collider[] hitColliders = Physics.OverlapBox(chosenBox.transform.position, chosenBox.transform.localScale/2, chosenBox.transform.localRotation); //, layer Mask (but not needed)
        string[] bonesInBox = new string[hitColliders.Length];
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].transform.parent != null)
            { // if the object has a hull/ tis a bone
                boneCorrect = CorrectBox(hitColliders[i].transform.parent.name); // test for correct box
                bonesInBox[i] = hitColliders[i].transform.parent.name;
            }
            else
            {
                // may get rid of this in final version
                if (hitColliders[i].transform.name != "Table 2")
                {
                    if (!(timeKeys.ContainsKey(hitColliders[i].transform.name)))
                    {
                        boneCorrect = CorrectBox(hitColliders[i].transform.name); // test for correct box
                        bonesInBox[i] = hitColliders[i].transform.name;
                    }
                }
            }
            i++;
            missingBones = CheckAllBones(bonesInBox);
        }
    }

    private bool CheckAllBones(string[] bonesToCheck)
    {
        // checks if all correct bones are in the box
        bonesMissing = false;
        foreach (string str in timeKeys[chosenBox.name])
        {
            if (bonesToCheck.Contains(str))
            {
                if (incorrectBones.Contains(str)) { incorrectBones.Remove(str); }
            }
            else
            {
                bonesMissing = true;
                incorrectBones.Add(str);
            }
        }
        incorrectBones = new HashSet<string>(incorrectBones).ToList(); // gets rid of duplicates
        return bonesMissing;
        //check if the bones are in the correct box, and if all the correct bones are in the box.
    }
    private void OnDrawGizmos()
    { // draws the invisible lines of the OverlapBox in editor 
        Gizmos.color = Color.red;
        if (m_Started && chosenBox != null) Gizmos.DrawWireCube(chosenBox.transform.position, chosenBox.transform.localScale);
    }
}
