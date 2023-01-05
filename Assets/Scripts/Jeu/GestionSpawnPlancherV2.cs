using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionSpawnPlancherV2 : MonoBehaviour
{
    public GameObject[] floorTilePrefabs; //Prefab des tuiles du plancher
    public float cellSize = 1f; //Taille d'une tuile
    public int nbRangeeColonnes; //Nombre de rangees et colonnes
    public List<Vector2> points; //Liste des points du graphique
    public float rayonJoueur; //Rayon a l'entour du joueur
    private Transform playerTransform; //Position du joueur

    void Start()
    {
        //Assigner les références
        playerTransform = SpawnJoueur.joueur.transform;

        //Créer la liste
        points = new List<Vector2>();

        //Générer initialement les points du graphique
        GeneratePoints();
    }

    private void Update()
    {
        //Mettre a jour la liste de points a l'entour du joueur
    }

    void GeneratePoints()
    {
        // Iterate over the rows and columns of the grid
        for (int i = -nbRangeeColonnes; i <= nbRangeeColonnes; i++)
        {
            for (int j = -nbRangeeColonnes; j <= nbRangeeColonnes; j++)
            {
                // Calculate the position of the grid cell
                Vector2 pos = new Vector2(i * cellSize, j * cellSize);

                // Add the position to the list of points
                points.Add(pos);
            }
        }
    }
}
