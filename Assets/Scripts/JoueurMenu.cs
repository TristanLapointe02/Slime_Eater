using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoueurMenu : MonoBehaviour
{
    [Header("Valeurs")]
    public float jumpForce; //Force du saut du slime

    [Header("Sons")]
    public AudioClip sonJump; // Son lorsque le slime saute

    private void OnCollisionEnter(Collision collision)
    {
        //Lorsqu'on collide avec le sol, rebondir
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce * Random.Range(0.5f, 1.75f));

        //Émettre un sound effect
        if (sonJump != null)
        {
            //GetComponent<AudioSource>().PlayOneShot(sonJump);
        }
    }
}
