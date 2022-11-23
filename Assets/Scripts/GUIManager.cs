using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    // Public variables
    public Dropdown dropDownPlanets;
    public CameraManager cameraManager;

    // Local Variables
    GameObject[] bodies; // Corps presents dans la scene
    List<string> planetsList;

    // Start is called before the first frame update
    void Start()
    {
        planetsList = new List<string> ();

        // On recupere tous les corps du systeme solaire
        bodies = GameObject.FindGameObjectsWithTag("Bodies");

        // On ajoute tous les noms des corps dans le menu drop down
        foreach (GameObject body in bodies)
        {
            planetsList.Add(body.name);
        }
        dropDownPlanets.AddOptions(planetsList);

        //Add listener for when the value of the Dropdown changes, to take action
        dropDownPlanets.onValueChanged.AddListener(delegate { UpdateCameraFollowPlanet(dropDownPlanets); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateCameraFollowPlanet(Dropdown dropdown)
    {
        print("New Planet to Follow: " + dropdown.options[dropdown.value].text);

        foreach (GameObject body in bodies)
        {
            if(body.name.Equals(dropdown.options[dropdown.value].text))
            {
                cameraManager.setBodyToFollow(body.transform);
                break;
            }
        }
    }
}
