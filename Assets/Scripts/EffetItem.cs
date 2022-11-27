using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffetItem : MonoBehaviour

{
    public StatsItems item; // type d'item
    public GameObject joueur; // Référence au joueur
    public string nom; // nom de l'item
    public float valeur; // valeu de l'item
    public float duree; //Duree de l'effet


    // Start is called before the first frame update
    void Awake()
    {
        // Assigner valeurs du scriptableObject
        nom = item.name;
        valeur = item.valeur;
        duree = item.duree;
        gameObject.GetComponent<MeshRenderer>().material.color = item.couleur;

        // Trouver le joueur lorsque l'item spawn;
        joueur = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Lorsqu'on est en collision avec le joueur
        if (collision.gameObject.tag == "Player")
        {
            //Selon la potion que nous sommes
            switch (nom)
            {
                case "potionVie":
                    //Heal le joueur de la valeur associée, donner un bonus selon l'etage
                    collision.gameObject.GetComponent<ComportementJoueur>().AugmenterVie(valeur * (1 + SpawnEnemy.etageActuel / 5));
                    break;

                case "potionVitesse":
                    //todo: ajouter fonction pour speedBoost dans ComportementJoueur
                    StartCoroutine(collision.gameObject.GetComponent<ComportementJoueur>().AugmenterVitesse(valeur, duree));
                    break;

                case "potionDegats":
                    //todo: ajouter fonction DegatsBoost() dans ComportementJoueur
                    StartCoroutine(collision.gameObject.GetComponent<ComportementJoueur>().AugmenterDegats(valeur, duree));
                    break;

                case "potionJump":
                    //todo: ajouter fonction JumpBoost() dans ComportementJoueur
                    StartCoroutine(collision.gameObject.GetComponent<ComportementJoueur>().AugmenterSaut(valeur, duree));
                    break;

                case "potionInvulnerable":
                    //todo: ajouter fonction Invulnerable() dans ComportementJoueur
                    StartCoroutine(collision.gameObject.GetComponent<ComportementJoueur>().Invulnerabilite(duree));
                    break;

                case "nuke":
                    //todo: ajouter fonction Nuke(), eleminer tous les ennemis a lentour du joueur
                    break;

                case "slime":
                    //Grossir le joueur
                    collision.gameObject.GetComponent<ComportementJoueur>().AugmenterGrosseur(valeur);

                    //Ajouter de l'xp au joueur
                    collision.gameObject.GetComponent<ComportementJoueur>().AugmenterXp(1);

                    break;
            }

            //Detruire l'item
            DestroyItem(item.sonItem);

        }
    }

    private void DestroyItem(AudioClip audioClip)
    {
        if(audioClip != null)
        {
            joueur.GetComponent<AudioSource>().PlayOneShot(audioClip);
        }
        Destroy(gameObject);
    }
}
