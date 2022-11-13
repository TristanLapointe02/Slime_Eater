using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; //Target de la camera
    public float smoothSpeed; //Vitesse a laquelle la camera smooth
    public Vector3 offset; //Offset de la caméra

    private void FixedUpdate()
    {
        //Changer la position de la camera
        //Trouver la position
        Vector3 positionCam = target.position + offset;

        //Smooth avec un lerp
        Vector3 smoothPositionCam = Vector3.Lerp(transform.position, positionCam, smoothSpeed);
        transform.position = smoothPositionCam;

        //Dire a la cam de regarder son target
        transform.LookAt(target);
    }
}
