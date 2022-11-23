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
    public Text textDays;
    public Text textHours;
    public Text textYears;

    // Local Variables
    GameObject[] bodies; // Corps presents dans la scene
    GameObject curBody;
    List<string> planetsList;


    // Local Variables for Test
    int elapsedHours = 0;
    int elapsedDays = 0;
    int elapsedYears = 0;
    bool hourReached = false;

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
        UpdateTimePassed();
    }

    void UpdateCameraFollowPlanet(Dropdown dropdown)
    {
        print("New Planet to Follow: " + dropdown.options[dropdown.value].text);

        foreach (GameObject body in bodies)
        {
            if(body.name.Equals(dropdown.options[dropdown.value].text))
            {
                curBody = body;
                cameraManager.setBodyToFollow(curBody.transform);
                break;
            }
        }
    }

    void UpdateTimePassed()
    {
        foreach (GameObject body in bodies)
        {
            if (body.name.Equals("Earth")) //Test uniquement sur la Terre, faudrait que ça soit lie au corps qu'on regarde
            {
                // On recupere l'angle de rotation actuel
                float curAngleDeg = Mathf.Abs(body.transform.eulerAngles.y);

                // On recupere le temps necessaire au corps pour effectuer
                // une rotation sur lui même
                float rotationPeriodHours = SolarSystemManager.GetRotationPeriodInHours("Earth");

                // On recupere le temps necessaire au corps pour effectuer
                // une revolution autour du soleil
                float orbitalPeriodDays = SolarSystemManager.GetOrbitalPeriodInDays("Earth");

                // On teste si l'angle actuel est equivalent a 1 heure terrestre
                // passee sur le corps en fonction de son temps de revolution
                if ((curAngleDeg % (360 / rotationPeriodHours)) > 1)
                {
                    hourReached = false;
                }
                if ((curAngleDeg % (360 / rotationPeriodHours) < 1) && !hourReached)
                {
                    elapsedHours += 1;
                    hourReached = true;
                }

                // 1 jour = 1 rotation complete du corps
                if (elapsedHours > rotationPeriodHours)
                {
                    elapsedHours = 0;
                    elapsedDays += 1;
                }

                // 1 an = 1 revolution complete du corps autour du soleil
                if (elapsedDays > orbitalPeriodDays)
                {
                    elapsedDays = 0;
                    elapsedYears += 1;
                }

                // On met a jour l'affichage
                textDays.text = "Days: " + elapsedDays;
                textHours.text = "Hours: " + elapsedHours;
                textYears.text = "Years: " + elapsedYears;
            }
        }
    }
}
