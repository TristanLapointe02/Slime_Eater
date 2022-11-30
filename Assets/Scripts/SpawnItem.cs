using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public int SpawnAmount; //Nombre d'items à spawn
    public float spawnDelay; //Delai de spawn
    public GameObject[] tableauItems; //Tableau des items
    public List<GameObject> itemsActuels = new List<GameObject>();
    public GameObject[] Etages; //Position y des etages
    public static int etageActuel; //Etage actuel où il faut spawn les items
    private bool canSpawn; //Determine si on peut spawn ou non

    // Start is called before the first frame update
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
        //TEST
        if (Input.GetKeyDown(KeyCode.E))
        {
            changerEtage();
        }

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

    public void InitialSpawn()
    {
        // Vider la liste d'items actuels
        itemsActuels.Clear();

        // Faire spawner 5 items a des positions au hasard
        for (int i = 0; i < 5; i++)
        {
            foreach (GameObject item in tableauItems)
            {
                if (item.GetComponent<EffetItem>().item.etage <= etageActuel)
                {
                    //Ajouter les items choisis
                    itemsActuels.Add(item);
                }
            }

            // Si nous avons un étage de choisi et que notre liste d'items à spawn n'est pas vide
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
        }
    }
}
