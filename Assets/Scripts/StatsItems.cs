using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Scriptable object pour les items du jeu
 * Fait par : Tristan Lapointe et Samuel Séguin
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Item", fileName = "Item")]

public class StatsItems : ScriptableObject
{
    public string nomItem; //Nom de l'item
    public float duree; //durée de l'effet de l'item
    public float valeur; //valeur de l'effet
    public int etage; //Premier étage auquel l.item se drop
    public AudioClip sonItem; //Son qui joue quand on ramasse l'item
    public Sprite icone; //Icône de l'effet
}
