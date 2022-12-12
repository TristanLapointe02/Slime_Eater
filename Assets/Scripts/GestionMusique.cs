using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionMusique : MonoBehaviour
{
    //Garder l'objet musique lorsqu'on change de scène
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
