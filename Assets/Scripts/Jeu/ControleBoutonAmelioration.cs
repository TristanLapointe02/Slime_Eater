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
    private GameObject joueur; //Référence au joueur
    public TextMeshProUGUI texteLevelUp; //Texte de level up
    public GameObject ecranDegrade; //Ecran de degradé pour choix de bouton
    public Transform parentEffetDegrade; //Parent du degrade

    void Start()
    {
        //Assigner les références
        TrouverReferences();

        //Ajouter la fonction de modification au bouton
        GetComponent<Button>().onClick.AddListener(AppliquerModification);

        //Mettre à jour le texte au départ
        texteBouton.text = charges.ToString();

        //Pour tous les boutons
        foreach (Transform child in transform.parent)
        {
            //Empêcher d'interagir avec les autres boutons
            child.GetComponent<Button>().interactable = false;

            //Disable leur event trigger
            child.GetComponent<EventTrigger>().enabled = false;
        }
    }

    //Fonction permettant d'appliquer une modification selon le bouton
    public void AppliquerModification()
    {
        //Jouer un son de sélection
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
                    //Empêcher d'interagir avec les autres boutons
                    child.GetComponent<Button>().interactable = false;

                    //Disable leur event trigger
                    child.GetComponent<EventTrigger>().enabled = false;
                }
                break;
        }

        //Afficher une couleur de dégradé à l'écran
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AjoutEcranEffet(bouton.couleurBouton, bouton.dureeDegrade));
        }

        //Diminuer la charge
        charges--;

        //Mettre à jour le texte
        texteBouton.text = charges.ToString();

        //Si nous sommes à 0 charges
        if (charges == 0)
        {
            //Disable le bouton
            GetComponent<Button>().interactable = false;
        }
    }

    //Fonction permettant de changer le texte de sélection lors du hover
    public void ChangerTexte()
    {
        //Changer le texte si on a au moins une charge
        if(charges >= 1)
        {
            //Changer le texte
            texteLevelUp.text = bouton.texteDescription;

            //Changer la couleur du texte
            texteLevelUp.color = bouton.couleurBouton;

            //Afficher une couleur de dégradé à l'écran
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

        //Mettre à jour le texte
        texteBouton.text = charges.ToString();
    }

    //Fonction permettant de verifier si nous pouvons ajouter une charge
    public void VerifierCharge()
    {
        //Clearer tous les effets du dégradé
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
            //Indiquer que le bouton est intéractif
            GetComponent<Button>().interactable = true;

            //Enable le event hover
            GetComponent<EventTrigger>().enabled = true;
        }
    }

    //Fonction permettant de trouver les références
    public void TrouverReferences()
    {
        //Assigner la référence au joueur
        joueur = SpawnJoueur.joueur;
        texteBouton = GetComponentInChildren<TextMeshProUGUI>();
    }

    //Fonction permettant d'instancier l'effet de couleur UI
    public IEnumerator AjoutEcranEffet(Color couleurEffet, float duree, float valeurAlpha = 1)
    {
        //Instancier l'élément de UI
        GameObject nouvelEcranEffetUI = Instantiate(ecranDegrade);
        nouvelEcranEffetUI.transform.SetParent(parentEffetDegrade, false);

        //Lui donner une couleur
        nouvelEcranEffetUI.GetComponent<Image>().color = new Color(couleurEffet.r, couleurEffet.g, couleurEffet.b, valeurAlpha);

        //Storer la variable de l'image
        Color couleurEcran = nouvelEcranEffetUI.GetComponent<Image>().color;

        //Diminuer l'opacité avec le temps
        for (float i = 0; i < 1; i += Time.deltaTime / duree)
        {
            //Changer l'opacité de la couleur
            var nouvelleCouleur = new Color(couleurEcran.r, couleurEcran.g, couleurEcran.b, Mathf.Lerp(couleurEcran.a, 0, i));
            nouvelEcranEffetUI.GetComponent<Image>().color = nouvelleCouleur;

            yield return null;
        }

        //Enlever l'élément de UI
        Destroy(nouvelEcranEffetUI);
    }
}
