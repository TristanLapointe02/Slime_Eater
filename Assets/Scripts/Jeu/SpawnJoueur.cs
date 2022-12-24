using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Permet de spawn le joueur au début de la partie
 * Fait par : Tristan Lapointe
 */

public class SpawnJoueur : MonoBehaviour
{
    public GameObject prefabJoueur; //Référence au prefab du joueur
    public static GameObject joueur; //Référence globale au joueur

    private void Awake()
    {
        //Spawn le joueur au milieu de la map
        GameObject nouveauJoueur = Instantiate(prefabJoueur.gameObject, transform.position, Quaternion.identity);

        //Pour chaque sous objet dans le joueur
        foreach (Transform obj in nouveauJoueur.transform)
        {
            //Si nous trouvons le tag joueur
            if (obj.tag == "Player")
            {
                //Assigner la référence au joueur
                joueur = obj.gameObject;
            }
        }
    }
}
