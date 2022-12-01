using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aimant : MonoBehaviour
{
    public int vitesse; //Vitesse du magnet
    public int rayonAimant; //Rayon de l'aimant
    private SphereCollider colliderAimant; //Collider de l'aimant
    private GameObject joueur; //Ref au joueur

    private void Start()
    {
        //Reference a l'aimant
        colliderAimant = GetComponent<SphereCollider>();
        

        //Ref au joueur
        joueur = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        //Suivre le joueur
        transform.position = new Vector3(joueur.transform.position.x, joueur.transform.position.y - joueur.GetComponent<Collider>().bounds.extents.y, joueur.transform.position.z);

        //Changer le rayon du collider selon la taille du joueur
        colliderAimant.radius = rayonAimant + (joueur.transform.localScale.magnitude / (rayonAimant * 2));
    }
    private void OnTriggerStay(Collider collision)
    {
        //Si nous touchons un item
        if (collision.gameObject.layer == 11)
        {
            //Faire bouger l'item vers le joueur
            collision.gameObject.transform.position = Vector3.MoveTowards(collision.gameObject.transform.position, transform.position, vitesse * Time.deltaTime);
        }
    }
}
