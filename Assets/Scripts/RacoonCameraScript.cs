using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacoonCameraScript : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;

    public Vector3 offset = Vector3.zero;

    private void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y + offset.y, transform.position.z);
    }
}
