using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public int SpawnAmount; //Nombre d'ennemis a spawn
    public float spawnDelay; //Delai de spawn
    public GameObject[] tableauEnnemis; //Tableau des ennemis
    public List<GameObject> ennemisActuels = new List<GameObject>(); //Liste des ennemis actuels
    public Transform[] positionsSpawn; //Positions où les ennemis peuvent spawn
    public GameObject[] Etages; //Position y des etages
    public static int etageActuel; //Etage actuel où il faut spawn les ennemis
    private bool canSpawn; //Determine si on peut spawn ou non
    public AudioClip sonChangerEtage; //Son qui joue lorsqu'on change d'etage
    public int nombreEnnemisSpawnPop; //Nombre d'ennemis a spawn en 1 instant

    void Start()
    {
        //Indiquer que l'on peut spawn au debut
        canSpawn = true;

        //Reset l'etage actuel
        etageActuel = 1;

        //Spawn des ennemis au debut
        InitialSpawn();
    }

    void Update()
    {
       //TEST
       if (Input.GetKeyDown(KeyCode.E))
       {
            changerEtage();
       }

        //Spawner les ennemis
        Spawn();
    }

    //Fonction permettant de donner un delai au spawn
    public IEnumerator delaiSpawn(float delai)
    {
        //Attendre un delai
        yield return new WaitForSeconds(delai);
        canSpawn = true;
    }

    //Fonction induqant qu'on vient de finir un etage
    public void changerEtage()
    {
        //Si nous sommes a un etage plus bas que 5
        if(etageActuel < Etages.Length)
        {
            //Detruire le plancher de l'etage
            Etages[etageActuel - 1].gameObject.SetActive(false);

            //Jouer un sound effect
            GameObject.Find("Joueur").gameObject.GetComponent<AudioSource>().PlayOneShot(sonChangerEtage);

            //Augmenter l'etage actuel
            etageActuel++;

            //Clear la liste d'ennemis possible de spawn
            ennemisActuels.Clear();

            // Spawner un nombre d'ennemis initial au changement de l'étage
            InitialSpawn();
        }
    }

    public void Spawn()
    {
        //Si on peut spawn
        if (canSpawn)
        {
            canSpawn = false;

            //Clear la liste des ennemis actuels
            ennemisActuels.Clear();

            //Spawn un ennemi au hasard a une position au hasard
            foreach (GameObject ennemy in tableauEnnemis)
            {
                if (ennemy.GetComponent<EnemyController>().enemy.etage <= etageActuel)
                {
                    //Ajouter les ennemis choisis
                    ennemisActuels.Add(ennemy);
                }
            }

            //Si nous avons un etage de choisi et que notre liste d'ennemis a spawn n'est pas vide
            if (etageActuel > 0 && ennemisActuels.Count > 0)
            {
                //Determiner une position sur l'horizontale
                Transform positionAleatoire = positionsSpawn[Random.Range(0, positionsSpawn.Length)];

                //Determiner la position selon l'etage
                Vector3 positionSpawn = new Vector3(positionAleatoire.position.x, Etages[etageActuel - 1].transform.position.y, positionAleatoire.position.z);

                //Spawn un ennemi
                GameObject ennemiChoisi = Instantiate(ennemisActuels[Random.Range(0, ennemisActuels.Count)].gameObject, positionSpawn, Quaternion.identity);

                //Debug.Log("Spawned " + ennemiChoisi.name + " at" + etageActuel + " floor");
            }

            //Commencer le cooldown de spawn
            StartCoroutine(delaiSpawn(spawnDelay));

        }
    }

    //Fonction permettant de spawn des ennemis en un instant
    public void InitialSpawn()
    {
        //Clear la liste d'ennemis actuels
        ennemisActuels.Clear();

        //Selon le nombre d'ennemis a spawn
        for (int i = 0; i < nombreEnnemisSpawnPop; i++)
        {
            //Pour chaque ennemi
            foreach (GameObject ennemy in tableauEnnemis)
            {
                //Si il peut spawn sur cet étage
                if (ennemy.GetComponent<EnemyController>().enemy.etage <= etageActuel)
                {
                    //Ajouter les ennemis choisis
                    ennemisActuels.Add(ennemy);
                }
            }

            // Si nous avons un étage de choisi et que notre liste d'ennemis à spawn n'est pas vide
            if (etageActuel > 0 && etageActuel < Etages.Length && ennemisActuels.Count > 0)
            {
                //Determiner une position aléatoire sur l'horizontale
                float positionAleatoireX = Random.Range(positionsSpawn[1].position.x, positionsSpawn[0].position.x);
                float positionAleatoireZ = Random.Range(positionsSpawn[2].position.z, positionsSpawn[3].position.z);

                //Determiner la position selon l'etage
                Vector3 positionSpawn = new Vector3(positionAleatoireX, Etages[etageActuel - 1].transform.position.y + 10, positionAleatoireZ);

                //Spawn un ennemi
                GameObject ennemiChoisi = Instantiate(ennemisActuels[Random.Range(0, ennemisActuels.Count)].gameObject, positionSpawn, Quaternion.identity);

                //Debug.Log("Spawned " + ennemiChoisi.name + " at" + etageActuel + " floor");
            }
        }
    }
}
