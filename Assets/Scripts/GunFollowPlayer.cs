using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Faire que l'objet gun se déplace correctement selon la position du joueur
 * Fait par : Tristan Lapointe
 */

public class GunFollowPlayer : MonoBehaviour
{
    public GameObject player; //Référence au joueur

    // Update is called once per frame
    void Update()
    {
        //Si le joueur existe dans la scène
        if(player!= null)
        {
            //Suivre la position du joueur, mais toujours se placer à ses pieds, peu importe sa taille
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y - player.GetComponent<Collider>().bounds.size.y / 2.25f , player.transform.position.z);
        }

        //Changer la rotation du gun selon la position de la souris
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Sol")))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
