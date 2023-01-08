using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Bouton", fileName = "Bouton")]
public class BoutonAmelioration : ScriptableObject
{
    public enum TypesBoutons { Rafraichir, ToutPrendre, PrendreDouble }; //Liste de types de boutons
    public TypesBoutons boutonChoisi; //Bouton choisi
    public string texteDescription; //Texte de description du bouton
    public Color32 couleurBouton; //Couleur d'effet du bouton
    public int nbNiveauxRecharge; //Nombre de niveaux à prendre pour avoir une autre recharge du bouton
}
