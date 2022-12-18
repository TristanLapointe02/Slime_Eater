using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Scriptable object pour les ennemis du jeu
 * Fait par : Samuel Séguin et Tristan Lapointe
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Ennemi", fileName = "Ennemi")]

public class StatsEnemy : ScriptableObject
{
    public string nomEnnemi; //Nom de l'ennemi
    public string descriptionEnnemi; //Description de l’ennemi
    public float vieMax; //Vie maximum de l'ennemi
    public float degats; //Degats de l'ennemi
    public float vitesse; //Vitesse de l'ennemi
    public float tailleEnnemi; //Grosseur de l’ennemi
    public float valeurLoot; //Valeur de loot de l’ennemi
    public int nombreLootSpawn; //Nombre de loot a spawn de l’ennemi
    public bool ranged; //Determine si l’ennemi est ranged ou non
    public bool boss; //Determine si on est un boss ou non
    public int etage; //Etage sur laquelle le mob va spawn
    public int range; //Range, si l'ennemi attaque à distance
    public Color32 couleur; //Couleur de l'ennemi
}
