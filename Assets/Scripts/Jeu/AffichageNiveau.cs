using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AffichageNiveau : MonoBehaviour
{
    public float vitesseDisparition; //Vitesse de fade de l'alpha
    [HideInInspector] public int niveau; //Niveau actuel à afficher
    public TextMeshProUGUI texteNiveau; //Texte de niveau

    private void Start()
    {
        //Changer le texte de niveau
        texteNiveau.text = "Stage " + niveau.ToString();
    }

    void Update()
    {
        //Decrease l'apha avec le temps
        if (GetComponent<CanvasGroup>().alpha > 0)
        {
            //Decrease l'alpha de la barre de vie
            GetComponent<CanvasGroup>().alpha -= vitesseDisparition * Time.deltaTime;
        }
        //Sinon, si nous avons atteint 0
        else if(GetComponent<CanvasGroup>().alpha <= 0)
        {
            //Se détruire
            Destroy(gameObject);
        }
    }
}
