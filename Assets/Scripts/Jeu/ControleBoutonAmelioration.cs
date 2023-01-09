using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*
 * Gestion des boutons de modification d'am�liorations
 * Fait par : Tristan Lapointe
 */

public class ControleBoutonAmelioration : MonoBehaviour
{
    public BoutonAmelioration bouton; //Bouton choisi
    private TextMeshProUGUI texteBouton; //Texte du bouton
    private int charges; //Nombre de charges du bouton
    private GameObject joueur; //R�f�rence au joueur
    public TextMeshProUGUI texteLevelUp; //Texte de level up
    public GameObject ecranDegrade; //Ecran de degrad� pour choix de bouton
    public Transform parentEffetDegrade; //Parent du degrade

    void Start()
    {
        //Assigner les r�f�rences
        TrouverReferences();

        //Ajouter la fonction de modification au bouton
        GetComponent<Button>().onClick.AddListener(AppliquerModification);

        //Mettre � jour le texte au d�part
        texteBouton.text = charges.ToString();

        //Se disable au d�part
        ChangerEtat(false);

        //Si on est le bouton pour rafraichir, ajouter une charge de d�part
        if(bouton.boutonChoisi == BoutonAmelioration.TypesBoutons.Rafraichir)
        {
            AjouterCharge();
        }
    }

    //Fonction permettant d'appliquer une modification selon le bouton
    public void AppliquerModification()
    {
        //Jouer un son de s�lection
        joueur.GetComponent<AudioSource>().PlayOneShot(bouton.effetSonore);

        //Selon le type de bouton
        switch (bouton.boutonChoisi)
        {
            case BoutonAmelioration.TypesBoutons.Rafraichir:
                //Rafraichir les choix
                joueur.GetComponent<ControleAmeliorations>().RafraichirOptions();
                break;

            case BoutonAmelioration.TypesBoutons.ToutPrendre:
                //Tous prendre les choix
                joueur.GetComponent<ControleAmeliorations>().ToutChoisir();
                break;

            case BoutonAmelioration.TypesBoutons.PrendreDouble:
                //Prendre en double un choix
                joueur.GetComponent<ControleAmeliorations>().ChoisirDouble();

                //Pour tous les boutons
                foreach(Transform child in transform.parent)
                {
                    //Changer l'etat du bouton
                    child.GetComponent<ControleBoutonAmelioration>().ChangerEtat(false);
                }
                break;
        }

        //Afficher une couleur de d�grad� � l'�cran
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AjoutEcranEffet(bouton.couleurBouton, bouton.dureeDegrade));
        }

        //Diminuer la charge
        charges--;

        //Mettre � jour le texte
        texteBouton.text = charges.ToString();

        //Si nous sommes � 0 charges
        if (charges == 0)
        {
            //Changer l'etat du bouton
            ChangerEtat(false);
        }
    }

    //Fonction permettant de changer le texte de s�lection lors du hover
    public void ChangerTexte()
    {
        //Changer le texte si on a au moins une charge
        if(charges >= 1)
        {
            //Changer le texte
            texteLevelUp.text = bouton.texteDescription;

            //Changer la couleur du texte
            texteLevelUp.color = bouton.couleurBouton;

            //Afficher une couleur de d�grad� � l'�cran
            StartCoroutine(AjoutEcranEffet(bouton.couleurBouton, 1f));
        } 
    }

    //Fonction permettant de remettre le texte de base
    public void RemettreTexteBase()
    {
        //Remettre le texte de base
        texteLevelUp.text = joueur.GetComponent<ComportementJoueur>().TexteBaseLevel;

        //Remettre la couleur au blanc
        texteLevelUp.color = Color.white;
    }

    //Fonction permettant de rajouter une charge au bouton
    public void AjouterCharge()
    {
        //Ajouter une charge
        charges++;

        //Mettre � jour le texte
        texteBouton.text = charges.ToString();

        //Changer l'etat du bouton
        ChangerEtat(true);

    }

    //Fonction permettant de verifier si nous pouvons ajouter une charge
    public void VerifierCharge()
    {
        //Clearer tous les effets du d�grad�
        foreach(Transform degrade in parentEffetDegrade.transform)
        {
            Destroy(degrade.gameObject);
        }

        //Si nous savons pas c'est qui le joueur, le chercher
        if(joueur == null)
        {
            TrouverReferences();
        }

        //Si le niveau actuel est un multiple du nombre de niveaux de recharge du bouton
        if (joueur.GetComponent<ComportementJoueur>().levelActuel % bouton.nbNiveauxRecharge == 0)
        {
            //Ajouter une charge
            AjouterCharge();
        }

        //Enable les boutons, si possible
        if(charges > 0)
        {
            //Changer l'etat du bouton
            ChangerEtat(true);
        }
    }

    //Fonction permettant de trouver les r�f�rences
    public void TrouverReferences()
    {
        //Assigner la r�f�rence au joueur
        joueur = SpawnJoueur.joueur;
        texteBouton = GetComponentInChildren<TextMeshProUGUI>();
    }

    //Fonction permettant d'instancier l'effet de couleur UI
    public IEnumerator AjoutEcranEffet(Color couleurEffet, float duree, float valeurAlpha = 1)
    {
        //Instancier l'�l�ment de UI
        GameObject nouvelEcranEffetUI = Instantiate(ecranDegrade);
        nouvelEcranEffetUI.transform.SetParent(parentEffetDegrade, false);

        //Lui donner une couleur
        nouvelEcranEffetUI.GetComponent<Image>().color = new Color(couleurEffet.r, couleurEffet.g, couleurEffet.b, valeurAlpha);

        //Storer la variable de l'image
        Color couleurEcran = nouvelEcranEffetUI.GetComponent<Image>().color;

        //Diminuer l'opacit� avec le temps
        for (float i = 0; i < 1; i += Time.deltaTime / duree)
        {
            //Changer l'opacit� de la couleur
            var nouvelleCouleur = new Color(couleurEcran.r, couleurEcran.g, couleurEcran.b, Mathf.Lerp(couleurEcran.a, 0, i));
            nouvelEcranEffetUI.GetComponent<Image>().color = nouvelleCouleur;

            yield return null;
        }

        //Enlever l'�l�ment de UI
        Destroy(nouvelEcranEffetUI);
    }

    //Fonction permettant de changer l'etat du bouton
    public void ChangerEtat(bool etat)
    {
        //Interactivite du bouton
        GetComponent<Button>().interactable = etat;

        //Event hover
        GetComponent<EventTrigger>().enabled = etat;
    }
}
