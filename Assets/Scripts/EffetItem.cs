using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : G�rer les effets des diff�rents items
 * Fait par : Samuel S�guin et Tristan Lapointe
 */

public class EffetItem : MonoBehaviour
{
    public StatsItems item; //Type d'item
    public GameObject player; //R�f�rence au joueur
    public string nom; //Nom de l'item
    public float valeur; //Valeur de l'item
    public float duree; //Dur�e de l'effet
    public GameObject effetUI; //UI de l'effet
    public GameObject parentEffetUI; //Parent de la liste d'effets
    

    void Awake()
    {
        //Assigner les valeurs du scriptableObject � l'item
        nom = item.name;
        valeur = item.valeur;
        duree = item.duree;
        player = GameObject.FindGameObjectWithTag("Player");
        parentEffetUI = GameObject.Find("ListeEffets");
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Lorsque l'item entre en collision avec le joueur
        if (collision.tag == "Player")
        {
            //Assigner la r�f�rence au joueur
            ComportementJoueur joueur = collision.gameObject.GetComponent<ComportementJoueur>();

            //Disable notre collider
            GetComponent<Collider>().enabled = false;

            //Selon le type d'item : 
            switch (nom)
            {
                //Pour la potion de vie
                case "potionVie":
                    //Augmenter la vie du joueur selon l'�tage actuel
                    joueur.AugmenterVie(valeur * (1 + StageProgression.etageActuel / 5));
                    break;

                //Pour la potion de vitesse
                case "potionVitesse":
                    //Augmenter temporairement la vitesse du joueur
                    joueur.StartCoroutine(joueur.AugmenterVitesse(valeur, duree));
                    break;

                //Pour la potion de d�g�ts
                case "potionDegats": 
                    //Augmenter temporairement les d�g�ts du joueur
                    joueur.StartCoroutine(joueur.AugmenterDegats(valeur, duree));
                    break;

                //Pour la potion de saut
                case "potionJump":
                    //Augmenter temporairement la hauteur de saut du joueur
                    joueur.StartCoroutine(joueur.AugmenterSaut(valeur, duree));
                    break;

                //Pour la potion d'invuln�rabilit�
                case "potionInvulnerable":
                    //Rendre temporairement le joueur invincible
                    joueur.StartCoroutine(joueur.Invulnerabilite(duree));
                    break;

                //Pour la bombe
                case "nuke":
                    //Demarrer le compteur d'explosion
                    StartCoroutine(GetComponent<ExplosionBombe>().Explosion());
                    break;

                //Pour un morceau de slime
                case "slime":
                    //Grossir le joueur
                    joueur.AugmenterGrosseur(valeur);

                    //Ajouter de l'xp au joueur
                    joueur.AugmenterXp(1);
                    break;
            }

            //Cr�er un UI pour l'effet
            CreerEffetUI();

            //D�truire l'item
            DestroyItem(item.sonItem);
        }
    }

    //Fonction permettant d'ajouter un �l�ment UI indiquant l'effet
    public void CreerEffetUI()
    {
        //Si l'effet a une dur�e
        if(item.duree > 0)
        {
            //Faire appara�tre un �l�ment UI indiquant l'effet ajout�
            GameObject nouvelEffetUI = Instantiate(effetUI);
            nouvelEffetUI.transform.SetParent(parentEffetUI.transform, false);

            //Assigner les valeurs
            nouvelEffetUI.GetComponent<EffetItemUI>().texteNomEffet.text = item.nomItem;
            nouvelEffetUI.GetComponent<EffetItemUI>().temps = duree;
            nouvelEffetUI.GetComponent<EffetItemUI>().imageEffet.sprite = item.icone;
        }
    }

    //Fonction qui d�truit l'item
    private void DestroyItem(AudioClip audioClip)
    {
        //Si on peut jouer un son, le faire
        if (audioClip != null)
        {
            player.GetComponent<AudioSource>().PlayOneShot(audioClip);
        }

        //Detruire l'item si ce n'est pas la bombe
        if(item.nomItem != "Nuke")
        {
            Destroy(gameObject);
        }
    }
}
