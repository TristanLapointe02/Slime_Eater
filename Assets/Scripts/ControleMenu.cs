using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControleMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
