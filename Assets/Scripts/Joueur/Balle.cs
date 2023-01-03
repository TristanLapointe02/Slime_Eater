using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Gestion des objets balles
 * Fait par : Tristan Lapointe et Samuel Séguin
 */

public class Balle : MonoBehaviour
{
    public int lifeTime; //Temps de vie de la balle
    public float degats; //Dégâts de la balle
    public AudioClip sonBalleHit; //Son lorsque la balle frappe quelque chose
    public bool goThrough; //Indiquer si on peut passer au travers des ennemis
    public bool explose; //Indiquer si la balle explose au contact
    public GameObject objetExplosion; //Si on peut exploser, ceci est l'objet d'explosion
    public AudioClip sonExplosion; //Son de l'explosion
    public float forceExplosion;
    public int rayonExplosion; //Rayon de l'explosion
    public bool slow; //Indiquer si la balle peut slow
    private TrailRenderer trail; //Trail de la balle

    void Start()
    {
        //Detruire la balle apres x secondes
        Invoke("DetruireBalle", lifeTime);

        //Assigner les références
        trail = GetComponent<TrailRenderer>();

        //Changer la taille de début du trail renderer selon la taille de la balle
        if(trail != null)
        {
            trail.startWidth = transform.localScale.magnitude / 2;
        }
    }

    private void Update()
    {
        //Si le jeu est en pause, se détruire
        if (ControleAmeliorations.pause || ControleMenu.pauseMenu)
        {
            DetruireBalle();
        }

        //Si on peut passer a travers des ennemis, devenir trigger
        if (goThrough)
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }

    //Fonction permettant de détruire la balle
    public void DetruireBalle()
    {
        //Destroy la balle
        if(gameObject != null)
        {
            //Detruire l'objet
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Si la balle est tirée par le joueur et touche un ennemi
        if (collision.gameObject.tag == "Ennemi")
        {
            faireDegatsEnnemi(collision.gameObject);
        }

        //Si la balle est tirée par un ennemi et touche le joueur
        if (collision.gameObject.tag == "Player")
        {
            //Disable la collision
            GetComponent<Collider>().enabled = false;

            //Faire des dégats au joueur
            collision.gameObject.GetComponent<ComportementJoueur>().TakeDamage(degats);
        }

        //Si on ne peut pas passer a travers des ennemis
        if(goThrough == false)
        {
            //Si on peut exploser, le faire
            if (explose)
            {
                Explosion(collision.gameObject);
            }

            //Detruire la balle
            DetruireBalle();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Si la balle est tirée par le joueur et touche un ennemi
        if (collision.gameObject.tag == "Ennemi" && goThrough)
        {
            faireDegatsEnnemi(collision.gameObject);
        }

        //Si la balle touche le mur
        if(collision.gameObject.layer == 12)
        {
            DetruireBalle();
        }
    }

    //Fonction qui permet de faire des dégâts à l'ennemi
    public void faireDegatsEnnemi(GameObject ennemi)
    {
        //Faire des degats à l'ennemi
        ennemi.gameObject.GetComponent<EnemyController>().TakeDamage(degats);

        //Jouer un sound effect
        AudioSource.PlayClipAtPoint(sonBalleHit, transform.position);

        //Si on peut exploser, et qu'on est go trough le faire quand même au contact
        if (goThrough && explose)
        {
            Explosion(ennemi);
        }

        //Si on peut slow l'ennemi, le faire
        if (slow)
        {
            ennemi.GetComponent<EnemyController>().StartCoroutine(ennemi.GetComponent<EnemyController>().SlowMovement(75, 5));
        }
    }

    //Fonction d'explosion
    public void Explosion(GameObject source)
    {
        //Spawn un objet visuel
        GameObject effet = Instantiate(objetExplosion, gameObject.transform.position, Quaternion.identity);

        //Changer la grosseur de l'effet selon notre rayon
        effet.transform.localScale = new Vector3(rayonExplosion, rayonExplosion, rayonExplosion);

        //Le detruire tout de suite après
        Destroy(effet, 0.15f);

        //Jouer un sound effect
        AudioSource.PlayClipAtPoint(sonExplosion, gameObject.transform.position);

        //Pour tous les colliders dans la zone d'explosion
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, rayonExplosion);

        //Pour tous les collider touchés
        foreach (var collider in hitColliders)
        {
            //Trouver les ennemis
            if (collider.gameObject.TryGetComponent(out EnemyController ennemy))
            {
                //Pour tous sauf la source, si c'est pas le boss
                if(ennemy.gameObject.GetInstanceID() != source.GetInstanceID())
                {
                    //Leur faire subir une explosion
                    ennemy.SubirExplosion(forceExplosion, transform.position, rayonExplosion, degats);
                }
            }
        }
    }
}
