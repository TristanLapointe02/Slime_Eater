using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Comportement des Slimes sur le menu d'accueil
 * Fait par : Samuel S�guin
 */

public class JoueurMenu : MonoBehaviour
{
    public float jumpForce; //Force du saut du slime
    Rigidbody rb; //R�f�rence au rigidbody du slime
    public int fallMultiplier; //Multiplicateur de gravit�
    public float rangeJumpAleatoire; //Range du jump aleatoire

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        //Am�lioration de gravit�
        if (rb.velocity.y < 0)
        {
            rb.velocity += fallMultiplier * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Lorsqu'on collide avec le sol, rebondir avec une force random
        GetComponent<Rigidbody>().AddForce(jumpForce * Random.Range(1-rangeJumpAleatoire, 1+ rangeJumpAleatoire) * Vector3.up);
    }
}
