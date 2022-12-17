using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class SolarSystemManager : MonoBehaviour
{
    // Lambda functions
    static Func<int, int> DAYS_TO_HOURS = x => x * 24;

    // Public Variables
    public float G; // Constante universelle de gravitation augmentee pour rendre la simulation plus rapide
    public float simulationFactor = 0.25f; // Paramètre permettant de réduire les distances, masses etc pour que la visualisation reste possible

    [SerializeField]
    bool IsElepticalOrbit = true;

    // Local Variables
    GameObject[] bodies; // Corps presents dans la scene

    // Start is called before the first frame update
    void Start()
    {
        // On recupere tous les corps du systeme solaire
        bodies = GameObject.FindGameObjectsWithTag("Bodies");

        // On applique la vitesse orbitale initiale a tous les corps sauf la Lune
        ApplyInitialOrbitSpeedToBodies();
    }

    private void FixedUpdate()
    {
        ApplyPhysicsToBodies();
    }

    /*
     * Applique la vitesse orbitale a tous les corps
     */
    void ApplyInitialOrbitSpeedToBodies()
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

                    if (!IsElepticalOrbit)
                    {
                        /* On applique la vitesse orbitale initiale au corps A
                         * selon la formule,
                         *
                         *      v0 = Sqrt(G * m2)
                         *                -----
                         *                  d
                        */
                        bodyA.GetComponent<Rigidbody>().velocity += bodyA.transform.right * Mathf.Sqrt((G * mB * simulationFactor) / d);
                    }
                    else
                    {
                        /* On applique la vitesse orbitale eliptique initiale au corps A
                         * selon la formule,
                         *
                         *      v0 = Sqrt(G * m2 * ( 2 -  1 ))
                         *                           --   --
                         *                           d    a
                        */
                        bodyA.GetComponent<Rigidbody>().velocity += bodyA.transform.right * Mathf.Sqrt((G * mB * simulationFactor) * ((2 / d) - (1 / (d * 1.5f))));
                    }
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
        bodyA.GetComponent<Rigidbody>().AddForce(dirVector * (G * ((mA * mB) * simulationFactor) / (d * d)));
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
        body.transform.Rotate(Vector3.up * ((Mathf.PI * 2) / GetRotationPeriodInHours(body.name)) * Time.fixedDeltaTime);
    }

    public void SetSpeed(float _speed)
    {
        print("New speed: " + _speed);
        Time.timeScale = _speed;
    }

    public float GetSunScale()
    {
        foreach (GameObject body in bodies)
        {
            if (body.name == "Sun")
                return body.transform.localScale.x;
        }
        return 0f;
    }

    public GameObject GetCelestial(string name)
    {
        foreach (GameObject body in bodies)
        {
            if (body.name == name)
                return body;
        }
        return null;
    }

    public GameObject GetSun()
    {
        foreach (GameObject body in bodies)
        {
            if (body.name == "Sun")
                return body;
        }
        return null;
    }

    public GameObject GetEarth()
    {
        foreach (GameObject body in bodies)
        {
            if (body.name == "Earth")
                return body;
        }
        return null;
    }

    /*
     * Obtient la periode de rotation du corps en heures
     */
    static public float GetRotationPeriodInHours(string name)
    {
        float periodHours = 0;

        switch (name)
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
    static public int GetOrbitalPeriodInDays(string name)
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

    /*
     * not used
     */
    static public float GetDistanceToTheSunInMKm(string name)
    {
        float distance = 0;

        switch (name)
        {
            case "Jupiter":
                distance = 738.34f;
                break;
            case "Saturn":
                distance = 1426.7f;
                break;
            case "Neptune":
                distance = 4498.4f;
                break;
            case "Uranus":
                distance = 2870.7f;
                break;
            case "Earth":
                distance = 227.9f;
                break;
            case "Mars":
                distance = 227.9f;
                break;
            case "Sun":
                distance = 0f;
                break;
            case "Moon":
                distance = 149.6f;
                break;
            case "Mercury":
                distance = 57.9f;
                break;
            case "Venus":
                distance = 108.2f;
                break;
            default:
                distance = 227.9f;
                break;
        }
        return distance;
    }
}
