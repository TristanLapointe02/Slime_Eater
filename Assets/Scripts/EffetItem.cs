using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Gérer les effets des différents items
 * Fait par : Samuel Séguin et Tristan Lapointe
 */

public class EffetItem : MonoBehaviour
{
    public StatsItems item; //Type d'item
    public GameObject player; //Référence au joueur
    public string nom; //Nom de l'item
    public float valeur; //Valeur de l'item
    public float duree; //Durée de l'effet
    public GameObject effetUI; //UI de l'effet
    public GameObject parentEffetUI; //Parent de la liste d'effets
    

    void Awake()
    {
        //Assigner les valeurs du scriptableObject à l'item
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
            //Assigner la référence au joueur
            ComportementJoueur joueur = collision.gameObject.GetComponent<ComportementJoueur>();

            //Disable notre collider
            GetComponent<Collider>().enabled = false;

            //Selon le type d'item : 
            switch (nom)
            {
                //Pour la potion de vie
                case "potionVie":
                    //Augmenter la vie du joueur selon l'étage actuel
                    joueur.AugmenterVie(valeur * (1 + StageProgression.etageActuel / 5));
                    break;

                //Pour la potion de vitesse
                case "potionVitesse":
                    //Augmenter temporairement la vitesse du joueur
                    joueur.StartCoroutine(joueur.AugmenterVitesse(valeur, duree));
                    break;

                //Pour la potion de dégâts
                case "potionDegats": 
                    //Augmenter temporairement les dégâts du joueur
                    joueur.StartCoroutine(joueur.AugmenterDegats(valeur, duree));
                    break;

                //Pour la potion de saut
                case "potionJump":
                    //Augmenter temporairement la hauteur de saut du joueur
                    joueur.StartCoroutine(joueur.AugmenterSaut(valeur, duree));
                    break;

                //Pour la potion d'invulnérabilité
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

            //Créer un UI pour l'effet
            CreerEffetUI();

            //Détruire l'item
            DestroyItem(item.sonItem);
        }
    }

    //Fonction permettant d'ajouter un élément UI indiquant l'effet
    public void CreerEffetUI()
    {
        //Si l'effet a une durée
        if(item.duree > 0)
        {
            //Faire apparaître un élément UI indiquant l'effet ajouté
            GameObject nouvelEffetUI = Instantiate(effetUI);
            nouvelEffetUI.transform.SetParent(parentEffetUI.transform, false);

            //Assigner les valeurs
            nouvelEffetUI.GetComponent<EffetItemUI>().texteNomEffet.text = item.nomItem;
            nouvelEffetUI.GetComponent<EffetItemUI>().temps = duree;
            nouvelEffetUI.GetComponent<EffetItemUI>().imageEffet.sprite = item.icone;
        }
    }

    //Fonction qui détruit l'item
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
