using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtZAxis : MonoBehaviour
{
    public Transform FollowTransform;
    public Transform TargetTransform;

    private void FixedUpdate()
    {
        Vector3 difference = TargetTransform.position - FollowTransform.position;
        float rotationZ = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
        FollowTransform.rotation = Quaternion.Euler(0.0f, rotationZ, 0.0f);
    }
}
