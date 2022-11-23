using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class SolarSystemMgmt : MonoBehaviour
{
    // Lambda functions
    Func<int, int> DAYS_TO_HOURS = x => x * 24;

    // Variables
    public float G; // Constante universelle de gravitation augmentee pour rendre la simulation plus rapide
    GameObject[] bodies; // Corps presents dans la scene
    Text textDays;
    Text textHours;
    Text textYears;

    // Local Variables for Test
    int elapsedHours = 0;
    int elapsedDays = 0;
    int elapsedYears = 0;
    bool hourReached = false;

    // Start is called before the first frame update
    void Start()
    {
        // On recupere les textes
        textDays = GameObject.Find("Days").GetComponent<Text>();
        textHours = GameObject.Find("Hours").GetComponent<Text>();
        textYears = GameObject.Find("Years").GetComponent<Text>();

        // On recupere tous les corps du systeme solaire
        bodies = GameObject.FindGameObjectsWithTag("Bodies");

        // On applique la vitesse orbitale initiale a tous les corps
        ApplyInitialVelocityToBodies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ApplyPhysicsToBodies();
        UpdateTimePassed();
    }

    /*
     * Applique la vitesse orbitale a tous les corps
     */
    void ApplyInitialVelocityToBodies()
    {
        foreach (GameObject bodyA in bodies)
        {
            foreach (GameObject bodyB in bodies)
            {
                if (!bodyA.Equals(bodyB))
                {
                    // On recupere la masse du corps B
                    float mB = bodyB.GetComponent<Rigidbody>().mass;

                    // On recupere la distance entre les 2 corps
                    float d = Vector3.Distance(bodyA.transform.position, bodyB.transform.position);

                    // On fait pointer le corps A en direction du corps B
                    bodyA.transform.LookAt(bodyB.transform);

                    /* On applique la vitesse orbitale initiale au corps A
                     * selon la formule,
                     *
                     *      v0 = Sqrt(G * m2)
                     *                -----
                     *                  d
                    */
                    bodyA.GetComponent<Rigidbody>().velocity += bodyA.transform.right * Mathf.Sqrt((G * mB) / d);
                }
            }
        }
    }

    /*
     * Applique la physique sur les corps
     */
    void ApplyPhysicsToBodies()
    {
        foreach (GameObject bodyA in bodies)
        {
            foreach (GameObject bodyB in bodies)
            {
                if (!bodyA.Equals(bodyB))
                {

                    // On applique les forces de gravitation
                    ApplyGravity(bodyA, bodyB);

                    // On applique la rotation sous forme de vitesse angulaire
                    ApplyAngularVelocity(bodyA);
                }
            }
        }
    }

    /*
     * Applique les forces de gravitation a tous les corps
     */
    void ApplyGravity(GameObject bodyA, GameObject bodyB)
    {
        // On recupere les masses des 2 corps
        float mA = bodyA.GetComponent<Rigidbody>().mass;
        float mB = bodyB.GetComponent<Rigidbody>().mass;

        // On recupere la distance entre les 2 corps
        float d = Vector3.Distance(bodyA.transform.position, bodyB.transform.position);

        // On recupere le vecteur a utiliser pour appliquer la force (en direction de l'objet A)
        Vector3 dirVector = (bodyB.transform.position - bodyA.transform.position).normalized;

        /* On applique la loi universelle de la gravitation 
         * en tant que force sur le corps A selon la formule,
         *
         *      F = G * m1*m2
         *              -----
         *               d^2
        */
        bodyA.GetComponent<Rigidbody>().AddForce(dirVector * (G * (mA * mB) / (d * d)));
    }

    /*
     * Applique la rotation a un corps en utilisant sa periode
     */
    void ApplyAngularVelocity(GameObject body)
    {
        /* On applique la rotation sur 
        * le corps (vitesse angulaire) selon la formule,
        * 
        *     W = 2 * PI
        *         ------
        *         P / 3600
        */
        body.transform.Rotate(Vector3.up * ((Mathf.PI * 2) / GetRotationPeriodInHours(body.name)));
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
                float rotationPeriodHours = GetRotationPeriodInHours("Earth");

                // On recupere le temps necessaire au corps pour effectuer
                // une revolution autour du soleil
                float orbitalPeriodDays = GetOrbitalPeriodInDays("Earth");

                // On teste si l'angle actuel est equivalent a 1 heure terrestre
                // passee sur le corps en fonction de son temps de revolution
                if ( (curAngleDeg % (360 / rotationPeriodHours)) > 1)
                {
                    hourReached = false;
                }
                if ( (curAngleDeg % (360 / rotationPeriodHours) < 1) && !hourReached)
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



    /*
     * Obtient la periode de rotation du corps en heures
     */
    float GetRotationPeriodInHours(string name)
    {
        float periodHours = 0;

        switch(name)
        {
            case "Jupiter":
                periodHours = 9;
                break;
            case "Saturn":
                periodHours = 10;
                break;
            case "Neptune":
                periodHours = 16;
                break;
            case "Uranus":
                periodHours = 17;
                break;
            case "Earth":
                periodHours = 24;
                break;
            case "Mars":
                periodHours = 24;
                break;
            case "Sun":
                periodHours = DAYS_TO_HOURS(25);
                break;
            case "Moon":
                periodHours = DAYS_TO_HOURS(28);
                break;
            case "Mercury":
                periodHours = DAYS_TO_HOURS(58) + 15;
                break;
            case "Venus":
                periodHours = DAYS_TO_HOURS(116) + 18;
                break;
            default:
                periodHours = 24;
                break;
        }
        return periodHours;
    }

    /*
     * Obtient la periode de revolution autour du soleil en jours
     */
    int GetOrbitalPeriodInDays(string name)
    {
        int periodDays = 0;

        switch (name)
        {
            case "Jupiter":
                periodDays = 4335;
                break;
            case "Saturn":
                periodDays = 10758;
                break;
            case "Neptune":
                periodDays = 60225;
                break;
            case "Uranus":
                periodDays = 30708;
                break;
            case "Earth":
                periodDays = 365;
                break;
            case "Mars":
                periodDays = 687;
                break;
            case "Sun":
                periodDays = 0xFFFF;
                break;
            case "Moon":
                periodDays = 28;
                break;
            case "Mercury":
                periodDays = 176;
                break;
            case "Venus":
                periodDays = 225;
                break;
            default:
                periodDays = 365;
                break;
        }
        return periodDays;
    }

}
