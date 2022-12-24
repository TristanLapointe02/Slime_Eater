using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Gestion et affichage des menus
 * Fait par : Samuel Séguin
 */

public class ControleMenu : MonoBehaviour
{
    public GameObject menuOptions; //Référence au menu Options
    public GameObject menuStatistiques; //Référence au menu Statistiques
    public GameObject menuPause; //Référence au menu Pause
    private bool etatMenuOptions; //État du menu Options
    private bool etatMenuPause; //État du menu Pause
    public Slider volumeSlider; //Référence au volume slider
    public static bool pauseMenu; //Indique si nous sommes en pause

    [Header("Menu Statistiques")]
    public TextMeshProUGUI vieMax; //Valeur de la vie max du joueur
    public TextMeshProUGUI regen; //Valeur de la régénération de vie du joueur
    public TextMeshProUGUI vitesse; //Valeur de la vitesse du joueur
    public TextMeshProUGUI forceSaut; //Valeur de la force du saut du joueur
    public TextMeshProUGUI degatsSaut; //Valeur des dégats de l'attaque de zone du joueur
    public TextMeshProUGUI forceExplosion; //Valeur de l'armure du joueur
    public TextMeshProUGUI degatsBalle; //Valeur des dégats des balles du joueur
    public TextMeshProUGUI delaiTir; //Valeur du délai entre chaque tir
    public TextMeshProUGUI perteTir; //Valeur du nombre de balles tirés à chaque tir
    public TextMeshProUGUI taille; //Valeur de la taille du joueur 
   

    public List<string> listUpgrades = new List<string>(); //Liste des améliorations qui ont été obtenus
    public TextMeshProUGUI ameliorations; //Élément du menu pour les améliorations qui ont été obtenus
    public TextMeshProUGUI ameliorations2; //Deuxième élément du menu pour les améliorations qui ont été obtenus

    private void Awake()
    {
        //Reset la pause
        pauseMenu = false;
    }

    void Start()
    {
        //Si aucun paramètre de volume est enregistré
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            //Mettre le volume à 50%
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
        }
        //Si nous avions deja des paramètres d'enregistrés
        else
        {
            //Load nos paramètres
            LoadVolume();
        }
    }

    //Fonction permettant de recommencer à jouer
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

    //Fonction permettant d'aller au menu principal
    public void MainMenu()
    {
        //Retourner au menu principal
        SceneManager.LoadScene("MainMenu");
    }

    //Gérer le menu Options
    public void MenuOptions()
    {
        //Si le menu Options était désactivé
        if (etatMenuOptions == false)
        {
            //Activer le menu d'options
            menuOptions.SetActive(true);

            //Indiquer que l'état du menu a changé
            etatMenuOptions = true;
        }
        //Si le menu Options était activé
        else if (etatMenuOptions == true)
        {
            //Fermer le menu
            FermerOptions();
        }
    }

    //Fonction permettant de fermer le menu Options
    public void FermerOptions()
    {
        //Fermer le menu
        menuOptions.SetActive(false);

        //Indiquer que l'etat du menu a change
        etatMenuOptions = false;
    }

    //Fonction pour ouvrir le menu Statistiques
    public void OuvrirStatistiques()
    {
        //Assigner les valeurs des différentes stats aux textes du menu
        vieMax.text = GetComponent<ComportementJoueur>().vieMax.ToString();
        regen.text = GetComponent<ComportementJoueur>().regenVie.ToString() + "/s";
        vitesse.text = GetComponent<ControleJoueur>().vitesse.ToString();
        forceSaut.text = GetComponent<ControleJoueur>().forceSaut.ToString();
        degatsSaut.text = GetComponent<ControleJoueur>().degatsZone.ToString();
        int forceExplosionRound = Mathf.RoundToInt(GetComponent<ControleJoueur>().forceExplosion); //Arrondir la valeur de forceExplosion pour ne pas afficher les décimales
        forceExplosion.text = forceExplosionRound.ToString();
        degatsBalle.text = GetComponent<ControleTir>().degatsJoueur.ToString();
        delaiTir.text = GetComponent<ControleTir>().shootCooldown.ToString();
        perteTir.text = GetComponent<ControleTir>().valeurPerteTir.ToString();
        taille.text = (Mathf.Round(GetComponent<Transform>().localScale.y * 10f) *0.1f).ToString(); //Arrondir la valeur de taille à 1 décimale après la virgule

        //Ajouter les améliorations obtenus
        string upgrades = "";
        string upgrades2 = "";
        int counter = 0;
        foreach(string upgrade in listUpgrades)
        {
            //Ajouter les 12 premiers upgrades à la première colone
            if (counter <= 11)
            {
                upgrades += "- ";
                upgrades += upgrade;
                upgrades += "\n";
                counter++;
            }
            // Ajouter les upgrades suivantes à la deuxième colone s'il y a lieu
            else
            {
                upgrades2 += "- ";
                upgrades2 += upgrade;
                upgrades2 += "\n";
                counter++;
            }
        }

        // Remplir les 2 colones
        ameliorations.text = upgrades;
        ameliorations2.text = upgrades2;

        //Activer le menu de statistiques
        menuStatistiques.SetActive(true);
    }


    //Fonction pour fermer le menu Statistiques
    public void FermerStatistiques()
    {
        //Fermer le menu
        menuStatistiques.SetActive(false);
    }


    //Fonction pour gérer le menu Pause
    public void MenuPause()
    {
        //Si le menu Pause n'est pas activé
        if (etatMenuPause == false)
        {
            //Activer le menu
            menuPause.SetActive(true);

            //Indiquer que l'état du menu a changé
            etatMenuPause = true;

            //Activer la pause
            pauseMenu = true;
        }
        //Si le menu Pause est activé
        else if (etatMenuPause == true)
        {
            //Fermer le menu
            FermerPause();
        }
    }

    //Fonction pour fermer le menu Pause
    public void FermerPause()
    {
        //Fermer le menu
        menuPause.SetActive(false);

        //Indiquer que l'état du menu a changé
        etatMenuPause = false;

        //Désactiver la pause
        pauseMenu = false;
    }


    //Fonction pour changer le volume
    public void ChangeVolume()
    {
        //Changer le volume
        AudioListener.volume = volumeSlider.value;

        //Enregistrer les changements
        SaveVolume();
    }

    

    //Fonction permettant de charger les bons settings du joueur
    private void LoadVolume()
    {
        //Récupérer la valeur sauvegardée
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
