using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComportementJoueur : MonoBehaviour
{
    public float vieJoueur; //Vie du joueur
    public float vieMax; //Vie max du joueur
    public static bool mortJoueur; //Detecte si nous sommes mort ou non
    public GameObject menuFin; //Reference au menu de fin
    public Slider sliderVie; //Slider de barre de vie

    void Start()
    {
        vieJoueur = vieMax;
    }

    void Update()
    {
        //Mettre a jour la valeur du slider de vie
        float fillValue = vieJoueur / vieMax;
        sliderVie.value = fillValue;

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }
    }

    //Fonction permettant au joueur de prendre des dégâts
    public void TakeDamage(float valeurDegat)
    {
        //Enlever de la vie au joueur
        vieJoueur -= valeurDegat;

        //Si le joueur était pour mourir
        if(vieJoueur <= 0)
        {
            mortJoueur = true;

            //Appeler une fonction affichant le menu de fin
            FinJeu();
        }
    }

    //Fonction permettant de heal le joueur
    public void Heal(float valeurVie)
    {
        //Ajouter de la vie au joueur
        vieJoueur += valeurVie;

        //Si nous avons trop de vie
        if(vieJoueur > vieMax)
        {
            //La mettre a son maximum
            vieJoueur = vieMax;
        }
    }

    public void FinJeu()
    {
        //Faire apparaitre un menu
        menuFin.SetActive(true);
    }
}
