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
    public Text playBtn;
    public Text textSpaceshipSpeed;

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
    bool playModeON = false;

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

    private void FixedUpdate()
    {
        if (objectivesManager.initOK() && GetPlayModeON())
        {
            Vector3 dir;
            float a;
            Vector3 objPos = solarSystemManager.GetCelestial(objectivesManager.GetCurObjective().name).transform.position;
            Vector3 spaceShipPos = spaceship.transform.position;
            double dist = Math.Round(Vector3.Distance(spaceShipPos, objPos) * solarSystemManager.simulationFactor, 1);
            double spaceSpeed = Math.Round((spaceship.GetComponent<SpaceshipMovement>().GetSpeed()), 0);

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

            textSpaceshipSpeed.text = "Ship Speed: " + spaceSpeed.ToString() + " mKm/s";
        }

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
        int days = solarSystemManager.GetEarthDays();
        int hours = solarSystemManager.GetEarthHours();
        int years = solarSystemManager.GetEarthYears();

        // On met a jour l'affichage
        textDays.text = "Days: " + days;
        textHours.text = "Hours: " + hours;
        textYears.text = "Years: " + years;
    }
    public void PlayButtonClicked()
    {
        if (!playModeON)
        {
            playModeON = true;
            cameraManager.setBodyToFollow(spaceship.transform);
            playBtn.text = "Exit Play Mode";

            // Show UI related
            ShowSpaceShipUI();
        }
        else
        {
            playModeON = false;
            cameraManager.setBodyToFollow(solarSystemManager.GetEarth().transform);
            playBtn.text = "Enter Play Mode";

            // Hide UI related
            HideSpaceShipUI();
        }
    }

    public void SpeedValueChanged()
    {
        float value = speedSlider.value;

        solarSystemManager.SetSpeed(value);
        textSpeedVal.text = "x" + value.ToString();
    }

    public bool GetPlayModeON()
    {
        return playModeON;
    }

    protected void ShowSpaceShipUI()
    {
        textSpaceshipSpeed.gameObject.SetActive(true);
        textObj.gameObject.SetActive(true);
        textDistanceObj.gameObject.SetActive(true);
        arrow.gameObject.SetActive(true);
    }

    protected void HideSpaceShipUI()
    {
        textSpaceshipSpeed.gameObject.SetActive(false);
        textObj.gameObject.SetActive(false);
        textDistanceObj.gameObject.SetActive(false);
        arrow.gameObject.SetActive(false);
    }
}
