using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Faire que l'objet gun se déplace et tourne correctement selon la position du joueur et de la souris
 * Fait par : Tristan Lapointe
 */

public class GunFollowPlayer : MonoBehaviour
{
    private GameObject joueur; //Référence au joueur

    private void Start()
    {
        joueur = SpawnJoueur.joueur;
    }

    void Update()
    {
        //Si le joueur existe dans la scène
        if(joueur != null)
        {
            //Suivre la position du joueur, mais toujours se placer à ses pieds, peu importe sa taille
            transform.position = new Vector3(joueur.transform.position.x, joueur.transform.position.y - joueur.GetComponent<Collider>().bounds.size.y / 2.25f , joueur.transform.position.z);
        }

        //Changer la rotation du gun selon la position de la souris
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Si nous touchons le sol
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Sol")))
        {
            //Regarder vers cette direction
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
