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
            // On applique une pouss�e positive ou n�gative au vaisseau ( 3�me loi de Newton)
            thrust=Input.GetAxis("Vertical") * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                thrust = thrust * 2;
            }
            speed += thrust; // La pouss� modifie la vitesse ( 2�me loi de Newton)
            transform.Translate(0, 0, speed * 0.02f); 
            /*Le vide spatial n'entra�ne pas de frottement donc s' il n'y a pas de pouss�e
             le syst�me est en inertie donc la vitesse ne change pas (1�re loi de Newton) */
             
            // syst�mes de particules
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

            // Si bouton droit press�
            if (Input.GetMouseButton(1))
            {
                // On r�cup�re les rotations X, Y de la souris
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
