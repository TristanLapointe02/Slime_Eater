using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageProgression : MonoBehaviour
{
    public int[] ennemiesToKillPerStage; //Nombre d'ennemis a tuer par stage
    public SpawnEnemy refSpawnEnnemi; //Reference au spawn d'ennemis
    public SpawnItem refSpawnItem; //Reference au spawn d'items
    public TextMeshProUGUI texteStage; //Texte affichant le stage actuel

    void Update()
    {
        VerificationNiveau();
    }

    //Fonction permettant de vérifier si un etage est terminé
    public void VerificationNiveau()
    {
        //Mettre a jour le texte de niveau
        texteStage.text = SpawnEnemy.etageActuel.ToString();

        for (int i = 0; i < ennemiesToKillPerStage.Length; i++)
        {
            if (SpawnEnemy.etageActuel == i + 1 && ComportementJoueur.ennemisTues >= ennemiesToKillPerStage[i])
            {
                //Changer d'etage
                refSpawnEnnemi.changerEtage();
                refSpawnItem.changerEtage();

                //Reset le compteur d'ennemis tues
                ComportementJoueur.ennemisTues = 0;
            };
        }
    }
}
