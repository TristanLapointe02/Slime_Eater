using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item", fileName = "Item")]

public class StatsItems : ScriptableObject
{
    public string nomItem; //Nom de l'item
    public float duree; //duree de l'effet de l'item
    public float valeur; //valeur de l'effet
    public int etage; //Premier étage auquel item se drop
    public AudioClip sonItem; //Son qui joue quand on pick up l'item
    public Sprite icone; //Icone de l'effet
}
