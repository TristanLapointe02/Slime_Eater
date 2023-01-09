using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/*
 * Description : Gestion du tir de balle pour les ennemis qui attaquent à distance
 * Fait par : Tristan Lapointe et Samuel Séguin
 */

public class EnnemiTir : MonoBehaviour
{

    [Header("Balle")]
    public Balle balle; //Balle tirée

    [Header("Valeurs de tir")]
    public bool peutTirer; //Bool permettant de savoir si l'ennemi peut tirer/cooldown
    public float shootCooldown; //Cooldown de tir
    public float shootDelay; //Délai entre les balles tirées
    public int nombreBalles; //Nombre de balles à tirer
    public float forceTir; //Force du tir

    //VARIABLE DE TEST
    public bool forceStopTir; //Forcer l'arrêt du tir

    [Header("Autres références")]
    public AudioClip sonTir; //Son de tir
    public GameObject gun; //Position du "gun"

    private void Start()
    {
        //Commencer initialement le délai de tir
        StartCoroutine(delaiTir(shootCooldown));
    }
    void Update()
    {
        //Faire que le gun pointe vers le joueur
        gun.transform.LookAt(SpawnJoueur.joueur.transform);
    }

    private void FixedUpdate()
    {
        //S'il peut tirer et qu'il est in range
        if (peutTirer && ControleAmeliorations.pause == false && ControleMenu.pauseMenu == false && GetComponent<EnemyController>().InRangeJoueur() && forceStopTir == false)
        {
            //Commencer le cooldown après avoir tirer
            peutTirer = false;
            StartCoroutine(delaiTir(shootCooldown));

            //Jouer un son de tir
            GetComponent<AudioSource>().PlayOneShot(sonTir);

            //Tirer une balle vers l'avant
            StartCoroutine(tirBalleEnnemie(nombreBalles, shootDelay, gun.transform.forward));
        }
    }

    //Fonction de cooldown du tir
    public IEnumerator delaiTir(float delai)
    {
        //Attendre un certain délai
        yield return new WaitForSeconds(delai);

        //Indiquer qu'il peut tirer à nouveau
        peutTirer = true;
    }

    //Fonction permettant de tirer les balles
    public IEnumerator tirBalleEnnemie(int nbBalles, float delai, Vector3 direction)
    {
        //Pour le nombre de balles a tirer
        for (int i = 0; i < nbBalles; i++)
        {
            //Jouer un son de tir
            GetComponent<AudioSource>().PlayOneShot(sonTir);

            //Instancier une balle
            Balle nouvelleBalle = Instantiate(balle, gun.transform.position, gun.transform.rotation);

            //Propulser la balle vers la direction du joueur
            nouvelleBalle.GetComponent<Rigidbody>().AddForce(gun.transform.forward * forceTir, ForceMode.Impulse);

            //Changer les dégâts de la balle selon ceux du joueur
            nouvelleBalle.GetComponent<Balle>().degats = GetComponent<EnemyController>().enemy.degats;

            //Délai pour la prochaine balle
            yield return new WaitForSeconds(delai);
        }
    }
}
