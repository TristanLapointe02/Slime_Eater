using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Description : G�rer la progression des �tages durant la partie
 * Fait par : Tristan Lapointe
 */

public class StageProgression : MonoBehaviour
{
    public int[] ennemiesToKillPerStage; //Nombre d'ennemis � tuer par �tage
    public ObjectSpawner refSpawnEnnemi; //R�f�rence au spawn d'ennemis
    public ObjectSpawner refSpawnItem; //R�f�rence au spawn d'items
    public TextMeshProUGUI texteStage; //Texte affichant le stage actuel
    public GameObject[] Etages; //R�f�rence aux �tages
    public static int etageActuel; //�tage actuel
    public AudioClip sonChangerEtage; //Son qui joue lorsqu'on change d'�tage
    public GameObject joueur; //R�f�rence au joueur
    public AudioClip sonVictoire; //Son de victoire lorsque le joueur fini le jeu
    public Image fillProgression; //Image indiquant la progression du niveau
    public GameObject mur; //R�f�rence au mur
    public Color32[] couleurMur; //Tableau de couleurs de niveau
    public GameObject boss; //R�f�rence au boss
    public AudioClip bossMusic; //Musique du boss

    private void Awake()
    {
        //Reset l'�tage actuel � 1
        etageActuel = 1;

        //Trouver le joueur
        joueur = GameObject.Find("Joueur");
    }

    void Update()
    {
        //Constamment v�rifier si nous avons fini un niveau
        VerificationNiveau();

        //Mettre a jour la progression du niveau si on a pas fini
        if(etageActuel <= Etages.Length && ComportementJoueur.finJeu == false)
        {
            fillProgression.fillAmount = ComportementJoueur.ennemisTues / ennemiesToKillPerStage[etageActuel - 1];
        }
    }

    //Fonction permettant de v�rifier si un �tage est termin�
    public void VerificationNiveau()
    {
        //Mettre � jour le texte de niveau
        texteStage.text = etageActuel.ToString();

        for (int i = 1; i <= ennemiesToKillPerStage.Length; i++)
        {
            //Pour chaque �tage, si le joueur tue assez d'ennemis pour passer � l'�tage suivant
            if (etageActuel == i && ComportementJoueur.ennemisTues >= ennemiesToKillPerStage[i-1] && ComportementJoueur.finJeu == false && etageActuel <= Etages.Length)
            {
                //Si on vient de finir le jeu
                if (etageActuel == Etages.Length)
                {
                    //Faire appara�tre le menu de fin
                    joueur.GetComponent<ComportementJoueur>().FinJeu("Vous avez gagn�!", sonVictoire);
                    print("Etage actuel " + etageActuel + " Etage length " + Etages.Length);
                }

                //Sinon, pour le restant des �tages
                else
                {
                    //Changer de niveau
                    ChangerNiveau();

                    //Augmenter d'�tage
                    etageActuel++;
                } 
            }
        }
    }

    //Fonction qui permet de changer de niveau
    public void ChangerNiveau()
    {
        //Si nous n'avons pas fini
        if(etageActuel <= Etages.Length && ComportementJoueur.finJeu == false)
        {
            //Pour tous les ennemis pr�sents dans la sc�ne
            GameObject[] ennemisPresents = GameObject.FindGameObjectsWithTag("Ennemi");

            //Les �l�miner sans drop de loot
            foreach (GameObject ennemi in ennemisPresents)
            {
                if (ennemi.TryGetComponent(out EnemyController enemyControler))
                {
                    enemyControler.MortEnnemi();
                }
            }

            //D�truire le plancher de l'�tage
            Etages[etageActuel - 1].gameObject.SetActive(false);

            //Faire spawn des objets
            refSpawnEnnemi.InitialSpawn();
            refSpawnItem.InitialSpawn();

            //Jouer un sound effect
            joueur.gameObject.GetComponent<AudioSource>().PlayOneShot(sonChangerEtage);

            //Changer la couleur du mur
            mur.GetComponent<MeshRenderer>().material.color = couleurMur[etageActuel - 1];

            //Reset le compteur d'ennemis tu�s
            ComportementJoueur.ennemisTues = 0;

            //Montrer les visuels de la zone d'explosion
            GameObject.Find("ZoneDegats").gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        //Si on est � l'avant dernier niveau, spawn le boss
        if(etageActuel == Etages.Length - 1)
        {
            //Instancier le boss
            Instantiate(boss, gameObject.transform.position, Quaternion.identity);

            //Jouer un bruit de boss fight
            GetComponent<AudioSource>().PlayOneShot(bossMusic);
        }
    }
}
