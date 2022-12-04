using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public enum TypesObjet { Ennemi, Item }; //Liste de types d'objets
    public TypesObjet objetChoisi; //Objet choisi
    public int SpawnAmount; //Nombre d'objets a spawn
    public float spawnDelay; //Delai de spawn
    public int nombreObjetsSpawnPop; //Nombre d'ennemis a spawn en 1 instant
    public int rayonSpawn; //Positions où les objets peuvent spawn
    public GameObject[] tableauObjets; //Tableau des elements
    private List<GameObject> objetsActuels = new List<GameObject>(); //Liste des objets actuels
    public GameObject[] Etages; //Position y des etages
    private bool canSpawn; //Determine si on peut spawn ou non

    void Start()
    {
        //Indiquer que l'on peut spawn au debut
        canSpawn = true;

        //Spawn des objets au debut
        InitialSpawn();
    }

    void Update()
    {
        //Spawner les objets si le jeu est en cours
        if(ComportementJoueur.finJeu == false)
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

    //Fonction permettant de spawn un objet à une position aléatoire sur le radius d'un cercle
    public void Spawn()
    {
        //Si on peut spawn
        if (canSpawn)
        {
            //Empêcher immediatement le spawn d'autres objets
            canSpawn = false;

            //Trouver quel ennemi il est possible de spawn
            AjouterObjets();

            //Determiner une position aleatoire sur le périmètre d'un cercle
            var cercle = Random.insideUnitCircle.normalized * rayonSpawn;
            Vector3 positionSurRadiusCercle = new Vector3(cercle.x, 0, cercle.y);

            //Spawn un objet
            SpawnUnObjet(positionSurRadiusCercle);

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
        for (int i = 0; i < nombreObjetsSpawnPop; i++)
        {
            //Determiner une position aléatoire dans un cercle
            var cercle = Random.insideUnitCircle * rayonSpawn;
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
