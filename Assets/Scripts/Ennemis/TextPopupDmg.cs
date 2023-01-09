using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Gestion de la disparition du texte de dégâts des ennemis
 * Fait par : Tristan Lapointe
 */
public class TextPopupDmg : MonoBehaviour
{
    void Update()
    {
        //Si nous sommes transparent
        if(GetComponent<CanvasGroup>().alpha <= 0)
        {
            //Se détruire
            Destroy(gameObject);
        }
    }
}
