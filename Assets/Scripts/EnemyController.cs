using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyController : MonoBehaviour
{
    public StatsEnemy enemy; // type d'ennemi
    public GameObject joueur; // Référence au joueur
    private Vector3 directionJoueur; // distance et direction entre ennemi et joueur
    public float vie;

    private void Start()
    {
        vie = enemy.vieMax;
        gameObject.transform.localScale =  new Vector3(enemy.tailleEnnemi, enemy.tailleEnnemi, enemy.tailleEnnemi);
        gameObject.GetComponent<MeshRenderer>().material.color = enemy.couleur;
        joueur = GameObject.FindGameObjectWithTag("Player");


    }

    private void FixedUpdate()
    {
        directionJoueur = joueur.transform.position - transform.position; // Obtenir la distance et direction avec joueur
        if (enemy.ranged == false)
        {
            Move();
        }

        if (enemy.ranged == true)
        {
            // Si le joueur est assez proche, l'attaquer
            if (directionJoueur.magnitude <= 25f + joueur.GetComponent<Collider>().bounds.size.x)
            {
                //Attack();
            }
            // Sinon bouger vers lui
            else
            {
                Move();
            }
        }
    }

    private void Move()
    {
        // Bouger vers le joueur
        transform.position += directionJoueur.normalized * enemy.vitesse * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            // Faire perdre de la vie au joueur
            // Ajouter animation de mort éventuellement
        }
    }

    public void TakeDamage(float damage)
    {
        vie -= damage;
        if (vie <= 0)
        {
            Destroy(gameObject);
        }
    }

}
