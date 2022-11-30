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
        //Si nous touchons un ennemi
        if (collision.gameObject.tag == "Ennemi")
        {
            //Faire des degats à l'ennemi
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(degats);
        }

        //Detruire la balle
        DetruireBalle();
    }
}
