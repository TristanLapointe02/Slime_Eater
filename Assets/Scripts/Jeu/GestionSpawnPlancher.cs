using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionSpawnPlancher : MonoBehaviour
{
    
    public GameObject[] floorTilePrefabs; //The prefab for the floor tile
    private Transform playerTransform; //The player's transform component
    private float spawnZP = 0f; //Axe des z positif
    public float nbTiles; //Nombre de tuiles à avoir sur l'écran
    private float tileLength; //Taille d'une tuile

    private void Start()
    {
        //Assigner les références
        playerTransform = SpawnJoueur.joueur.transform;

        //Indiquer la taille d'une tuile
        tileLength = 5.1f;

        //Spawn les premières tuiles
        for (int i = 0; i < nbTiles; i++)
        {
            SpawnTile(0);
        }
    }

    private void Update()
    {
        //Si la position du joueur est plus grande que les coordonnées enregistrées
        if(playerTransform.position.z > (spawnZP - nbTiles * tileLength))
        {
            SpawnTile(0);
        }
    }

    private void SpawnTile(int prefabIndex)
    {
        //Spawn une tuile
        GameObject nouvelleTuile = Instantiate(floorTilePrefabs[prefabIndex]);

        //Changer son parent
        nouvelleTuile.transform.SetParent(transform);

        //Changer la position de cette nouvelle tuile
        nouvelleTuile.transform.position = new Vector3(0,0,1) * spawnZP;

        //Incrémenter les coordonnées
        spawnZP += tileLength;
    }

    public bool IsDivisible(float x, float n)
    {
        print("Recu: " + x + " et j'ai essayé de le diviser par " + n);

        //Si les deux chiffres divisés donnent aucun restant, ils sont divisibles
        return (x % n) == 0f;
    }
}
