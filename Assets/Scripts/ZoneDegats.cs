using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZoneDegats : MonoBehaviour
{
    public GameObject joueur; //Reference au joueur
    public LayerMask layerSol; //Layer du sol
    public float distance; //Distance actuelle avec le sol
    public float plusGrandeDistance; //Plus grande distance
    public float rayonActuel; //Rayon actuel de la zone

    void Update()
    {
        //Changer la position du cercle selon le joueur
        transform.position = new Vector3(joueur.transform.position.x, transform.position.y, joueur.transform.position.z);

        //Raycast avec le sol
        RaycastHit hit;
        // Does the ray intersect any objects on the ground
        if (Physics.Raycast(joueur.transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerSol))
        {
            //Mettre a jour la distance
            distance = hit.distance;

            //Mettre a jour la position de la zone
            gameObject.transform.position = new Vector3(transform.position.x, hit.transform.position.y + 0.55f, transform.position.z);

            //Mettre a jour la lus grande valeur
            if (plusGrandeDistance < hit.distance)
            {
                plusGrandeDistance = hit.distance;
            }

            //Mettre a jour la taille de la zone
            rayonActuel = plusGrandeDistance + joueur.transform.localScale.x;
            transform.localScale = new Vector3(rayonActuel, transform.localScale.y, rayonActuel);
        }
    }
}
