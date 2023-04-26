using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Gestion du tir du joueur
 * Fait par : Tristan Lapointe et Samuel Séguin
 */

public class ControleTir : MonoBehaviour
{
    [Header("Balle")]
    public Balle balle; //Balle tirée

    [Header("Valeurs de tir")]
    bool peutTirer = true; //Bool permettant de savoir si le joueur peut tirer
    public float shootCooldown; //Délai de tir entre les balles
    public float shootDelay; //Délai entre balles multiples
    public int nombreBalles; //Nombre de balles à tirer
    public float forceTir; //Force du tir
    public float valeurPerteTir; //Taille que le joueur perd en tirant
    public float diviseurGrosseurBalle; //Multiplicateur de taille de la balle selon le joueur
    public float degatsJoueur; //Dégâts infligés par joueur
    public bool peutTirerArriere; //Indique si on peut tirer en arrière
    public bool peutTirerCotes; //Indique si on peut tirer sur les côtés
    public bool peutTirerATravers; //Indique si la balle peut passer au travers des ennemis
    public bool peutExploser; //Indique si les balles peuvent exploser
    public bool peutSlow; //Indique si les balles ralentissent les ennemis

    [Header("Autres références")]
    public AudioClip sonTir; //Son de tir
    //public AudioClip[] sonsTir; //Tableau des sons de tir
    public AudioClip sonTirImpossible; //Son indiquant qu'on ne peut pas tirer
    public GameObject gun; //Position du "gun"
    bool tropPetit; //Variable indiquant que nous somme trop petit ou non

    void Update()
    {
        //Si le joueur appuie sur clique gauche
        if (Input.GetButton("Fire1") && peutTirer && ControleAmeliorations.pause == false && ControleMenu.pauseMenu == false && ComportementJoueur.finJeu == false)
        {
            //Si on a assez de masse pour tirer
            if(tropPetit == false)
            {
                //Afficher un UI nous permettant de voir le cooldown
                StartCoroutine(GetComponent<ControleJoueur>().cooldownAction(shootCooldown, GetComponent<ControleJoueur>().imageAttaque));

                //Indiquer qu'on ne peut plus tirer, et appeler une fonction
                peutTirer = false;
                StartCoroutine(delaiTir(shootCooldown));

                //Tirer une balle vers l'avant
                StartCoroutine(tirBalle(nombreBalles, shootDelay, gun.transform.forward));

                //Si on peut tirer une balle vers l'arrière, le faire
                if (peutTirerArriere)
                {
                    StartCoroutine(tirBalle(nombreBalles, shootDelay, -gun.transform.forward));
                }
                //Si on peut tirer une balle sur les côtés, le faire
                if (peutTirerCotes)
                {
                    StartCoroutine(tirBalle(nombreBalles, shootDelay, gun.transform.right));
                    StartCoroutine(tirBalle(nombreBalles, shootDelay, -gun.transform.right));
                }
            }
            //Sinon
            else
            {
                //Si on n'a pas déjà le son qui joue
                if(GetComponent<AudioSource>().isPlaying == false)
                {
                    //Jouer un son indiquant au joueur qu'il n'a pas assez de masse pour tirer
                    GetComponent<AudioSource>().PlayOneShot(sonTirImpossible);
                }
            }
        }

        //Si nous sommes trop petit, empêcher la possibilitée de tirer
        if(gameObject.transform.localScale.magnitude <= 1)
        {
            //Indiquer que nous sommes trop petit
            tropPetit = true;
        }
        else
        {
            tropPetit = false;
        }
    }

    //Fonction qui tire une balle
    public IEnumerator tirBalle(int nbBalles, float delai, Vector3 direction)
    {
        //Pour le nombre de balles a tirer
        for (int i = 0; i < nbBalles; i++)
        {
            //Jouer un son de tir
            GetComponent<AudioSource>().PlayOneShot(sonTir);

            //Instancier une balle
            Balle nouvelleBalle = Instantiate(balle, gun.transform.position, gun.transform.rotation);

            //Changer la taille de la balle selon la taille du joueur
            nouvelleBalle.gameObject.transform.localScale += new Vector3(transform.localScale.x / diviseurGrosseurBalle, transform.localScale.y / diviseurGrosseurBalle, transform.localScale.z / diviseurGrosseurBalle);

            //Propulser la balle vers la direction du curseur
            nouvelleBalle.GetComponent<Rigidbody>().AddForce(direction * forceTir, ForceMode.Impulse);

            //Changer les dégâts de la balle selon ceux du joueur
            nouvelleBalle.GetComponent<Balle>().degats = degatsJoueur;

            //Dire à la balle qu'elle peut passer au travers des ennemis ou non
            if (peutTirerATravers)
            {
                nouvelleBalle.GetComponent<Balle>().goThrough = true;
            }

            //Dire a la balle qu'elle peut exploser au contact ou non
            if (peutExploser)
            {
                nouvelleBalle.GetComponent<Balle>().explose = true;
            }

            //Dire a la balle qu'elle peut slow au contact ou non
            if (peutSlow)
            {
                nouvelleBalle.GetComponent<Balle>().slow = true;
            }

            //Diminuer la taille du joueur
            gameObject.transform.localScale -= new Vector3(valeurPerteTir, valeurPerteTir, valeurPerteTir);

            //Délai pour la prochaine balle
            yield return new WaitForSeconds(delai);
        }
    }

    //Fonction de cooldown du tir
    public IEnumerator delaiTir(float delai)
    {
        //Attendre un certain délai
        yield return new WaitForSeconds(delai);

        //Indiquer qu'on peut tirer à nouveau
        peutTirer = true;
    }
}
