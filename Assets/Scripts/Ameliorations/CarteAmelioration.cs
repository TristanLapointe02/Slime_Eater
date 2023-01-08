using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/* 
 * Description : Afficher l'information sur les cartes d'amélioration
 * Fait par : Tristan Lapointe
 */

public class CarteAmelioration : MonoBehaviour
{
    public Amelioration amelioration; //Amélioration en soi
    public TextMeshProUGUI titre; //Nom de l'amélioration
    public TextMeshProUGUI description; //Texte de description de l'amélioration
    public Image iconeAmelioration; //Icône d'amélioration
    [HideInInspector] public bool pigerDouble; //Variable indiquant si on peut piger en double

    //Variables pour correctement afficher le texte
    private string texteValeur1; //Texte de valeur 1
    private string texteValeur2; //Texte de valeur 2

    //Fonction qui enlève les améliorations
    public void FermerAmelioration()
    {
        //Appeler la fonction d'amélioration sur le joueur
        SpawnJoueur.joueur.GetComponent<ControleAmeliorations>().AjoutAmelioration(amelioration.nom, amelioration.valeur, amelioration.valeur2);

        //Si on pouvait appeler en double, le faire
        if (pigerDouble)
        {
            //Indiquer qu'on ne peut plus
            pigerDouble = false;

            //Reappeler la fonction
            SpawnJoueur.joueur.GetComponent<ControleAmeliorations>().AjoutAmelioration(amelioration.nom, amelioration.valeur, amelioration.valeur2);
        }
    }

    //Fonction permettant d'assigner des valeurs à la carte
    public void AssignerValeurs(Amelioration ameliorationAccordee)
    {
        //Indiquer l'amélioration qu'on vient de choisir
        amelioration = ameliorationAccordee;

        //Assigner les valeurs
        titre.text = ameliorationAccordee.nom;

        //Refresh les strings de texte
        texteValeur1 = "";
        texteValeur2 = "";

        //Ajuster le texte selon les valeurs
        if (amelioration.valeur > 0)
        {
            texteValeur1 = amelioration.valeur.ToString();
        }
        if (amelioration.valeur2 > 0)
        {
            texteValeur2 = amelioration.valeur2.ToString();
        }

        //Changer le texte
        description.text = amelioration.description1 + texteValeur1 + amelioration.description2 + texteValeur2 + amelioration.description3;

        //Changer l'icone
        iconeAmelioration.sprite = amelioration.icone;
    }
}
