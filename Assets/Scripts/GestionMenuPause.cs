using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class GestionMenuPause : MonoBehaviour
{
    public GameObject menuJeu; //Reference au menu de pause
    private bool etatMenu; //Etat du menu de pause
    public Slider volumeSlider; //Reference au volume slider

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
        }
        //Si nous avions deja des parametres d'enregistres
        else
        {
            //Load nos parametres
            LoadVolume();
        }
    }

    private void Update()
    {
        //Si jamais le joueur appuie sur escape
        if (Input.GetButtonDown("Cancel"))
        {
            print("miaw");
            if (etatMenu == false)
            {
                //Activer le menu
                menuJeu.SetActive(true);

                //Indiquer que l'etat du menu a change
                etatMenu = true;
            }
            else if (etatMenu == true)
            {
                FermerMenu();
            }
        }
    }

    //Fonction permettant de ferme le menu
    public void FermerMenu()
    {
        //Fermer le menu
        menuJeu.SetActive(false);

        //Indiquer que l'etat du menu a change
        etatMenu = false;
    }

    //Fonction permettant de changer le volume
    public void ChangeVolume()
    {
        //Changer le volume
        AudioListener.volume = volumeSlider.value;

        //Enregistrer les changements
        SaveVolume();
    }

    //Fonction permettant de loader les bons settings du joueur
    private void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");

        //Enregistrer les changements
        SaveVolume();
    }

    //Fonction permettant de sauvegarder les settings du joueur
    private void SaveVolume()
    {
        //Sauvegarder la valeur des sliders dans une catégorie playerprefs
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}
