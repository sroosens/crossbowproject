using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{
    public float speed= 5;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      if ( Input.GetKey(KeyCode.Space))
        {
            transform.Translate(0, 0, speed + 5);
        };
    }
}
