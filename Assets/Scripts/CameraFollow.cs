using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; //Target de la camera
    public float smoothSpeed; //Vitesse a laquelle la camera smooth
    private Vector3 offset; //Offset de la caméra
    public float forceZoomOutCam; //Force a laquelle la cam s'eloigne selon le scale du joueur
    public CinemachineVirtualCamera vcam; //Virtual Camera ref
    public CinemachineTransposer _transposer; //Composer

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        _transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void FixedUpdate()
    {
        //ANCIEN SCRIPT DE CAMERA
        /*// Changer distance entre caméra et joueur selon sa taille
        offset.z = (target.localScale.magnitude * -forceZoomOutCam) - (forceZoomOutCam + 1.5f); 
        offset.y = (target.localScale.magnitude * forceZoomOutCam) + forceZoomOutCam;

        //Changer la position de la camera
        Vector3 positionCam = target.position + offset;

        //Smooth avec un lerp
        transform.position = Vector3.Lerp(transform.position, positionCam, smoothSpeed);*/

        //Changer la position de la caméra selon la taille du joueur
        offset.z = (target.localScale.magnitude * -forceZoomOutCam*1.5f) - (forceZoomOutCam*5);
        offset.y = (target.localScale.magnitude * forceZoomOutCam) + (forceZoomOutCam*2.5f);

        _transposer.m_FollowOffset = new Vector3 (0, offset.y, offset.z);
    }
}
