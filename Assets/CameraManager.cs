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
}
