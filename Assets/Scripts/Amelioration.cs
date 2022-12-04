using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Amelioration", fileName = "Amelioration")]
public class Amelioration : ScriptableObject
{
    public string nom; //Nom de l'am�lioration
    public Sprite icone; //Icone UI de l'amelioration
    public string description1; //Debut de la description de l'am�lioration
    public float valeur; //Valeur associ�e � l'am�lioration
    public string description2; //Fin de la description de l'am�lioration
}
