using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffetItem : MonoBehaviour

{
    public StatsItems item; // type d'item
    public GameObject joueur; // Référence au joueur
    public string nom; // nom de l'item
    public float valeur; // valeu de l'item
    public float duree; 


    // Start is called before the first frame update
    void Awake()
    {
        // Assigner valeurs du scriptableObject
        nom = item.name;
        valeur = item.valeur;
        duree = item.duree;
        gameObject.GetComponent<MeshRenderer>().material.color = item.couleur;

        // Trouver le joueur lorsque l'item spawn;
        joueur = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
                
            switch (nom)
            {
                case "potionVie":
                    other.gameObject.GetComponent<ComportementJoueur>().Heal(valeur);
                    break;

                case "potionVitesse":
                    //todo: ajouter fonction pour speedBoost dans ComportementJoueur
                    //other.gameObject.GetComponent<ComportementJoueur>().SpeedBoost(valeur, duree);
                    break;

                case "potionDegats":
                    //todo: ajouter fonction DegatsBoost() dans ComportementJoueur
                    //other.gameObject.GetComponent<ComportementJoueur>().DegatsBoost(valeur, duree);
                    break;

                case "potionJump":
                    //todo: ajouter fonction JumpBoost() dans ComportementJoueur
                    //other.gameObject.GetComponent<ComportementJoueur>().JumpBoost(valeur, duree);
                    break;

                case "potionInvulnerable":
                    //todo: ajouter fonction Invulnerable() dans ComportementJoueur
                    //other.gameObject.GetComponent<ComportementJoueur>().DegatsBoost(valeur, duree);
                    break;

                case "nuke":
                    //todo: ajouter fonction Nuke() mais je sais pas trop où
                    break;

                case "slime":
                    other.gameObject.GetComponent<ComportementJoueur>().Grossir(valeur);
                    
                    break;
            }


            DestroyItem(item.sonItem);

        }
    }

    private void DestroyItem(AudioClip audioClip)
    {
        if(audioClip != null)
        {
            joueur.GetComponent<AudioSource>().PlayOneShot(audioClip);
        }
        Destroy(gameObject);
    }
}
