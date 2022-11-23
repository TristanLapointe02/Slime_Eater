using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComportementJoueur : MonoBehaviour
{
    //VIE
    public float vieJoueur; //Vie du joueur
    public float vieMax; //Vie max du joueur
    public Slider sliderVie; //Slider de barre de vie
    public TextMeshProUGUI texteVie; //Ref au texte de vie

    //XP
    public int levelActuel; //Ref au niveau actuel du joueur
    public float xpActuel; //Xp actuel du joueur
    public float xpMax; //Xp max du niveau actuel
    public Slider sliderXp; //Slider de barre de vie
    public TextMeshProUGUI texteXp; //Ref au texte de vie
    public TextMeshProUGUI texteLevelActuel; //Texte du level actuel du joueur

    public static bool mortJoueur; //Detecte si nous sommes mort ou non
    public GameObject menuFin; //Reference au menu de fin
    
    
    public AudioClip sonHit; //Son lorsque le joueur prend des degats
    float velocity; //Velocite pour faire fonctionner le damp

    void Start()
    {
        //Assigner quelques valeurs
        vieJoueur = vieMax;
        velocity = 0;
    }

    void Update()
    {
        //Mettre a jour la valeur du slider de vie
        sliderVie.maxValue = vieMax;
        sliderVie.value = Mathf.SmoothDamp(sliderVie.value, vieJoueur, ref velocity, 150 * Time.deltaTime);

        //Mettre a jour le texte de vie
        texteVie.text = Mathf.RoundToInt(vieJoueur).ToString();

        //TEST, PRENDRE DEGATS
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }

        //TEST, AUGMENTER XP
        if (Input.GetKeyDown(KeyCode.M))
        {

        }
    }

    //Fonction permettant au joueur de prendre des dégâts
    public void TakeDamage(float valeurDegat)
    {
        //Enlever de la vie au joueur
        vieJoueur -= valeurDegat;

        //Si le joueur était pour mourir
        if(vieJoueur <= 0)
        {
            mortJoueur = true;

            //Appeler une fonction affichant le menu de fin
            FinJeu();
        }
    }

    //Fonction permettant de heal le joueur
    public void Heal(float valeurVie)
    {
        //Ajouter de la vie au joueur
        vieJoueur += valeurVie;

        //Si nous avons trop de vie
        if(vieJoueur > vieMax)
        {
            //La mettre a son maximum
            vieJoueur = vieMax;
        }
    }

    //Fonction permettant de grossir le joueur
    public void Grossir(float valeurGrosseur)
    {
        //Augmenter le scale du joueur
        transform.localScale += new Vector3(valeurGrosseur, valeurGrosseur, valeurGrosseur);
    }

    //Fonction permettant d'augmenter l'xp du joueur
    public void IncreaseXp(float valeurXp)
    {
        //Augmenter l'xp actuel du joueur
        xpActuel += valeurXp;

        //Si jamais on dépasse l'xp max
        if(xpActuel >= xpMax)
        {
            //Augmenter de niveau
            levelActuel++;

            //Reset l'xp actuel et l'xp max
            xpActuel = 0;
        }
    }

    public void FinJeu()
    {
        //Faire apparaitre un menu
        menuFin.SetActive(true);
    }
}
