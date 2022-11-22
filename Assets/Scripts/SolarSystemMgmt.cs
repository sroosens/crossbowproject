using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemMgmt : MonoBehaviour
{
    readonly float G = 200f; // Constante universelle de gravitation augment�e pour rendre la simulation plus rapide
    GameObject[] bodies; // Corps presents dans la scene

    // Start is called before the first frame update
    void Start()
    {
        // On r�cup�re tous les corps du syst�me solaire
        bodies = GameObject.FindGameObjectsWithTag("Bodies");

        // On applique la vitesse orbitale initiale � tous les corps
        ApplyInitialVelocityToBodies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ApplyGravityToBodies();
    }

    /*
     * Applique les forces de gravit�s 
     * sur les plan�tes/�toiles entre elles
     */
    void ApplyGravityToBodies()
    {
        foreach (GameObject corpsA in bodies)
        {
            foreach (GameObject corpsB in bodies)
            {
                if (!corpsA.Equals(corpsB))
                {
                    // On r�cup�re les masses des 2 corps
                    float mA = corpsA.GetComponent<Rigidbody>().mass;
                    float mB = corpsB.GetComponent<Rigidbody>().mass;

                    // On r�cup�re la distance entre les 2 corps
                    float d = Vector3.Distance(corpsA.transform.position, corpsB.transform.position);

                    // On r�cup�re le vecteur � utiliser pour appliquer la force (en direction de l'objet A)
                    Vector3 dirVector = (corpsB.transform.position - corpsA.transform.position).normalized;

                    /* On applique la loi universelle de la gravitation 
                     * en tant que force sur le corps A selon la formule,
                     *
                     *      F = G * m1*m2
                     *              -----
                     *               d^2
                    */
                    corpsA.GetComponent<Rigidbody>().AddForce(dirVector * (G * (mA * mB) / (d * d)));
                }
            }
        }
    }

    void ApplyInitialVelocityToBodies()
    {
        foreach (GameObject corpsA in bodies)
        {
            foreach (GameObject corpsB in bodies)
            {
                if (!corpsA.Equals(corpsB))
                {
                    // On r�cup�re la masse du corps B
                    float mB = corpsB.GetComponent<Rigidbody>().mass;

                    // On r�cup�re la distance entre les 2 corps
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

}
