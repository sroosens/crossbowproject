using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceshipMovement : MonoBehaviour
{
    public float speed;
    public float rotationX;
    public float rotationY;
    public bool  invertMouseY;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // On applique une vitesse au vaisseau
        speed += Input.GetAxis("Vertical") * Time.deltaTime;
        transform.Translate(0, 0, speed * 0.02f);

        // Si bouton droit pressé
        if (Input.GetMouseButton(1))
        {
            // On récupère les rotations X, Y de la souris
            rotationX += Input.GetAxis("Mouse Y") * (invertMouseY ? -1 : 1);
            rotationY += Input.GetAxis("Mouse X");

            // On limite la rotation X entre -90 deg et +90 deg
            //rotationX = Mathf.Clamp(rotationX, -90, 90);

            // On effectue une rotation d'Euler sur le vaisseau
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
    }
}
