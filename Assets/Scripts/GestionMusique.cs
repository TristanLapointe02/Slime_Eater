using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionMusique : MonoBehaviour
{
    //Garder l'objet musique lorsqu'on change de sc�ne
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
