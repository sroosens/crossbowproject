using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{
    public float speed;
    public float rotationX;
    public float rotationY;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      speed += Input.GetAxis("Vertical")*Time.deltaTime;
           transform.Translate(0,0,speed);
        rotationX += Input.GetAxis("Mouse Y");
        rotationY += Input.GetAxis("Mouse X");
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        transform.rotation= Quaternion.Euler(rotationX,rotationY,0);
       
    }
}
