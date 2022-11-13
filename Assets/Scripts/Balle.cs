using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balle : MonoBehaviour
{
    public int lifeTime; //Temps de vie de la balle
    public float degats; //Degats de la balle


    // Start is called before the first frame update
    void Start()
    {
        //Detruire la balle apres x secondes
        Invoke("DetruireBalle", lifeTime);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Fonction permettant de détruire la balle
    public void DetruireBalle()
    {
        //Destroy la balle
        if(gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Si nous touchons un ennemi
        //Lui faire des degats
        //[FONCTION POUR FAIRE DEGATS]
        DetruireBalle();
    }
}
