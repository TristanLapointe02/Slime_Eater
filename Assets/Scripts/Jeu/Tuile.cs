using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuile : MonoBehaviour
{
    [HideInInspector] public Transform joueur; //Reference au joueur
    [HideInInspector] public float rayonVerif; //Rayon de verification avec le joueur
    [HideInInspector] public GestionSpawnPlancherV3 spawner; //Objet qui a spawn cette tuile

    // Update is called once per frame
    void Update()
    {
        //Si on est trop loin du joueur
        if ((joueur.position - gameObject.transform.position).magnitude > rayonVerif)
        {
            //S'enlever de la liste de son spawner
            spawner.plancherActuel.Remove(gameObject);

            //Se détruire
            Destroy(gameObject);
        }
    }
}
