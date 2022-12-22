using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceshipMovement : MonoBehaviour
{
    public float thrust;
    public float speed;
    public float rotationX;
    public float rotationY;
    public bool  invertMouseY;
    public ParticleSystem thrust1;
    public ParticleSystem thrust2;
    public ParticleSystem thrust3;
    public ParticleSystem Boostthrust;
    public ParticleSystem BoostthrustL;
    public ParticleSystem BoostthrustR;
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
            // On applique une poussée positive ou négative au vaisseau ( 3ème loi de Newton)
            thrust=Input.GetAxis("Vertical") * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                thrust = thrust * 2;
            }
            speed += thrust; // La poussé modifie la vitesse ( 2ème loi de Newton)
            transform.Translate(0, 0, speed * 0.02f); 
            /*Le vide spatial n'entraîne pas de frottement donc s' il n'y a pas de poussée
             le système est en inertie donc la vitesse ne change pas (1ère loi de Newton) */
             
            // systèmes de particules
            if(thrust>0)
            {
                thrust1.Play();
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    Boostthrust.Play();
                }      
            }
            else if (thrust<0)
            {
                thrust2.Play();
                thrust3.Play();
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    BoostthrustL.Play();
                    BoostthrustR.Play();
                }

            }

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

   
}
