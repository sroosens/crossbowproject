using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemMgmt : MonoBehaviour
{
    public float G; // Constante universelle de gravitation augmentée pour rendre la simulation plus rapide
    GameObject[] bodies; // Corps presents dans la scene

    // Start is called before the first frame update
    void Start()
    {
        // On récupère tous les corps du système solaire
        bodies = GameObject.FindGameObjectsWithTag("Bodies");

        // On applique la vitesse orbitale initiale à tous les corps
        ApplyInitialVelocityToBodies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ApplyPhysicsToBodies();
    }

    void ApplyInitialVelocityToBodies()
    {
        foreach (GameObject corpsA in bodies)
        {
            foreach (GameObject corpsB in bodies)
            {
                if (!corpsA.Equals(corpsB))
                {
                    // On récupère la masse du corps B
                    float mB = corpsB.GetComponent<Rigidbody>().mass;

                    // On récupère la distance entre les 2 corps
                    float d = Vector3.Distance(corpsA.transform.position, corpsB.transform.position);

                    // On fait pointer le corps A en direction du corps B
                    corpsA.transform.LookAt(corpsB.transform);

                    /* On applique la vitesse orbitale initiale au corps A
                     * selon la formule,
                     *
                     *      v0 = Sqrt(G * m2)
                     *                -----
                     *                  d
                    */
                    corpsA.GetComponent<Rigidbody>().velocity += corpsA.transform.right * Mathf.Sqrt((G * mB) / d);
                }
            }
        }
    }

    /*
     * Applique la physique sur les corps
     */
    void ApplyPhysicsToBodies()
    {
        foreach (GameObject corpsA in bodies)
        {
            foreach (GameObject corpsB in bodies)
            {
                if (!corpsA.Equals(corpsB))
                {

                    // On applique les forces de gravitation
                    ApplyGravity(corpsA, corpsB);

                    // On applique la rotation sous forme de vitesse angulaire
                    ApplyAngularVelocity(corpsA);
                }
            }
        }
    }

    void ApplyGravity(GameObject bodyA, GameObject bodyB)
    {
        // On récupère les masses des 2 corps
        float mA = bodyA.GetComponent<Rigidbody>().mass;
        float mB = bodyB.GetComponent<Rigidbody>().mass;

        // On récupère la distance entre les 2 corps
        float d = Vector3.Distance(bodyA.transform.position, bodyB.transform.position);

        // On récupère le vecteur à utiliser pour appliquer la force (en direction de l'objet A)
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

    void ApplyAngularVelocity(GameObject body)
    {
        /* On applique la formule de rotation sur 
        * le corps (vitesse angulaire) selon la formule,
        * 
        *     W = 2 * PI
        *         ------
        *         P / 3600
        */
        body.transform.Rotate(Vector3.up * ((Mathf.PI * 2) / GetRotationPeriodInHours(body.name)));
    }



    float GetRotationPeriodInHours(string name)
    {
        float period = 0;

        switch(name)
        {
            case "Earth":
                period = 24;
                break;
            case "Moon":
                period = 28 * 24;
                break;
            default:
                period = 24;
                break;
        }
        return period;
    }

}
