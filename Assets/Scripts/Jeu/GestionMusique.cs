using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Gérer la musique du jeu
 * Fait par : Samuel Séguin
 */

public class GestionMusique : MonoBehaviour
{
    void Awake()
    {
        //Garder l'objet musique activé lorsqu'on change de scène
        DontDestroyOnLoad(this.gameObject);
    }
}
