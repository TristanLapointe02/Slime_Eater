using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Amelioration", fileName = "Amelioration")]
public class Amelioration : ScriptableObject
{
    public string nom; //Nom de l'amélioration
    public Sprite icone; //Icone UI de l'amelioration
    public string description1; //Debut de la description de l'amélioration
    public float valeur; //Valeur associée à l'amélioration
    public string description2; //Fin de la description de l'amélioration
    public float valeur2; //Valeur 2, s'il y a lieu
    public string description3; //Description 3, s'il y a lieu
    public bool peutRepiger; //Determine si on peut repiger cette amélioration
}
