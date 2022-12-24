using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Gestion et affichage des menus
 * Fait par : Samuel S�guin
 */

public class ControleMenu : MonoBehaviour
{
    public GameObject menuOptions; //R�f�rence au menu Options
    public GameObject menuStatistiques; //R�f�rence au menu Statistiques
    public GameObject menuPause; //R�f�rence au menu Pause
    private bool etatMenuOptions; //�tat du menu Options
    private bool etatMenuPause; //�tat du menu Pause
    public Slider volumeSlider; //R�f�rence au volume slider
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
    public TextMeshProUGUI taille; //Valeur de la taille du joueur 
   

    public List<string> listUpgrades = new List<string>(); //Liste des am�liorations qui ont �t� obtenus
    public TextMeshProUGUI ameliorations; //�l�ment du menu pour les am�liorations qui ont �t� obtenus
    public TextMeshProUGUI ameliorations2; //Deuxi�me �l�ment du menu pour les am�liorations qui ont �t� obtenus

    private void Awake()
    {
        //Reset la pause
        pauseMenu = false;
    }

    void Start()
    {
        //Si aucun param�tre de volume est enregistr�
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            //Mettre le volume � 50%
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
        }
        //Si nous avions deja des param�tres d'enregistr�s
        else
        {
            //Load nos param�tres
            LoadVolume();
        }
    }

    //Fonction permettant de recommencer � jouer
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

    //G�rer le menu Options
    public void MenuOptions()
    {
        //Si le menu Options �tait d�sactiv�
        if (etatMenuOptions == false)
        {
            //Activer le menu d'options
            menuOptions.SetActive(true);

            //Indiquer que l'�tat du menu a chang�
            etatMenuOptions = true;
        }
        //Si le menu Options �tait activ�
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

        //Ajouter les am�liorations obtenus
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
            // Ajouter les upgrades suivantes � la deuxi�me colone s'il y a lieu
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


    //Fonction pour g�rer le menu Pause
    public void MenuPause()
    {
        //Si le menu Pause n'est pas activ�
        if (etatMenuPause == false)
        {
            //Activer le menu
            menuPause.SetActive(true);

            //Indiquer que l'�tat du menu a chang�
            etatMenuPause = true;

            //Activer la pause
            pauseMenu = true;
        }
        //Si le menu Pause est activ�
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

        //Indiquer que l'�tat du menu a chang�
        etatMenuPause = false;

        //D�sactiver la pause
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
        //R�cup�rer la valeur sauvegard�e
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
