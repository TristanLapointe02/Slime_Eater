using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StageProgression : MonoBehaviour
{
    public int[] ennemiesToKillPerStage; //Nombre d'ennemis a tuer par stage
    public ObjectSpawner refSpawnEnnemi; //Reference au spawn d'ennemis
    public ObjectSpawner refSpawnItem; //Reference au spawn d'items
    public TextMeshProUGUI texteStage; //Texte affichant le stage actuel
    public GameObject[] Etages; //Reference aux etages
    public static int etageActuel; //Etage actuel
    public AudioClip sonChangerEtage; //Son qui joue lorsqu'on change d'etage
    public GameObject joueur; //Reference au joueur
    public AudioClip sonVictoire; //Son de victoire lorsque le joueur fini le jeu
    public Image fillProgression; //Image indiquant la progression du niveau

    private void Awake()
    {
        //Reset l'etage actuel à 1
        etageActuel = 1;

        //Trouver le joueur
        joueur = GameObject.Find("Joueur");
    }

    void Update()
    {
        //Constamment verifier si nous avons fini un niveau
        VerificationNiveau();

        //Mettre a jour la progression du niveau
        fillProgression.fillAmount = ComportementJoueur.ennemisTues / ennemiesToKillPerStage[etageActuel - 1];

        //TEST POUR CHANGER DE NIVEAU IMMEDIATEMENT
        if (Input.GetKeyDown(KeyCode.B))
        {
            etageActuel++;
            ChangerNiveau();
        }
    }

    //Fonction permettant de vérifier si un etage est terminé
    public void VerificationNiveau()
    {
        //Mettre a jour le texte de niveau
        texteStage.text = etageActuel.ToString();

        for (int i = 1; i < ennemiesToKillPerStage.Length; i++)
        {
            if (etageActuel == i && ComportementJoueur.ennemisTues >= ennemiesToKillPerStage[i-1])
            {
                //Augmenter d'étage
                etageActuel++;

                //Si on vient de finir le jeu
                if (etageActuel == Etages.Length)
                {
                    //Faire apparaitre le menu de fin
                    joueur.GetComponent<ComportementJoueur>().FinJeu("Vous avez gagné!", sonVictoire);
                }

                //Sinon, pour le restant des étages
                else
                {
                    ChangerNiveau();
                } 
            }
        }
    }

    //Fonction qui permet de changer de niveau
    public void ChangerNiveau()
    {
        //Detruire le plancher de l'etage
        Etages[etageActuel - 2].gameObject.SetActive(false);

        //Faire spawn des objets
        refSpawnEnnemi.InitialSpawn();
        refSpawnItem.InitialSpawn();

        //Jouer un sound effect
        joueur.gameObject.GetComponent<AudioSource>().PlayOneShot(sonChangerEtage);

        //Reset le compteur d'ennemis tues
        ComportementJoueur.ennemisTues = 0;
    }
}
