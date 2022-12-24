using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/* 
 * Description : Afficher l'information sur les cartes d'am�lioration
 * Fait par : Tristan Lapointe
 */

public class CarteAmelioration : MonoBehaviour
{
    public Amelioration amelioration; //Am�lioration en soi
    public TextMeshProUGUI titre; //Nom de l'am�lioration
    public TextMeshProUGUI description; //Texte de description de l'am�lioration
    public Image iconeAmelioration; //Ic�ne d'am�lioration

    //Variables pour correctement afficher le texte
    private string texteValeur1; //Texte de valeur 1
    private string texteValeur2; //Texte de valeur 2

    // Start is called before the first frame update
    void Start()
    {
        //Assigner les valeurs
        titre.text = amelioration.nom;
        
        //Ajuster le texte selon les valeurs
        if(amelioration.valeur > 0)
        {
            texteValeur1 = amelioration.valeur.ToString();
        }
        if(amelioration.valeur2 > 0)
        {
            texteValeur2 = amelioration.valeur2.ToString();
        }

        //Changer le texte
        description.text = amelioration.description1 + texteValeur1 + amelioration.description2 + texteValeur2 + amelioration.description3;

        //Changer l'icone
        iconeAmelioration.sprite = amelioration.icone;
    }

    //Fonction qui enl�ve les am�liorations
    public void FermerAmelioration()
    {
        //Appeler la fonction d'am�lioration sur le joueur
        SpawnJoueur.joueur.GetComponent<ControleAmeliorations>().AjoutAmelioration(amelioration.nom, amelioration.valeur, amelioration.valeur2);
    }
}
