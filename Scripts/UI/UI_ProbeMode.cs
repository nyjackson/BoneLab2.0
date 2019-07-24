using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

// This class highlights the selected game object, and spawns an information box, 
// that shows the users details on the objects, such as ID, Taxonomy, Elements, Location, Date of Discovery, etc.


// Nyala Jackson
// njackson@vassar.edu
// Vassar College '21

public class ProbeMode : MonoBehaviour
{
    // Info Box Vars
    public Canvas infoCanvas;
    public Text boneID;
    public GameObject boneTags;
    private Text[] boneInfo;
    public TextAsset file_path;
    private int tagCount = 0;
    private Vector3 oldBoxScale;
    private GameObject bone;
    private Bounds combinedBounds;

    private bool timelineFin;
    private UI_Timeline timeline;

    private void Start()
    {
        Debug.Log("Welcome to Probe Mode.");
        infoCanvas.enabled = false;
    }
    public void OnEnter(GameObject chosenBone)
    {
        bone = chosenBone.transform.parent.gameObject;
        if(infoCanvas != null) { infoCanvas.transform.localScale = new Vector3(0.6f, 0.6f, 0.8f); }
        Debug.Log("On Collision Enter");
        GenerateInfoBox(true);

    }

    public void OnExit(GameObject chosenBone)
    {
        Debug.Log("On Collision Exit");
        infoCanvas.enabled = false;
        Debug.Log("No more info box.");
    }

    void ReadTextFile()
    { // read info from BoneName.txt in the Resources folder.
        StreamReader inp_stm = new StreamReader("Assets/Resources/Probe Mode Info/" + file_path.name + ".txt");

        if (IsTimelineBone() && !timelineFin) // if its a timeline bone and if the timeline is not complete
        { 
            string[] boneLines = System.IO.File.ReadAllLines("Assets/Resources/Probe Mode Info/" + file_path.name + ".txt");
            boneInfo[0].text += " " + boneLines[0];
            boneInfo[1].text += " ??";
            boneInfo[2].text += " ??";
            boneInfo[3].text += " " + boneLines[3];
            boneInfo[4].text += " ??";

        }
        else
        {
            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                // assuming everything follows the same pattern, and the amount is the same.
                try
                {
                    boneInfo[tagCount].text += " " + inp_ln;
                    tagCount++;

                }
                catch (System.IndexOutOfRangeException e)
                {
                    Debug.Log("There are too many lines in the doc.");
                }
            }
        }
        inp_stm.Close();
        tagCount = 0;
    } 

    void GenerateInfoBox(bool active) // turns on the infobox in the canvas and adds the bone's info.
    {
        Debug.Log("Trying to generate an info box");
        if (active == true)
        {
            Debug.Log("Summoning Info Box");
            infoCanvas.transform.SetParent(bone.transform, true);
            infoCanvas.transform.localPosition = new Vector3(-GetBoneBounds().size.x * 20, GetBoneBounds().size.y, GetBoneBounds().size.z);
            infoCanvas.enabled = true;
            boneID.text = bone.name;
            boneInfo = boneTags.gameObject.GetComponentsInChildren<Text>();
            boneInfo[0].text = "Bone:";
            boneInfo[1].text = "Species:";
            boneInfo[2].text = "Geological Age:";
            boneInfo[3].text = "Site Location:";
            boneInfo[4].text = "Similar Specimen:";
            ReadTextFile();

        }
    }

    public void Update()
    {
        // makes the info box always face and follow the user
        if (infoCanvas != null)
        {
            Camera camera = Camera.main;
            infoCanvas.transform.rotation = camera.transform.rotation;
        }
        timelineFin = GameObject.Find("Timeline Window").GetComponent<UI_Timeline>().finTimeline;
    }

    Bounds GetBoneBounds()
    {
        Vector3 center = Vector3.zero;
        Renderer[] rs = bone.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
        {
            center += r.bounds.center;
        }
        center /= bone.transform.childCount; // center is average center of children
        // Now you have a center, calculate the bounds by creating a zero sized, 'Bounds,'
        Bounds bounds = new Bounds(center, Vector3.zero);

        foreach (Renderer r in rs)
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }

    private bool IsTimelineBone() { //add on to this and check if timeline is complete or not.
        GameObject[] timeChildren = GameObject.FindGameObjectsWithTag("Timeline Bone");
        foreach (GameObject child in timeChildren) {
            if (child.transform.parent.name == bone.name) {
                return true;
            }
        }
        return false;
    }
}
