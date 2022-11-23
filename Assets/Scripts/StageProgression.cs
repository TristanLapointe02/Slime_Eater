using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProgression : MonoBehaviour
{
    public int[] ennemiesToKillPerStage; //Nombre d'ennemis a tuer par stage
    public SpawnEnemy refSpawn; //Reference au spawn

    void Update()
    {
        //Regarder sur quel etage qu'on est
        switch (refSpawn.etageActuel)
        {
            case 1:
                if (ComportementJoueur.ennemisTues > ennemiesToKillPerStage[0])
                {
                    refSpawn.changerEtage();
                };
            break;
        }
    }
}
