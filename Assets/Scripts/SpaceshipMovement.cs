using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceshipMovement : MonoBehaviour
{
    public float speed;
    public float rotationX;
    public float rotationY;
    public bool  invertMouseY;
    public ParticleSystem boost;
    public GUIManager GUIManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
        rotationX = 0;
        rotationY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GUIManager.GetPlayModeON())
        {
            // On applique une vitesse au vaisseau
            speed += Input.GetAxis("Vertical") * Time.deltaTime;
            transform.Translate(0, 0, speed * 0.02f);


            /*        if(Input.GetAxis("Vertical")>0)
                    {*//*
            boost.Play();
            *//*        }*/


            // Si bouton droit pressé
            if (Input.GetMouseButton(1))
            {
                // On récupère les rotations X, Y de la souris
                rotationX += Input.GetAxis("Mouse Y") * (invertMouseY ? -1 : 1);
                rotationY += Input.GetAxis("Mouse X");

                rotationX = Mathf.Clamp(rotationX, -90, 90);

                // On effectue une rotation d'Euler sur le vaisseau
                transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
            }
        }
    }

    public float GetSpeed()
    { 
        return speed;
    }

    /* TODO: Pierre
    // Obtenir l'input de l'utilisateur pour contrôler les moteurs du vaisseau spatial
    float thrustInput = Input.GetAxis("Thrust");

    // Loi 1
    // Appliquer une force de poussée vers l'avant au vaisseau spatial en utilisant les moteurs
    rigidbody.AddForce(Vector3.forward* thrustInput * thrustForce);

    // Loi 3
    // Appliquer une force égale et opposée sur l'objet qui exerce la force (dans ce cas, les moteurs)
    engines.AddForce(-Vector3.forward* thrustInput * thrustForce);
    */
}
