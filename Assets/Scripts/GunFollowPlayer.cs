using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFollowPlayer : MonoBehaviour
{
    public GameObject player; //Reference au joueur
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Suivre la position du joueur, mais toujours se placer à ses pieds, peu importe sa taille
        if(player!= null)
        {
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
