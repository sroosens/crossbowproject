using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform Target;
    public Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - Target.position;
    }

    private void LateUpdate()
    {
        transform.position = Target.position + offset;
    }

    public void setBodyToFollow(Transform _target)
    {
        Target = _target;
        if (Target.tag == "Bodies")
        {
        offset.z = -25 * (Target.localScale.z / 12.74f);
        }
        else if (Target.tag == "Spaceship")
        {
            offset = new Vector3(0.1f, 0.4f, -2);
        }
       
    }
}
