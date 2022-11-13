using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleTir : MonoBehaviour
{
    [Header("Balle")]
    public Balle balle; //Balle tiree

    [Header("Valeurs de tir")]
    bool peutTirer = true; //Bool permettant de savoir si le joueur peut tirer / cooldown
    public float shootDelay; //Delai de tir entre les balles
    public int nombreBalles; //Nombre de balles a tirer
    public float forceTir; //Force du tir
    public float valeurPerteTir; //Scale que le joueur perd en tirant

    [Header("Autres références")]
    public AudioClip sonTir; //Son de tir
    Camera cam; //Ref a la camera
    Vector3 mousePos; //Position de la souris
    public GameObject gun; //Position du "gun"

    void Start()
    {
        //Assigner les références
        cam = Camera.main;
    }


    void Update()
    {

        //Position du curseur


        //Si le joueur appuie sur clicque gauche
        if (Input.GetButtonDown("Fire1") && peutTirer)
        {
            //Indiquer qu'il ne peut plus tirer, et appeler une fonction
            peutTirer = false;
            StartCoroutine(delaiTir(shootDelay));

            //Jouer un son de tir
            GetComponent<AudioSource>().PlayOneShot(sonTir);

            //Pour le nombre de balles a tirer
            for (int i = 0; i < nombreBalles; i++)
            {
                //Instancier une balle
                Balle nouvelleBalle = Instantiate(balle, gun.transform.position, gun.transform.rotation);

                //Propulser la balle vers la direction du curseur
                nouvelleBalle.GetComponent<Rigidbody>().AddForce(gun.transform.forward * forceTir, ForceMode.Impulse);

                //Diminuer la taille du joueur
                gameObject.transform.localScale -= new Vector3(valeurPerteTir, valeurPerteTir, valeurPerteTir);
            }
        }
    }

    public IEnumerator delaiTir(float delai)
    {
        yield return new WaitForSeconds(delai);
        peutTirer = true;
    }
}
