using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public int SpawnAmount; //Nombre d'ennemis a spawn
    public float spawnDelay; //Delai de spawn
    public GameObject[] tableauEnnemis; //Tableau des ennemis
    public List<GameObject> ennemisActuels = new List<GameObject>();
    public Transform[] positionsSpawn; //Positions o� les ennemis peuvent spawn
    public Transform[] positionsYEtages; //Position y des etages
    public int etageActuel; //Etage actuel o� il faut spawn les ennemis
    private bool canSpawn; //Determine si on peut spawn ou non


    void Start()
    {
        //Indiquer que l'on peut spawn au debut
        canSpawn = true;
    }

    void Update()
    {
       //TEST
       if (Input.GetKeyDown(KeyCode.E))
       {
            changerEtage();
       }

       //Si on peut spawn
       if (canSpawn)
       {
            canSpawn = false;

            //Clear la liste des ennemis actuels
            ennemisActuels.Clear();

            //Spawn un ennemi au hasard a une position au hasard
            foreach (GameObject ennemy in tableauEnnemis)
            {
                if(ennemy.GetComponent<EnemyController>().enemy.etage <= etageActuel)
                {
                    //Ajouter les ennemis choisis
                    ennemisActuels.Add(ennemy);
                }
            }

            //Si nous avons un etage de choisi et que notre liste d'ennemis a spawn n'est pas vide
            if(etageActuel > 0 && ennemisActuels.Count > 0)
            {
                //Determiner une position sur l'horizontale
                Transform positionAleatoire = positionsSpawn[Random.Range(0, positionsSpawn.Length)];

                //Determiner la position selon l'etage
                Vector3 positionSpawn = new Vector3(positionAleatoire.position.x, positionsYEtages[etageActuel-1].position.y, positionAleatoire.position.z);

                //Spawn un ennemi
                GameObject ennemiChoisi = Instantiate(ennemisActuels[Random.Range(0, ennemisActuels.Count)].gameObject, positionSpawn, Quaternion.identity);

                Debug.Log("Spawned " + ennemiChoisi.name + " at" + etageActuel + " floor");
            }

            //Commencer le cooldown de spawn
            StartCoroutine(delaiSpawn(spawnDelay));

       }
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
        //Augmenter l'etage actuel
        etageActuel++;

        //Clear la liste d'ennemis possible de spawn
        ennemisActuels.Clear();
    }
}
