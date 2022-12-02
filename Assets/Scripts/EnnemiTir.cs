using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnnemiTir : MonoBehaviour
{

    [Header("Balle")]
    public Balle balle; //Balle tirée

    [Header("Valeurs de tir")]
    bool peutTirer = true; //Bool permettant de savoir si l'ennemi peut tirer/cooldown
    public float shootDelay; //Délai entre les balles tirées
    public int nombreBalles; //Nombre de balles a tirer
    public float forceTir; //Force du tir

    [Header("Autres références")]
    public AudioClip sonTir; //Son de tir
    public GameObject gun; //Position du "gun"

    void Update()
    {
        if (peutTirer && GetComponent<EnemyController>().InRangeJoueur())
        {
            //Commencer le cooldown après avoir tirer
            peutTirer = false;
            StartCoroutine(delaiTir(shootDelay));

            // Jouer un son de tir
            //GetComponent<AudioSource>().PlayOneShot(sonTir);

            //Pour le nombre de balles à tirer
            for (int i = 0; i < nombreBalles; i++)
            {
                // Instancier une nouvelle balle
                Balle nouvelleBalle = Instantiate(balle, gun.transform.position, gun.transform.rotation);

                // Propulser la balle vers la direction du joueur
                nouvelleBalle.GetComponent<Rigidbody>().AddForce(gun.transform.forward * forceTir, ForceMode.Impulse);

                // Changer les dégats de la balle selon ceux de l'ennemi
                nouvelleBalle.GetComponent<Balle>().degats = GetComponent<EnemyController>().enemy.degats;
            }
        }
    }

    //Fonction de cooldown du tir
    public IEnumerator delaiTir(float delai)
    {
        yield return new WaitForSeconds(delai);
        peutTirer = true;
    }
}
