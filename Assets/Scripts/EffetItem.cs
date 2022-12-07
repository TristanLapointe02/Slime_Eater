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
    public GameObject effetUI; //UI de l'effet
    public GameObject parentEffetUI; //Parent de la liste d'effets
    public GameObject objetExplosion; //Objet visuel de l'explosion
    public AudioClip sonExplosion; //Son de l'explosion

    void Awake()
    {
        // Assigner valeurs du scriptableObject
        nom = item.name;
        valeur = item.valeur;
        duree = item.duree;
        player = GameObject.FindGameObjectWithTag("Player");
        parentEffetUI = GameObject.Find("ListeEffets");
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
                    joueur.AugmenterVie(valeur * (1 + StageProgression.etageActuel / 5));
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
                    Explosion();
                    break;

                case "slime":
                    //Grossir le joueur
                    joueur.AugmenterGrosseur(valeur);

                    //Ajouter de l'xp au joueur
                    joueur.AugmenterXp(1);

                    break;
            }

            //Creer un UI pour l'effet
            CreerEffetUI();

            //Detruire l'item
            DestroyItem(item.sonItem);
        }
    }

    //Fonction d'explosion
    public void Explosion()
    {
        //Spawn un objet visuel
        GameObject effet = Instantiate(objetExplosion, gameObject.transform.position, Quaternion.identity);

        //Changer la grosseur de l'effet selon notre rayon
        effet.transform.localScale = new Vector3(valeur + player.transform.localScale.magnitude, valeur + player.transform.localScale.magnitude, valeur + player.transform.localScale.magnitude);

        //Le detruire tout de suite après
        Destroy(effet, 0.15f);

        //Jouer un sound effect
        AudioSource.PlayClipAtPoint(sonExplosion, gameObject.transform.position);

        //Pour tous les colliders dans la zone d'explosion
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, valeur + player.transform.localScale.magnitude);

        //Pour tous les collider touchés
        foreach (var collider in hitColliders)
        {
            //Trouver les ennemis
            if (collider.gameObject.TryGetComponent(out EnemyController ennemy) && objetExplosion != null)
            {
                //Leur faire des degats
                ennemy.TakeDamage(valeur * 1000);

                //Faire une explosion
                ennemy.GetComponent<Rigidbody>().AddExplosionForce(3500, transform.position, valeur + player.transform.localScale.magnitude);
            }
        }
    }

    //Fonction permettant d'ajouter un element UI indiquant l'effet
    public void CreerEffetUI()
    {
        //Si l'effet a une duree
        if(item.duree > 0)
        {
            //Faire apparaitre un element UI indiquant l'effet ajouté
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

        //Detruire l'item
        Destroy(gameObject);
    }
}
