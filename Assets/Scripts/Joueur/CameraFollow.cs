using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * Description : Movements de la cam�ra qui suit le joueur
 * Fait par: Tristan Lapointe et Samuel S�guin
 */

public class CameraFollow : MonoBehaviour
{
    public Transform target; //Target de la cam�ra
    private Vector3 offset; //Offset de la cam�ra
    public float forceZoomOutCam; //Force a laquelle la cam�ra s'�loigne selon la taille du joueur
    private CinemachineVirtualCamera vcam; //Virtual Camera reference
    private CinemachineTransposer _transposer; //Composer

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        _transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void FixedUpdate()
    {
        //Changer la position de la cam�ra selon la taille du joueur
        offset.y = (target.localScale.magnitude * forceZoomOutCam) + (forceZoomOutCam*10f);
        offset.z = -(target.localScale.magnitude * forceZoomOutCam) - (forceZoomOutCam * 10f);

        //Appliquer un offset afin de g�rer l'angle de vue
        _transposer.m_FollowOffset = new Vector3 (0, offset.y, offset.z);
    }
}
