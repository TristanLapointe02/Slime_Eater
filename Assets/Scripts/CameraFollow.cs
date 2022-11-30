using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; //Target de la camera
    public float smoothSpeed; //Vitesse a laquelle la camera smooth
    private Vector3 offset; //Offset de la caméra
    public float forceZoomOutCam; //Force a laquelle la cam s'eloigne selon le scale du joueur

    private void FixedUpdate()
    {
        // Changer distance entre caméra et joueur selon sa taille
        offset.z = (target.localScale.magnitude * -forceZoomOutCam) - (forceZoomOutCam + 1.5f); 
        offset.y = (target.localScale.magnitude * forceZoomOutCam) + forceZoomOutCam;

        //Changer la position de la camera
        Vector3 positionCam = target.position + offset;

        //Smooth avec un lerp
        Vector3 smoothPositionCam = Vector3.Lerp(transform.position, positionCam, smoothSpeed);
        transform.position = smoothPositionCam;

        //Dire a la cam de regarder son target
        //transform.LookAt(target);
    }
}
