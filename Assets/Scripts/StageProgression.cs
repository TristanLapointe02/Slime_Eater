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
    public GameObject mur; //Reference au mur
    public Color32[] couleurMur; //Tableau de couleurs de niveau
    public GameObject boss; //Reference au boss
    public AudioClip bossMusic; //Musique du boss

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

        //Mettre a jour la progression du niveau si on a pas fini
        if(etageActuel <= Etages.Length && ComportementJoueur.finJeu == false)
        {
            fillProgression.fillAmount = ComportementJoueur.ennemisTues / ennemiesToKillPerStage[etageActuel - 1];
        }
        

        //TEST POUR CHANGER DE NIVEAU IMMEDIATEMENT
        if (Input.GetKeyDown(KeyCode.B) && etageActuel < Etages.Length && ComportementJoueur.finJeu == false)
        {
            ComportementJoueur.ennemisTues = ennemiesToKillPerStage[etageActuel-1];
        }
    }

    //Fonction permettant de vérifier si un etage est terminé
    public void VerificationNiveau()
    {
        //Mettre a jour le texte de niveau
        texteStage.text = etageActuel.ToString();

        for (int i = 1; i <= ennemiesToKillPerStage.Length; i++)
        {
            if (etageActuel == i && ComportementJoueur.ennemisTues >= ennemiesToKillPerStage[i-1] && ComportementJoueur.finJeu == false && etageActuel <= Etages.Length)
            {
                //Si on vient de finir le jeu
                if (etageActuel == Etages.Length)
                {
                    //Faire apparaitre le menu de fin
                    joueur.GetComponent<ComportementJoueur>().FinJeu("Vous avez gagné!", sonVictoire);
                    print("Etage actuel " + etageActuel + " Etage length " + Etages.Length);
                }

                //Sinon, pour le restant des étages
                else
                {
                    //Changer de niveau
                    ChangerNiveau();

                    //Augmenter d'étage
                    etageActuel++;
                } 
            }
        }
    }

    //Fonction qui permet de changer de niveau
    public void ChangerNiveau()
    {
        //Si nous avons pas fini
        if(etageActuel <= Etages.Length && ComportementJoueur.finJeu == false)
        {
            //Pour tous les ennemis présents dans la scène
            GameObject[] ennemisPresents = GameObject.FindGameObjectsWithTag("Ennemi");

            //Les éléminer sans drop de loot
            foreach (GameObject ennemi in ennemisPresents)
            {
                if (ennemi.TryGetComponent(out EnemyController enemyControler))
                {
                    enemyControler.MortEnnemi();
                }
            }

            //Detruire le plancher de l'etage
            Etages[etageActuel - 1].gameObject.SetActive(false);

            //Faire spawn des objets
            refSpawnEnnemi.InitialSpawn();
            refSpawnItem.InitialSpawn();

            //Jouer un sound effect
            joueur.gameObject.GetComponent<AudioSource>().PlayOneShot(sonChangerEtage);

            //Changer la couleur du mur
            mur.GetComponent<MeshRenderer>().material.color = couleurMur[etageActuel - 1];

            //Reset le compteur d'ennemis tues
            ComportementJoueur.ennemisTues = 0;

            //Montrer les visuels de la zone d'explosion
            GameObject.Find("ZoneDegats").gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        //Si on est à l'avant dernier niveau, spawn le boss
        if(etageActuel == Etages.Length - 1)
        {
            //Instancier le boss
            Instantiate(boss, gameObject.transform.position, Quaternion.identity);

            //Jouer un bruit de boss fight
            GetComponent<AudioSource>().PlayOneShot(bossMusic);
        }
    }
}
