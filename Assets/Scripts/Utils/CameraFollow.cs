using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform primaryTarget;
    [SerializeField] private Transform secondaryTarget;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    private void LateUpdate()
    {
        Vector3 midpoint = (primaryTarget.position + secondaryTarget.position) * 0.5f;
        transform.position = Vector3.Lerp(transform.position, 
            midpoint + offset, 
            Time.deltaTime * followSpeed);
    }
}
