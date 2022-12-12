using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

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
        if (Target.tag == "Spaceship") // Spaceship
        {
            transform.rotation = Target.rotation;
            transform.position = Target.position + Target.TransformDirection(offset);
        }
        else // Celestials
        {
            transform.position = Target.position + offset;
        }
    }

    public void setBodyToFollow(Transform _target)
    {
        Target = _target;

        if (Target.tag == "Bodies")
        {
            // Calcul de l'offset en fct de la taille de la sphère
            offset.z = -25 * (Target.localScale.z / 12.74f);
        }
        else if (Target.tag == "Spaceship")
        {
            offset = new Vector3(0, 0.1f, -0.35f);
        }
    }
}
