using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollowCtrl : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] GameObject target;

    // Update is called once per frame
    void Update()
    {
        var targetPos = target.transform.position;
        targetPos.z = transform.position.z; 
        transform.position = Vector3.Lerp(transform.position, targetPos, 1f);
    }
}
