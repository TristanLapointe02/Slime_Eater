using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public int SpawnAmount; //Nombre d'items à spawn
    public float spawnDelay; //Delai de spawn
    public GameObject[] tableauItems; //Tableau des items
    public List<GameObject> itemsActuels = new List<GameObject>(); //Liste des items actuels
    public Transform[] positionsSpawn; //Positions où les ennemis peuvent spawn
    public GameObject[] Etages; //Position y des etages
    public static int etageActuel; //Etage actuel où il faut spawn les items
    private bool canSpawn; //Determine si on peut spawn ou non
    public int nombreItemsSpawnPop; //Nombre d'ennemis a spawn en 1 instant

    void Start()
    {
        canSpawn = true;
        etageActuel = 1;

        // Spawner un certain nombre d'items automatiquement au début de la partie
        InitialSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        //Spawner les items
        Spawn();
    }

    //Fonction permettant de donner un delai au spawn
    public IEnumerator delaiSpawn(float delai)
    {
        //Attendre un delai
        yield return new WaitForSeconds(delai);
        canSpawn = true;
    }

    public void changerEtage()
    {

        //Augmenter l'etage actuel
        etageActuel++;

        //Clear la liste d'items possible de spawn
        itemsActuels.Clear();

        // Spawner un nombre d'items initial au changement de l'étage
        InitialSpawn();
    }

    public void Spawn()
    {
        if (canSpawn)
        {
            canSpawn = false;

            // Clear la liste d'items actuels
            itemsActuels.Clear();

            //Spawn un item au hasard à une position au hasard
            foreach (GameObject item in tableauItems)
            {
                if (item.GetComponent<EffetItem>().item.etage <= etageActuel)
                {
                    //Ajouter les items choisis
                    itemsActuels.Add(item);
                }
            }

            //Si nous avons un etage de choisi et que notre liste d'items a spawn n'est pas vide
            if (etageActuel > 0 && itemsActuels.Count > 0)
            {
                //Determiner une position aléatoire sur l'horizontale
                int positionAleatoireX = Random.Range(-100, 100);
                int positionAleatoireY = Random.Range(-100, 100);

                //Determiner la position selon l'etage
                Vector3 positionSpawn = new Vector3(positionAleatoireX, Etages[etageActuel - 1].transform.position.y + 10, positionAleatoireY);

                //Spawn un item
                GameObject itemChoisi = Instantiate(itemsActuels[Random.Range(0, itemsActuels.Count)].gameObject, positionSpawn, Quaternion.identity);

                Debug.Log("Spawned " + itemChoisi.name + " at" + etageActuel + " floor");
            }

            //Commencer le cooldown de spawn
            StartCoroutine(delaiSpawn(spawnDelay));
        }
    }

    //Fonction permettant de spawn des items en un instant
    public void InitialSpawn()
    {
        // Vider la liste d'items actuels
        itemsActuels.Clear();

        //Selon le nombre de items a spawn
        for (int i = 0; i < nombreItemsSpawnPop; i++)
        {
            //Pour chaque potion
            foreach (GameObject item in tableauItems)
            {
                //Si il peut spawn sur cet étage
                if (item.GetComponent<EffetItem>().item.etage <= etageActuel)
                {
                    //Ajouter les items choisis
                    itemsActuels.Add(item);
                }
            }

            // Si nous avons un étage de choisi et que notre liste d'items à spawn n'est pas vide
            if (etageActuel > 0 && etageActuel < Etages.Length && itemsActuels.Count > 0)
            {
                //Determiner une position aléatoire sur l'horizontale
                float positionAleatoireX = Random.Range(positionsSpawn[1].position.x, positionsSpawn[0].position.x);
                float positionAleatoireZ = Random.Range(positionsSpawn[2].position.z, positionsSpawn[3].position.z);

                //Determiner la position selon l'etage
                Vector3 positionSpawn = new Vector3(positionAleatoireX, Etages[etageActuel - 1].transform.position.y + 10, positionAleatoireZ);

                //Spawn un item
                GameObject itemChoisi = Instantiate(itemsActuels[Random.Range(0, itemsActuels.Count)].gameObject, positionSpawn, Quaternion.identity);

                Debug.Log("Spawned " + itemChoisi.name + " at" + etageActuel + " floor");
            }
        }
    }
}
