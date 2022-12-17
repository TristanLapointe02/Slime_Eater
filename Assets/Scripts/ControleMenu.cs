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
    public GameObject menuStatistiques; //R�f�rence au menu Statistiques
    public GameObject menuPause; //R�f�rence au menu Pause
    private bool etatMenuOptions; //Etat du menu d'options
    private bool etatMenuPause; //Etat du menu pause
    public Slider volumeSlider; //Reference au volume slider
    public static bool pauseMenu; //Indique si nous sommes en pause

    [Header("Menu Statistiques")]
    public TextMeshProUGUI vieMax; //Valeur de la vie max du joueur
    public TextMeshProUGUI regen; //Valeur de la r�g�n�ration de vie du joueur
    public TextMeshProUGUI vitesse; //Valeur de la vitesse du joueur
    public TextMeshProUGUI forceSaut; //Valeur de la force du saut du joueur
    public TextMeshProUGUI degatsSaut; //Valeur des d�gats de l'attaque de zone du joueur
    public TextMeshProUGUI forceExplosion; //Valeur de l'armure du joueur
    public TextMeshProUGUI degatsBalle; //Valeur des d�gats des balles du joueur
    public TextMeshProUGUI delaiTir; //Valeur du d�lai entre chaque tir
    public TextMeshProUGUI perteTir; //Valeur du nombre de balles tir�s � chaque tir
    public TextMeshProUGUI taille;
   

    public List<string> listUpgrades = new List<string>(); //Liste des upgrades de tir qui ont �t� obtenues
    public TextMeshProUGUI ameliorations; //�l�ment du menu pour les upgrades de tir qui ont �t� obtenues
    public TextMeshProUGUI ameliorations2;

    private void Awake()
    {
        //Reset la pause
        pauseMenu = false;
    }

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

            //Indiquer que l'�tat du menu a chang�
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
        //Assigner les valeurs des diff�rentes stats aux textes du menu
        vieMax.text = GetComponent<ComportementJoueur>().vieMax.ToString();
        regen.text = GetComponent<ComportementJoueur>().regenVie.ToString() + "/s";
        vitesse.text = GetComponent<ControleJoueur>().vitesse.ToString();
        forceSaut.text = GetComponent<ControleJoueur>().forceSaut.ToString();
        degatsSaut.text = GetComponent<ControleJoueur>().degatsZone.ToString();
        int forceExplosionRound = Mathf.RoundToInt(GetComponent<ControleJoueur>().forceExplosion); //Arrondir la valeur de forceExplosion pour ne pas afficher les d�cimales
        forceExplosion.text = forceExplosionRound.ToString();
        degatsBalle.text = GetComponent<ControleTir>().degatsJoueur.ToString();
        delaiTir.text = GetComponent<ControleTir>().shootCooldown.ToString();
        perteTir.text = GetComponent<ControleTir>().valeurPerteTir.ToString();
        taille.text = (Mathf.Round(GetComponent<Transform>().localScale.y * 10f) *0.1f).ToString(); //Arrondir la valeur de taille � 1 d�cimale apr�s la virgule

        //Ajouter les upgrades de tir au menu
        string upgrades = "";
        string upgrades2 = "";
        int counter = 0;
        foreach(string upgrade in listUpgrades)
        {
            //Ajouter les 12 premiers upgrades � la premi�re colone
            if (counter <= 11)
            {
                upgrades += "- ";
                upgrades += upgrade;
                upgrades += "\n";
                counter++;
            }
            // Ajouter les upgrades suivantes � la deuxi�me colone
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

            //Activer la pause
            pauseMenu = true;
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

        //D�sactiver la pause
        pauseMenu = false;
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
        //Sauvegarder la valeur des sliders dans une cat�gorie playerprefs
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}
