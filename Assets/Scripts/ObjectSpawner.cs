using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public enum TypesObjet { Ennemi, Item, Zone }; //Liste de types d'objets
    public TypesObjet objetChoisi; //Objet choisi
    public int SpawnAmount; //Nombre d'objets a spawn
    public float spawnDelay; //Delai de spawn
    public int nombreObjetsSpawnPop; //Nombre d'ennemis a spawn en 1 instant
    public int rayonSpawn; //Positions où les objets peuvent spawn
    public int rangeSpawn; //Range a spawn
    public GameObject[] tableauObjets; //Tableau des elements
    private List<GameObject> objetsActuels = new List<GameObject>(); //Liste des objets actuels
    public GameObject[] Etages; //Position y des etages
    private bool canSpawn; //Determine si on peut spawn ou non
    private GameObject joueur; //Ref au joueur
    
    void Start()
    {
        //Trouver le joueur
        joueur = GameObject.FindGameObjectWithTag("Player");

        //Indiquer que l'on peut spawn au debut
        canSpawn = true;

        //Spawn des objets au debut
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

    //Fonction permettant de donner un delai au spawn
    public IEnumerator delaiSpawn(float delai)
    {
        //Attendre un delai
        yield return new WaitForSeconds(delai);
        canSpawn = true;
    }

    //Fonction permettant de spawn un objet à une position aléatoire sur le radius d'un cercle ou a l'intérieur, selon l'item
    public void Spawn()
    {
        //Si on peut spawn
        if (canSpawn)
        {
            //Empêcher immediatement le spawn d'autres objets
            canSpawn = false;

            //Trouver quel ennemi il est possible de spawn
            AjouterObjets();

            //Si on est un ennemi, spawn sur bordure
            if (objetChoisi == TypesObjet.Ennemi)
            {
                //Determiner une position aleatoire sur le périmètre d'un cercle à l'entour du joueur
                var cercle = Random.insideUnitCircle.normalized * rayonSpawn + new Vector2(joueur.transform.position.x, joueur.transform.position.z);
                Vector3 positionSurRadiusCercle = new Vector3(cercle.x, 0, cercle.y);

                //Spawn un objet
                SpawnUnObjet(positionSurRadiusCercle);
            }

            //Sinon, si on est un item, spawn a l'interieur du cercle
            if (objetChoisi == TypesObjet.Item)
            {
                //Determiner une position aleatoire sur le périmètre d'un cercle
                var cercle = Random.insideUnitCircle * rayonSpawn + new Vector2(joueur.transform.position.x, joueur.transform.position.z);
                Vector3 positionDansCercle = new Vector3(cercle.x, 0, cercle.y);

                //Spawn un objet
                SpawnUnObjet(positionDansCercle);
            }

            //Sinon, si on est une zone
            //TODO


            //Commencer le cooldown de spawn
            StartCoroutine(delaiSpawn(spawnDelay));
        }
    }

    //Fonction permettant de spawn des objets en un instant
    public void InitialSpawn()
    {
        //Trouver quel objet il est possible de spawn
        AjouterObjets();

        //Selon le nombre d'objets a spawn
        for (int i = 0; i < nombreObjetsSpawnPop + Random.Range(-rangeSpawn, rangeSpawn); i++)
        {
            //Determiner une position aléatoire dans un cercle
            var cercle = Random.insideUnitCircle * rayonSpawn + new Vector2(joueur.transform.position.x, joueur.transform.position.z);
            Vector3 positionDansCercle = new Vector3(cercle.x, 0, cercle.y);

            //Spawn un objet
            SpawnUnObjet(positionDansCercle);
        }
    }

    //Fonction permettant d'ajouter les bons objets a la liste
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
                //Si il peut spawn sur cet étage
                if (objet.GetComponent<EnemyController>().enemy.etage <= StageProgression.etageActuel)
                {
                    //Ajouter les ennemis choisis à la liste d'objets
                    objetsActuels.Add(objet);
                }
            }
            //Sinon, si on est un item
            else if (objetChoisi == TypesObjet.Item)
            {
                //S'il peut spawn sur cet étage
                if (objet.GetComponent<EffetItem>().item.etage <= StageProgression.etageActuel)
                {
                    //Ajouter les items choisis
                    objetsActuels.Add(objet);
                }
            }
        }
    }

    //Fonction permettant de spawn un objet à une position donnée
    public void SpawnUnObjet(Vector3 position)
    {
        //Pour un item, si la position de spawn dépasse la distance entre le centre de la map et le mur
        if(objetChoisi == TypesObjet.Item && Vector3.Distance(transform.position, position) > rayonSpawn)
        {
            //Sortir de la fonction et rien spawn
            return;
        }

        //Sinon, si le jeu est pas fini et que nous sommes pas en pause
        else if(StageProgression.etageActuel - 1 < Etages.Length && ComportementJoueur.finJeu == false)
        {
            //Determiner la position selon l'etage
            Vector3 positionSpawn = new Vector3(position.x, Etages[StageProgression.etageActuel - 1].transform.position.y + 10, position.z);

            //Spawn un objet
            GameObject nouvelObjet = Instantiate(objetsActuels[Random.Range(0, objetsActuels.Count)].gameObject, positionSpawn, Quaternion.identity);

            //Si nous sommes le spawner d'items, appliquer une rotation (bug fix)
            if (objetChoisi == TypesObjet.Item)
            {
                nouvelObjet.transform.Rotate(-90, 0, 0);
            }
        }
    }
}
