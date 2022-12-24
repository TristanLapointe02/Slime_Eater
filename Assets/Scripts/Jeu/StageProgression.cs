using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Description : Gérer la progression des étages durant la partie
 * Fait par : Tristan Lapointe
 */

public class StageProgression : MonoBehaviour
{
    public int[] ennemiesToKillPerStage; //Nombre d'ennemis à tuer par étage
    public ObjectSpawner refSpawnEnnemi; //Référence au spawn d'ennemis
    public ObjectSpawner refSpawnItem; //Référence au spawn d'items
    public GameObject[] Etages; //Référence aux étages
    public static int etageActuel; //Étage actuel
    public AudioClip sonChangerEtage; //Son qui joue lorsqu'on change d'étage
    public GameObject joueur; //Référence au joueur
    public AudioClip sonVictoire; //Son de victoire lorsque le joueur fini le jeu
    public GameObject boss; //Référence au boss
    public AudioClip bossMusic; //Musique du boss

    private void Awake()
    {
        //Reset l'étage actuel à 1
        etageActuel = 1;

        //Trouver le joueur
        joueur = SpawnJoueur.joueur;
    }

    void Update()
    {
        //Constamment vérifier si nous avons fini un niveau
        VerificationNiveau();

        //Mettre a jour la progression du niveau si on a pas fini
        if(etageActuel <= Etages.Length && ComportementJoueur.finJeu == false)
        {
            joueur.GetComponent<ComportementJoueur>().fillProgression.fillAmount = ComportementJoueur.ennemisTues / ennemiesToKillPerStage[etageActuel - 1];
        }

        //TEST POUR CHANGER DE NIVEAU IMMEDIATEMENT
        if (Input.GetKeyDown(KeyCode.B) && etageActuel < Etages.Length && ComportementJoueur.finJeu == false)
        {
            ComportementJoueur.ennemisTues = ennemiesToKillPerStage[etageActuel - 1];
        }
    }

    //Fonction permettant de vérifier si un étage est terminé
    public void VerificationNiveau()
    {
        for (int i = 1; i <= ennemiesToKillPerStage.Length; i++)
        {
            //Pour chaque étage, si le joueur tue assez d'ennemis pour passer à l'étage suivant
            if (etageActuel == i && ComportementJoueur.ennemisTues >= ennemiesToKillPerStage[i-1] && ComportementJoueur.finJeu == false && etageActuel <= Etages.Length)
            {
                //Si on vient de finir le jeu
                if (etageActuel == Etages.Length)
                {
                    //Faire apparaître le menu de fin
                    joueur.GetComponent<ComportementJoueur>().FinJeu("Vous avez gagné!", sonVictoire);
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
        //Si nous n'avons pas fini
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

            //Détruire le plancher de l'étage
            Etages[etageActuel - 1].gameObject.SetActive(false);

            //Faire spawn des objets
            refSpawnEnnemi.InitialSpawn();
            refSpawnItem.InitialSpawn();

            //Jouer un sound effect
            joueur.gameObject.GetComponent<AudioSource>().PlayOneShot(sonChangerEtage);

            //Reset le compteur d'ennemis tués
            ComportementJoueur.ennemisTues = 0;

            //Montrer les visuels de la zone d'explosion
            GameObject.Find("ZoneDegats").gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        //Si on est à l'avant dernier niveau, spawn le boss
        if(etageActuel == Etages.Length - 1)
        {
            //Instancier le boss
            Instantiate(boss, new Vector3(joueur.transform.position.x, 0, joueur.transform.position.z), Quaternion.identity);

            //Jouer un bruit de boss fight
            GetComponent<AudioSource>().PlayOneShot(bossMusic);
        }
    }
}
