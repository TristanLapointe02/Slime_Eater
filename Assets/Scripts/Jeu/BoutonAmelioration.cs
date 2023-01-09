using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Scriptable object des boutons de modification d'améliorations
 * Fait par : Tristan Lapointe
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Bouton", fileName = "Bouton")]
public class BoutonAmelioration : ScriptableObject
{
    public enum TypesBoutons { Rafraichir, ToutPrendre, PrendreDouble }; //Liste de types de boutons
    public TypesBoutons boutonChoisi; //Bouton choisi
    public string texteDescription; //Texte de description du bouton
    public Color32 couleurBouton; //Couleur d'effet du bouton
    public int nbNiveauxRecharge; //Nombre de niveaux à prendre pour avoir une autre recharge du bouton
    public float dureeDegrade; //Duree de l'effet de dégradé à l'écran
    public AudioClip effetSonore; //Effet sonore de sélection
}
