using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * G�rer la musique du jeu
 * Fait par : Samuel S�guin
 */

public class GestionMusique : MonoBehaviour
{
    void Awake()
    {
        //Garder l'objet musique activ� lorsqu'on change de sc�ne
        DontDestroyOnLoad(this.gameObject);
    }
}
