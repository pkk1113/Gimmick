using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollowCtrl : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] GameObject target;
    [SerializeField, Range(0f, 1f)] float followSpeed;
    [SerializeField] float xMin = -99999;
    [SerializeField] float xMax = +99999;
    [SerializeField] float yMin = -99999;
    [SerializeField] float yMax = +99999;

    // Update is called once per frame
    void Update()
    {
        var targetPos = target.transform.position;
        targetPos.z = transform.position.z;

        if (targetPos.x < xMin)
        {
            targetPos.x = xMin;
        }
        else if (targetPos.x > xMax)
        {
            targetPos.x = xMax;
        }

        if (targetPos.y < yMin)
        {
            targetPos.y = yMin;
        }
        else if (targetPos.y > yMax)
        {
            targetPos.y = yMax;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(xMin, yMin), new Vector2(xMin, yMax)); // 0,0 -> 0,1 
        Gizmos.DrawLine(new Vector2(xMin, yMax), new Vector2(xMax, yMax)); // 0,1 -> 1,1 
        Gizmos.DrawLine(new Vector2(xMax, yMax), new Vector2(xMax, yMin)); // 1,1 -> 1,0
        Gizmos.DrawLine(new Vector2(xMax, yMin), new Vector2(xMin, yMin)); // 1,0 -> 0,0
    }
}
