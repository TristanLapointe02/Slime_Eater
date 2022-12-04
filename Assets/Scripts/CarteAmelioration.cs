using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CarteAmelioration : MonoBehaviour
{
    public Amelioration amelioration; //Amelioration en soi
    public TextMeshProUGUI titre; //Nom de l'amelioration
    public TextMeshProUGUI description; //Texte de description de l'amelioration
    public Image iconeAmelioration; //Icone d'amelioration
    public GameObject joueur; //Reference au joueur

    // Start is called before the first frame update
    void Start()
    {
        //Assigner les valeurs
        titre.text = amelioration.nom;
        description.text = amelioration.description1 + amelioration.valeur + amelioration.description2;
        iconeAmelioration.sprite = amelioration.icone;
    }

    //Fonction qui enlève les améliorations
    public void FermerAmelioration()
    {
        //Appeler la fonction d'amélioration sur le joueur
        joueur.GetComponent<ControleAmeliorations>().AjoutAmelioration(amelioration.nom, amelioration.valeur);
    }
}
