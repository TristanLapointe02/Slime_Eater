using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Comportement des Slimes sur le menu d'accueil
 * Fait par : Samuel Séguin
 */

public class JoueurMenu : MonoBehaviour
{
    public float jumpForce; //Force du saut du slime

    private void OnCollisionEnter(Collision collision)
    {
        //Lorsqu'on collide avec le sol, rebondir avec une force random
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce * Random.Range(0.5f, 1.75f));
    }
}
