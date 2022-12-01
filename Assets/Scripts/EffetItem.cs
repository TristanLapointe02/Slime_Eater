using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffetItem : MonoBehaviour
{
    public StatsItems item; // type d'item
    public GameObject player; // Référence au joueur
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
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Lorsqu'on est en collision avec le joueur
        if (collision.tag == "Player")
        {
            ComportementJoueur joueur = collision.gameObject.GetComponent<ComportementJoueur>();
            //Selon la potion que nous sommes
            switch (nom)
            {
                case "potionVie":
                    joueur.AugmenterVie(valeur * (1 + SpawnEnemy.etageActuel / 5));
                    break;

                case "potionVitesse":
                    //todo: ajouter fonction pour speedBoost dans ComportementJoueur
                    joueur.StartCoroutine(joueur.AugmenterVitesse(valeur, duree));
                    break;

                case "potionDegats":
                    //todo: ajouter fonction DegatsBoost() dans ComportementJoueur
                    joueur.StartCoroutine(joueur.AugmenterDegats(valeur, duree));
                    break;

                case "potionJump":
                    //todo: ajouter fonction JumpBoost() dans ComportementJoueur
                    joueur.StartCoroutine(joueur.AugmenterSaut(valeur, duree));
                    break;

                case "potionInvulnerable":
                    //todo: ajouter fonction Invulnerable() dans ComportementJoueur
                    joueur.StartCoroutine(joueur.Invulnerabilite(duree));
                    break;

                case "nuke":
                    //todo: ajouter fonction Nuke(), eleminer tous les ennemis autour du joueur
                    GameObject zoneDegats = GameObject.Find("ZoneDegats");
                    zoneDegats.GetComponent<ZoneDegats>().rayonActuel = 100f;
                    player.GetComponent<ControleJoueur>().Explosion(valeur, 100f);
                    break;

                case "slime":
                    //Grossir le joueur
                    joueur.AugmenterGrosseur(valeur);

                    //Ajouter de l'xp au joueur
                    joueur.AugmenterXp(1);

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
            player.GetComponent<AudioSource>().PlayOneShot(audioClip);
        }
        Destroy(gameObject);
    }
}
