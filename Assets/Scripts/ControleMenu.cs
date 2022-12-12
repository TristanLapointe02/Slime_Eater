using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControleMenu : MonoBehaviour
{
    public GameObject menuOptions; //Reference au menu options
    public GameObject menuStatistiques; //Référence au menu Statistiques
    private bool etatMenuOptions; //Etat du menu d'options
    public Slider volumeSlider; //Reference au volume slider
    public GameObject Joueur; //Reference au joueur

    [Header("Menu Statistiques")]
    public TextMeshProUGUI taille; //Valeur de la taille du joueur
    public TextMeshProUGUI vitesse; //Valeur de la vitesse du joueur
    public TextMeshProUGUI degats; //Valeur des dégats du joueur
    public TextMeshProUGUI delaiTir; //Valeur du délai de tir
    public TextMeshProUGUI nbrBalles; //Valeur du nombre de balles tirés
    


    //Fonction permettant de recommencer a jouer
    public void ResetJeu()
    {
        //Reset le jeu
        SceneManager.LoadScene("InGame");
    }

    //Fonction permettant de quitter le jeu
    public void QuitterJeu()
    {
        //Quitter l'appli
        Application.Quit();
    }

    public void MainMenu()
    {
        //Retourner au menu principal
        SceneManager.LoadScene("MainMenu");
    }


    //TODO créer un script séparé pour la gestion du menu d'options (qui pourra être appelé ici et dans GestionMenuPause)

    public void MenuOptions()
    {
        if (etatMenuOptions == false)
        {
            //Activer le menu d'options
            menuOptions.SetActive(true);
            //Indiquer que l'état du menu a changé
            etatMenuOptions = true;

        }
        else if (etatMenuOptions == true)
        {
            FermerOptions();
        }
    }

    //Fonction permettant de ferme le menu d'options
    public void FermerOptions()
    {
        //Fermer le menu
        menuOptions.SetActive(false);

        //Indiquer que l'etat du menu a change
        etatMenuOptions = false;
    }

    public void OuvrirStatistiques()
    {
        //Activer le menu de statistiques
        menuStatistiques.SetActive(true);
        //Assigner les valeurs des différentes stats aux textes du menu
        taille.text = Joueur.GetComponent<Transform>().localScale.y.ToString();   
        vitesse.text = Joueur.GetComponent<ControleJoueur>().vitesse.ToString();
        degats.text = Joueur.GetComponent<ControleTir>().degatsJoueur.ToString();
        delaiTir.text = Joueur.GetComponent<ControleTir>().shootCooldown.ToString();
        nbrBalles.text = Joueur.GetComponent<ControleTir>().nombreBalles.ToString();
        
    }

    public void FermerStatistiques()
    {
        //Fermer le menu
        menuStatistiques.SetActive(false);
    }

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
