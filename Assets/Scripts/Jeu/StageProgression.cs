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
    public int ennemiesToKillInitially; //Nombre d'ennemis à tuer par étage
    [HideInInspector] int ennemiesToKill; //Nombre d'ennemis à tuer pour cet étage
    public int ennemiesToKillMultiplicateur; //À quel point il faut tuer plus d'ennemis par étage
    public ObjectSpawner refSpawnEnnemi; //Référence au spawn d'ennemis
    public ObjectSpawner refSpawnItem; //Référence au spawn d'items
    public static int etageActuel; //Étage actuel
    public AudioClip sonChangerEtage; //Son qui joue lorsqu'on change d'étage
    [HideInInspector] public GameObject joueur; //Référence au joueur
    public AudioClip sonVictoire; //Son de victoire lorsque le joueur fini le jeu
    public GameObject boss; //Référence au boss
    public AudioClip bossMusic; //Musique du boss
    public GestionSpawnPlancherV3 gestionPlancher; //Reference au gestionnaire de plancher
    public GameObject plancherInvisible; //Reference au plancher invisible
    public GameObject UIChangementNiveau; //Prefab UI de changement de niveau

    private void Awake()
    {
        //Reset l'étage actuel à 1
        etageActuel = 1;
    }

    private void Start()
    {
        //Trouver le joueur
        joueur = SpawnJoueur.joueur;

        //Indiquer une couleur de départ
        gestionPlancher.ChangerCouleur();

        //Indiquer le nombre d'ennemis à tuer au départ
        ennemiesToKill = ennemiesToKillInitially;

        //Initialement afficher le niveau
        AfficherNiveau(etageActuel);
    }

    void Update()
    {
        //Constamment vérifier si nous avons fini un niveau
        VerificationNiveau();

        //Mettre a jour la progression du niveau si on a pas fini
        if(etageActuel <= gestionPlancher.nombreEtages && ComportementJoueur.finJeu == false)
        {
            joueur.GetComponent<ComportementJoueur>().fillProgression.fillAmount = ComportementJoueur.ennemisTues / ennemiesToKill;
        }

        //Si la distance entre le joueur et le plancher invisible est trop grande, la recadrer
        if(Vector3.Distance(joueur.transform.position, plancherInvisible.transform.position) > 500)
        {
            plancherInvisible.transform.position = new Vector3(joueur.transform.position.x, plancherInvisible.transform.position.y, joueur.transform.position.z);
        }

        //TEST POUR CHANGER DE NIVEAU IMMEDIATEMENT
        if (Input.GetKeyDown(KeyCode.B) && etageActuel < gestionPlancher.nombreEtages && ComportementJoueur.finJeu == false)
        {
            ComportementJoueur.ennemisTues = ennemiesToKill;
        }
    }

    //Fonction permettant de vérifier si un étage est terminé
    public void VerificationNiveau()
    {
        for (int i = 1; i <= gestionPlancher.nombreEtages; i++)
        {
            //Pour chaque étage, si le joueur tue assez d'ennemis pour passer à l'étage suivant
            if (etageActuel == i && ComportementJoueur.ennemisTues >= ennemiesToKill && ComportementJoueur.finJeu == false && etageActuel <= gestionPlancher.nombreEtages)
            {
                //Si on vient de tuer le boss
                if (EnemyController.bossTue == true)
                {
                    //Faire apparaître le menu de fin
                    joueur.GetComponent<ComportementJoueur>().FinJeu("Vous avez gagné!", sonVictoire);
                }

                //Sinon, pour le restant des étages
                else if (etageActuel < gestionPlancher.nombreEtages)
                {
                    //Changer de niveau
                    ChangerNiveau();
                } 
            }
        }
    }

    //Fonction qui permet de changer de niveau
    public void ChangerNiveau()
    {
        //Si nous n'avons pas fini
        if(etageActuel <= gestionPlancher.nombreEtages && ComportementJoueur.finJeu == false)
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
            foreach(GameObject tuile in gestionPlancher.plancherActuel)
            {
                Destroy(tuile);
            }

            //Clear la liste du plancher actuel
            gestionPlancher.plancherActuel.Clear();

            //Changer la position actuelle du plancher invisible
            plancherInvisible.transform.Translate(Vector3.down * gestionPlancher.incrementationEtages);

            //Incrémenter l'étage
            gestionPlancher.yEtages -= gestionPlancher.incrementationEtages;
            etageActuel++;

            //Indiquer que nus changeons de couleur de niveau
            gestionPlancher.ChangerCouleur();

            //Afficher le niveau
            AfficherNiveau(etageActuel);

            //Faire spawn des objets
            refSpawnEnnemi.InitialSpawn();
            refSpawnItem.InitialSpawn();

            //Jouer un sound effect
            joueur.gameObject.GetComponent<AudioSource>().PlayOneShot(sonChangerEtage);

            //Dire au joueur que sa suivante zone peut exploser
            joueur.GetComponent<ControleJoueur>().peutExploser = true;

            //Reset le compteur d'ennemis tués
            ComportementJoueur.ennemisTues = 0;

            //Reset et incrémenter le nombre d'ennemis à tuer
            ennemiesToKill += ennemiesToKillMultiplicateur * etageActuel;
            //print("Nombre d'ennemis à tuer: " + ennemiesToKill);

            //Montrer les visuels de la zone d'explosion
            GameObject.Find("ZoneDegats").gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        //Si on est à l'avant dernier niveau, spawn le boss
        if(etageActuel == gestionPlancher.nombreEtages)
        {
            //Instancier le boss
            Instantiate(boss, new Vector3(joueur.transform.position.x, 0, joueur.transform.position.z), Quaternion.identity);

            //Jouer un bruit de boss fight
            GetComponent<AudioSource>().PlayOneShot(bossMusic);
        }
    }

    //Fonction permettant d'afficher à l'écran de nouveau niveau actuel
    public void AfficherNiveau(int niveau)
    {
        //Instancier le prefab UI dans le canvas
        GameObject panneau = Instantiate(UIChangementNiveau);

        //Changer le parent du paneau
        panneau.transform.SetParent(joueur.GetComponent<ComportementJoueur>().ParentLevelUI.transform, false);

        //Indiquer quel niveau qu'il doit afficher
        panneau.GetComponent<AffichageNiveau>().niveau = niveau;
    }
}
