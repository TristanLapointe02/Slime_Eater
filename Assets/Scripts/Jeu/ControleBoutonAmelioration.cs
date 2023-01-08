using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ControleBoutonAmelioration : MonoBehaviour
{
    public BoutonAmelioration bouton; //Bouton choisi
    private TextMeshProUGUI texteBouton; //Texte du bouton
    private int charges; //Nombre de charges du bouton
    private GameObject joueur; //R�f�rence au joueur
    public TextMeshProUGUI texteLevelUp; //Texte de level up

    void Start()
    {
        //Assigner les r�f�rences
        TrouverReferences();

        //Ajouter la fonction de modification au bouton
        GetComponent<Button>().onClick.AddListener(AppliquerModification);

        //Ajouter une charge de d�part
        AjouterCharge();

        //Changer la couleur du bouton
        GetComponent<Button>().image.color = bouton.couleurBouton;
    }

    //Fonction permettant d'appliquer une modification selon le bouton
    public void AppliquerModification()
    {
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

                //Pour tous les autres boutons
                foreach(Transform child in transform.parent)
                {
                    //Emp�cher d'interagir avec les autres boutons
                    child.GetComponent<Button>().interactable = false;

                    //Disable leur event trigger
                    child.GetComponent<EventTrigger>().enabled = false;
                }
                break;
        }

        //Diminuer la charge
        charges--;

        //Mettre � jour le texte
        texteBouton.text = charges.ToString();

        //Si nous sommes � 0 charges
        if (charges == 0)
        {
            //Disable le bouton
            GetComponent<Button>().interactable = false;
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
    }

    //Fonction permettant de verifier si nous pouvons ajouter une charge
    public void VerifierCharge()
    {
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
            //Indiquer que le bouton est int�ractif
            GetComponent<Button>().interactable = true;

            //Enable le event hover
            GetComponent<EventTrigger>().enabled = true;
        }
    }

    //Fonction permettant de trouver les r�f�rences
    public void TrouverReferences()
    {
        //Assigner la r�f�rence au joueur
        joueur = SpawnJoueur.joueur;
        texteBouton = GetComponentInChildren<TextMeshProUGUI>();
    }
}
