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
    public GameObject menuPause; //Référence au menu Pause
    private bool etatMenuOptions; //Etat du menu d'options
    private bool etatMenuPause; //Etat du menu pause
    public Slider volumeSlider; //Reference au volume slider
    public GameObject joueur; //Reference au joueur

    [Header("Menu Statistiques")]
    public TextMeshProUGUI vitesse; //Valeur de la vitesse du joueur
    public TextMeshProUGUI forceSaut; //Valeur de la force du saut du joueur
    public TextMeshProUGUI degatsSaut; //Valeur des dégats de l'attaque de zone du joueur
    public TextMeshProUGUI degatsBalle; //Valeur des dégats des balles du joueur
    public TextMeshProUGUI delaiTir; //Valeur du délai entre chaque tir
    public TextMeshProUGUI nbrBalles; //Valeur du nombre de balles tirés à chaque tir
    public TextMeshProUGUI armure; //Valeur de l'armure du joueur
    public TextMeshProUGUI regen; //Valeur de la régénération de vie du joueur


    public List<string> lstUpgrades = new List<string>(); //Liste des upgrades de tir qui ont été obtenues
    public TextMeshProUGUI ameliorations; //Élément du menu pour les upgrades de tir qui ont été obtenues


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
        
        //Assigner les valeurs des différentes stats aux textes du menu 
        vitesse.text = joueur.GetComponent<ControleJoueur>().vitesse.ToString();
        forceSaut.text = joueur.GetComponent<ControleJoueur>().forceSaut.ToString();
        degatsSaut.text = joueur.GetComponent<ControleJoueur>().degatsZone.ToString();
        degatsBalle.text = joueur.GetComponent<ControleTir>().degatsJoueur.ToString();
        delaiTir.text = joueur.GetComponent<ControleTir>().shootCooldown.ToString();
        nbrBalles.text = joueur.GetComponent<ControleTir>().nombreBalles.ToString();
        armure.text = joueur.GetComponent<ComportementJoueur>().armure.ToString();
        regen.text = joueur.GetComponent<ComportementJoueur>().regenVie.ToString() + "/s";

        //Ajouter les upgrades de tir au menu
        string upgrades = "";
        foreach(string upgrade in lstUpgrades)
        {
            upgrades += "- ";
            upgrades += upgrade;
            upgrades += "\n";
        }
        ameliorations.text = upgrades;
        Debug.Log(ameliorations.text);


        //Activer le menu de statistiques
        menuStatistiques.SetActive(true);
    }

    public void FermerStatistiques()
    {
        //Fermer le menu
        menuStatistiques.SetActive(false);
    }

    public void MenuPause()
    {
        if (etatMenuPause == false)
        {
            //Activer le menu
            menuPause.SetActive(true);

            //Indiquer que l'etat du menu a change
            etatMenuPause = true;
        }
        else if (etatMenuPause == true)
        {
            FermerPause();
        }
    }

    public void FermerPause()
    {
        //Fermer le menu
        menuPause.SetActive(false);

        //Indiquer que l'etat du menu a change
        etatMenuPause = false;
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
