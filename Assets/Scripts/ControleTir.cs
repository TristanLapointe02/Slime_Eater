using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleTir : MonoBehaviour
{
    [Header("Balle")]
    public Balle balle; //Balle tiree

    [Header("Valeurs de tir")]
    bool peutTirer = true; //Bool permettant de savoir si le joueur peut tirer / cooldown
    public float shootCooldown; //Delai de tir entre les balles
    public float shootDelay; //Delai entre balles multiples
    public int nombreBalles; //Nombre de balles a tirer
    public float forceTir; //Force du tir
    public float valeurPerteTir; //Scale que le joueur perd en tirant
    public float diviseurGrosseurBalle; //Multiplicateur de grosseur de la balle selon le joueur
    public float degatsJoueur; //Degats du joueur
    public bool peutTirerArriere; //Indique si on peut tirer sur les côtés
    public bool peutTirerCotes; //Indique si on peut tirer sur les côtés
    public bool peutTirerATravers; //Indique si la balle peut passer a travers
    public bool peutExploser; //Indiquer si les balles peuvent exploser
    public bool peutSlow; //Indiquer si les balles slow les ennemis

    [Header("Autres références")]
    public AudioClip sonTir; //Son de tir
    public AudioClip sonTirImpossible; //Son indiquant qu'on ne peut pas tirer
    public GameObject gun; //Position du "gun"
    bool tropPetit; //Variable indiquant que nous somme trop petit ou non

    void Update()
    {
        //Si le joueur appuie sur clicque gauche
        if (Input.GetButton("Fire1") && peutTirer && ControleAmeliorations.pause == false)
        {
            //Si on est pas trop petit
            if(tropPetit == false)
            {
                //Indiquer qu'il ne peut plus tirer, et appeler une fonction
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
                    //Jouer un son indiquant au joueur qu'il ne peut pas tirer
                    GetComponent<AudioSource>().PlayOneShot(sonTirImpossible);
                }
            }
        }

        //Si nous sommes trop petit, empêcher la possibilite de tirer
        if(gameObject.transform.localScale.magnitude <= 1)
        {
            //Indiquer qu'il est trop petit
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
        for (int i = 0; i < nombreBalles; i++)
        {
            //Jouer un son de tir
            GetComponent<AudioSource>().PlayOneShot(sonTir);

            //Instancier une balle
            Balle nouvelleBalle = Instantiate(balle, gun.transform.position, gun.transform.rotation);

            //Changer la taille de la balle selon la grosseur du joueur
            nouvelleBalle.gameObject.transform.localScale += new Vector3(transform.localScale.x / diviseurGrosseurBalle, transform.localScale.y / diviseurGrosseurBalle, transform.localScale.z / diviseurGrosseurBalle);

            //Propulser la balle vers la direction du curseur
            nouvelleBalle.GetComponent<Rigidbody>().AddForce(direction * forceTir, ForceMode.Impulse);

            //Changer les degats de la balle selon ceux du joueur
            nouvelleBalle.GetComponent<Balle>().degats = degatsJoueur;

            //Dire a la balle qu'elle peut passer a travers ou non
            if (peutTirerATravers)
            {
                nouvelleBalle.GetComponent<Balle>().goThrough = true;
            }

            //Dire a la balle qu'elle peut exploser au contact
            if (peutExploser)
            {
                nouvelleBalle.GetComponent<Balle>().explose = true;
            }

            //Dire a la balle qu'elle peut slow au contact
            if (peutSlow)
            {
                nouvelleBalle.GetComponent<Balle>().slow = true;
            }

            //Diminuer la taille du joueur
            gameObject.transform.localScale -= new Vector3(valeurPerteTir, valeurPerteTir, valeurPerteTir);

            //Delai pour la prochaine balle
            yield return new WaitForSeconds(delai);
        }
    }

    //Fonction de cooldown du tir
    public IEnumerator delaiTir(float delai)
    {
        yield return new WaitForSeconds(delai);
        peutTirer = true;
    }
}
