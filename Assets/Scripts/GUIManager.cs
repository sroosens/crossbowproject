using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static ObjectivesManager;

public class GUIManager : MonoBehaviour
{
    // Public variables
    public Text textDays;
    public Text textHours;
    public Text textYears;
    public Text textSpeedVal;
    public Transform arrow;
    public Text textDistanceObj;
    public Text textObj;

    public GameObject spaceship;
    public Dropdown dropDownPlanets;

    public ObjectivesManager objectivesManager;
    public Slider speedSlider;
    public SolarSystemManager solarSystemManager;
    public CameraManager cameraManager;

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
        planetsList = new List<string>();



        // On recupere tous les corps du systeme solaire
        bodies = GameObject.FindGameObjectsWithTag("Bodies");

        // On ajoute tous les noms des corps dans le menu drop down
        foreach (GameObject body in bodies)
        {
            planetsList.Add(body.name);
        }

        dropDownPlanets.AddOptions(planetsList);


        //Add listeners for GUI objects
        dropDownPlanets.onValueChanged.AddListener(delegate { UpdateCameraFollowPlanet(dropDownPlanets); });
        speedSlider.onValueChanged.AddListener(delegate { SpeedValueChanged(); });
        SpeedValueChanged();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimePassed();
    }

    private void FixedUpdate()
    {
        if (objectivesManager.initOK())
        {
            Vector3 dir;
            float a;
            Vector3 objPos = solarSystemManager.GetCelestial(objectivesManager.GetCurObjective().name).transform.position;
            Vector3 spaceShipPos = spaceship.transform.position;
            double dist = Math.Round(Vector3.Distance(spaceShipPos, objPos) * solarSystemManager.simulationFactor, 1);

            // Calcule et affiche la direction de l'objectif
            dir = spaceship.transform.InverseTransformPoint(objPos);
            a = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            a += 180;
            arrow.transform.localEulerAngles = new Vector3(0, 180, a);

            // Calcule et affiche la distance de l'objectif
            textDistanceObj.text = dist.ToString() + " mKm";
            textObj.text = "Find " + objectivesManager.GetCurObjective().name;

            // Objectif atteint
            if (dist < 10f)
            {
                objectivesManager.SetObjectiveReached(objectivesManager.GetCurObjective());
                objectivesManager.GetNextObjective();
            }
        }
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

                // On sort de foreach
                break;
            }
        }
    }
    public void PlayButtonClicked()
    {
        cameraManager.setBodyToFollow(spaceship.transform);
    }

    public void SpeedValueChanged()
    {
        float value = speedSlider.value;

        solarSystemManager.SetSpeed(value);
        textSpeedVal.text = value.ToString();
    }
}
