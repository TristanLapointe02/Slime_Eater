using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionSpawnPlancherV3 : MonoBehaviour
{
    public GameObject[] prefabsTuiles; //Prefab des tuiles du plancher
    private Transform playerTransform; //Ref au joueur
    private float tailleTuile; //Taille des tuiles
    public float rayonDeBase; //Rayon de spawn de base
    [HideInInspector] public float rayon; //Rayon a l'entour du joueur
    public LayerMask layerMaskCheck; //Layer mask bloquant les tuiles de spawn
    [HideInInspector] public int yEtages; //Valeur y des étages
    public int incrementationEtages; //Valeur d'incrementation de niveau en changeant d'étage
    public int nombreEtages; //Nombre d'etages
    [HideInInspector] public List<GameObject> plancherActuel = new List<GameObject>(); //Tableau storant toutes les tuiles du plancher de l'étage actuel

    void Start()
    {
        //Assigner les références
        playerTransform = SpawnJoueur.joueur.GetComponent<Transform>();

        //Trouver la taille de la tuile selon le premier prefab
        tailleTuile = prefabsTuiles[0].transform.localScale.x;

        //Indiquer la taille de notre rayon initial
        rayon = rayonDeBase;
    }

    void Update()
    {
        //Création du plancher
        SpawnSurCoordonee();
    }

    //Fonction permettant de créer le plancher dynamiquement sur des coordonnées fixes
    public void SpawnSurCoordonee()
    {
        //Initialiser la liste
        List<Vector3> coordinates = new List<Vector3>();

        //Calculer les valeurs minimum et maximum à rechercher
        int minX = Mathf.FloorToInt(playerTransform.position.x - rayon);
        int maxX = Mathf.CeilToInt(playerTransform.position.x + rayon);
        int minZ = Mathf.FloorToInt(playerTransform.position.z - rayon);
        int maxZ = Mathf.CeilToInt(playerTransform.position.z + rayon);

        //Itérer sur les valeurs x et z
        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                //Regarder si les valeurs sont des multiples de la taille des tuiles
                if (x % tailleTuile == 0 && z % tailleTuile == 0)
                {
                    //Calculer la distance entre la coordonnée et le joueur
                    float distance = Vector3.Distance(playerTransform.position, new Vector3(x, yEtages, z));

                    //Si nous sommes bel et bien dans le rayon
                    if (distance <= rayon)
                    {
                        // Add the coordinate to the list
                        Vector3 coordonnee = new Vector3(x, yEtages, z);
                        coordinates.Add(coordonnee);

                        //Si il n'y a pas de tuile
                        Collider[] hitColliders = Physics.OverlapBox(coordonnee, new Vector3(tailleTuile/2.05f, 1, tailleTuile/ 2.05f), Quaternion.identity, layerMaskCheck);
                        if (hitColliders.Length == 0)
                        {
                            //Spawn un prefab d'une tuile
                            GameObject nouvelleTuile = Instantiate(prefabsTuiles[0], coordonnee, Quaternion.identity);

                            //Changer son parent
                            nouvelleTuile.transform.parent = transform;

                            //Ajouter cette tuile dans la liste du plancher actuel
                            plancherActuel.Add(nouvelleTuile);
                        }
                    }
                }
            }
        }
    }
}
