using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balle : MonoBehaviour
{
    public int lifeTime; //Temps de vie de la balle
    public float degats; //Degats de la balle
    public AudioClip sonBalleHit; //Son lorsque la balle hit quelquechose

    void Start()
    {
        //Detruire la balle apres x secondes
        Invoke("DetruireBalle", lifeTime);
    }

    //Fonction permettant de détruire la balle
    public void DetruireBalle()
    {
        //Destroy la balle
        if(gameObject != null)
        {
            //Jouer un sound effect
            AudioSource.PlayClipAtPoint(sonBalleHit, transform.position);

            //Detruire l'objet apres 2 secondes
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        //Si la balle est tirée par le joueur et touche un ennemi
        if (collision.gameObject.tag == "Ennemi")
        {
            //Faire des degats à l'ennemi
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(degats);
        }

        //Si la balle est tirée par un ennemi et touche le joueur
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Collision");
            //Faire des dégats au joueur
            collision.gameObject.GetComponent<ComportementJoueur>().TakeDamage(degats);
            
        }

        //Detruire la balle
        DetruireBalle();
    }
}
