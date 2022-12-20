using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Gestion du spawn d'objets (ennemis et items)
 * Fait par : Tristan Lapointe et Samuel S�guin
 */

public class ObjectSpawner : MonoBehaviour
{
    public enum TypesObjet { Ennemi, Item, Zone }; //Liste de types d'objets
    public TypesObjet objetChoisi; //Objet choisi
    public int SpawnAmount; //Nombre d'objets � faire spawn
    public float spawnDelay; //D�lai de spawn
    public int nombreObjetsSpawnPop; //Nombre d'ennemis a spawn en m�me temps
    public int rayonSpawn; //Positions o� les objets peuvent spawn
    public int rangeSpawn; //Range a spawn
    public GameObject[] tableauObjets; //Tableau des �l�ments
    private List<GameObject> objetsActuels = new List<GameObject>(); //Liste des objets actuels
    public GameObject[] Etages; //Position y des �tages
    [HideInInspector] public bool canSpawn; //D�termine si on peut spawn ou non
    private GameObject joueur; //R�f�rence au joueur
    
    void Start()
    {
        //Trouver le joueur
        joueur = GameObject.FindGameObjectWithTag("Player");

        //Indiquer que l'on peut spawn au d�but
        canSpawn = true;

        //Spawn des objets au d�but
        InitialSpawn();
    }

    void Update()
    {
        //Spawner les objets si le jeu est en cours
        if(ComportementJoueur.finJeu == false && ControleAmeliorations.pause == false && ControleMenu.pauseMenu == false)
        {
            Spawn();
        }
    }

    //Fonction permettant de donner un d�lai au spawn
    public IEnumerator delaiSpawn(float delai)
    {
        //Attendre un d�lai
        yield return new WaitForSeconds(delai);
        
        //Indiquer qu'il peut spawn � nouveau
        canSpawn = true;
    }

    //Fonction permettant de spawn un objet � une position al�atoire sur le radius d'un cercle ou a l'int�rieur, selon l'objet
    public void Spawn()
    {
        //Si on peut spawn
        if (canSpawn)
        {
            //Emp�cher immediatement le spawn d'autres objets
            canSpawn = false;

            //Trouver quel objet il est possible de spawn
            AjouterObjets();

            //Si on est un ennemi, spawn sur bordure
            if (objetChoisi == TypesObjet.Ennemi)
            {
                //Determiner une position aleatoire sur le p�rim�tre d'un cercle � l'entour du joueur
                var cercle = Random.insideUnitCircle.normalized * rayonSpawn + new Vector2(joueur.transform.position.x, joueur.transform.position.z);
                Vector3 positionSurRadiusCercle = new Vector3(cercle.x, 0, cercle.y);

                //Spawn un objet
                SpawnUnObjet(positionSurRadiusCercle);
            }

            //Sinon, si on est un item, spawn a l'int�rieur du cercle
            if (objetChoisi == TypesObjet.Item)
            {
                //D�terminer une position aleatoire sur le p�rim�tre d'un cercle
                var cercle = Random.insideUnitCircle * rayonSpawn + new Vector2(joueur.transform.position.x, joueur.transform.position.z);
                Vector3 positionDansCercle = new Vector3(cercle.x, 0, cercle.y);

                //Spawn un objet
                SpawnUnObjet(positionDansCercle);
            }

            //Commencer le cooldown de spawn
            StartCoroutine(delaiSpawn(spawnDelay));
        }
    }

    //Fonction permettant de spawn des objets en un instant au d�but de chaque �tage
    public void InitialSpawn()
    {
        //Trouver quel objet il est possible de spawn
        AjouterObjets();

        //Selon le nombre d'objets � spawn
        for (int i = 0; i < nombreObjetsSpawnPop + Random.Range(-rangeSpawn, rangeSpawn); i++)
        {
            //D�terminer une position al�atoire dans un cercle
            var cercle = Random.insideUnitCircle * rayonSpawn + new Vector2(joueur.transform.position.x, joueur.transform.position.z);
            Vector3 positionDansCercle = new Vector3(cercle.x, 0, cercle.y);

            //Spawn un objet
            SpawnUnObjet(positionDansCercle);
        }
    }

    //Fonction permettant d'ajouter les bons objets � la liste
    public void AjouterObjets()
    {
        //Clear la liste d'objets actuels
        objetsActuels.Clear();

        //Trouver quel objet il est possible de spawn
        foreach (GameObject objet in tableauObjets)
        {
            //Selon le type d'objet que nous sommes
            //Si on est un ennemi
            if (objetChoisi == TypesObjet.Ennemi)
            {
                //Si il peut spawn sur cet �tage
                if (objet.GetComponent<EnemyController>().enemy.etage <= StageProgression.etageActuel)
                {
                    //Ajouter les ennemis choisis � la liste d'objets
                    objetsActuels.Add(objet);
                }
            }
            //Sinon, si on est un item
            else if (objetChoisi == TypesObjet.Item)
            {
                //S'il peut spawn sur cet �tage
                if (objet.GetComponent<EffetItem>().item.etage <= StageProgression.etageActuel)
                {
                    //Ajouter les items choisis
                    objetsActuels.Add(objet);
                }
            }
        }
    }

    //Fonction permettant de spawn un objet � une position donn�e
    public void SpawnUnObjet(Vector3 position)
    {
        //Sinon, si la partie n'est pas finie et que nous ne sommes pas en pause
        if(StageProgression.etageActuel - 1 < Etages.Length && ComportementJoueur.finJeu == false)
        {
            //Determiner la position selon l'�tage
            Vector3 positionSpawn = new Vector3(position.x, Etages[StageProgression.etageActuel - 1].transform.position.y + 10, position.z);

            //Spawn un objet
            GameObject nouvelObjet = Instantiate(objetsActuels[Random.Range(0, objetsActuels.Count)].gameObject, positionSpawn, Quaternion.identity);

            //Si nous sommes le spawner d'items, appliquer une rotation
            if (objetChoisi == TypesObjet.Item)
            {
                nouvelObjet.transform.Rotate(-90, 0, 0);
            }
            //Sinon, si c'�tait un ennemi, indiquer qu'on vient de le spawner
            else if (objetChoisi == TypesObjet.Ennemi)
            {
                nouvelObjet.GetComponent<EnemyController>().spawner = GetComponent<ObjectSpawner>();
            }
        }
    }
}
