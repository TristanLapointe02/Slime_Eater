using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZoneDegats : MonoBehaviour
{
    public GameObject joueur; //Reference au joueur
    public LayerMask layerSol; //Layer du sol
    public float plusGrandeDistance; //Plus grande distance


    //DEBUG
    public TextMeshProUGUI texteDistance; //Distance entre joueur et sol
    public TextMeshProUGUI texteDistanceMax; //Distance max entre joueur et sol

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Changer la position du cercle selon le joueur
        transform.position = new Vector3(joueur.transform.position.x, transform.position.y, joueur.transform.position.z);

        //Raycast avec le sol
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(joueur.transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerSol))
        {
            Debug.DrawRay(joueur.transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);

            if(plusGrandeDistance < hit.distance)
            {
                plusGrandeDistance = hit.distance;
            }

            //Mettre a jour la taille de la zone
            transform.localScale = new Vector3(plusGrandeDistance, transform.localScale.y, plusGrandeDistance);

            //DEBUG
            texteDistance.text = "Distance:" + Mathf.Round(hit.distance - 0.5f).ToString();
            texteDistanceMax.text = "Max:" + Mathf.Round(plusGrandeDistance - 0.5f).ToString();
        }
    }
}
