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
    public float regenVie; //Vie a regen
    public float regenTemps; //Intervalle de temps que le joueur regen de la vie
    public float vieVampire; //Vie reg�n�r�e de l'am�lioration vampire

    //XP
    public int levelActuel; //Ref au niveau actuel du joueur
    public float xpActuel; //Xp actuel du joueur
    public float xpMax; //Xp max du niveau actuel
    public float xpMaxLvl1; //Xp max de d�part
    public float multiplicateurXp; //Multiplicateur d'xp apr�s chaque lvl up
    public Slider sliderXp; //Slider de barre de vie
    public TextMeshProUGUI texteXp; //Ref au texte de vie
    public TextMeshProUGUI texteLevelActuel; //Texte du level actuel du joueur

    //AUTRES
    public bool invulnerable; //Determine si le joueur est invuln�rable ou non
    public static bool mortJoueur; //Detecte si nous sommes mort ou non
    public GameObject menuFin; //Reference au menu de fin
    public AudioClip sonHit; //Son lorsque le joueur prend des degats
    public static float ennemisTues; //Nombre d'ennemis tues
    public static bool finJeu; //Indiquer que c'est la fin du jeu
    public AudioClip sonLevelUp; //Son lorsque le joueur level up
    public AudioClip sonPartiePerdue; //Son lorsque le joueur perd la partie
    public float rayonSpawn; //Rayon dans lequel le joueur peut spawn au d�but du jeu
    public Image ecranDegats; //Ecran de degats
    private bool ecranDegatsActif; //Indique si l'�cran de d�g�ts est actif

    //BONUS AM�LIORATIONS
    public float bonusXp; //Bonus d'xp
    public float bonusTaille; //Bonus de taille
    public float armure; //Bonus permettant au joueur de subir moins de d�g�ts

    void Start()
    {
        //Augmenter le niveau du joueur
        AugmenterXp(xpMax);

        //Se d�placer � une positon al�atoire sur la map
        //Trouver une position al�atoire
        Vector3 positionAleatoire = Random.insideUnitCircle * rayonSpawn;
        transform.position = new Vector3(positionAleatoire.x, 35, positionAleatoire.z);

        //Assigner quelques valeurs
        vieJoueur = vieMax;
        xpMax = xpMaxLvl1;
        xpActuel = 0;
        ennemisTues = 0;
        finJeu = false;
        mortJoueur = false;

        //D�marer le life regen
        StartCoroutine(RegenVie(regenVie, regenTemps));
    }

    void Update()
    {
        //Mettre a jour la valeur du slider de vie
        float fillValueHp = vieJoueur / vieMax;
        sliderVie.value = fillValueHp;

        //Mettre a jour le texte de vie
        texteVie.text = Mathf.FloorToInt(vieJoueur).ToString();

        //Mettre a jour la valeuyr du slider d'xp
        float fillValueXp = xpActuel / xpMax;
        sliderXp.value = fillValueXp;

        //Mettre a jour le texte d'xp
        texteXp.text = Mathf.FloorToInt(xpActuel).ToString() + " / " + Mathf.FloorToInt(xpMax).ToString();
        texteLevelActuel.text = levelActuel.ToString();

        //TEST, PRENDRE DEGATS
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }

        //TEST, AUGMENTER XP
        if (Input.GetKeyDown(KeyCode.M))
        {
            AugmenterXp(5000);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            AugmenterXp(5);
        }
    }

    //Fonction permettant de regen de la vie
    public IEnumerator RegenVie(float amount, float delai)
    {
        //Si on est pas en pause et que le jeu n'est pas fini
        if(ControleAmeliorations.pause == false && finJeu == false)
        {
            AugmenterVie(amount);
        }

        yield return new WaitForSeconds(delai);

        StartCoroutine(RegenVie(regenVie, regenTemps));
    }

    //Fonction permettant au joueur de prendre des d�g�ts
    public void TakeDamage(float valeurDegat)
    {
        //Jouer un sound effect
        GetComponent<AudioSource>().PlayOneShot(sonHit);

        //Enlever de la vie au joueur
        if(vieJoueur > 0 && invulnerable == false)
        {
            vieJoueur -= valeurDegat/(1+ armure);

            //Faire appara�tre l'image de d�g�ts
            ecranDegats.color = new Color(0.75f, 0.1f, 0.1f, 1);
            StartCoroutine(FadeImage(valeurDegat));
        }

        //Si le joueur �tait pour mourir
        if(vieJoueur <= 0 && mortJoueur == false)
        {
            //Indiquer qu'il est mort
            mortJoueur = true;

            //Set sa vie � 0 peu importe
            vieJoueur = 0;

            //Appeler une fonction affichant le menu de fin
            FinJeu("Vous �tes mort.", sonPartiePerdue);
        }
    }

    //Fonction permettant de fade l'image de degats
    private IEnumerator FadeImage(float valeurDegats)
    {
        //Clamp la valeur entre 0 et 1
        float valeurAlphaEcran = Mathf.Clamp(valeurDegats/10, 0, 1);

        //Selon la force des d�g�ts
        for (float i = valeurAlphaEcran; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            ecranDegats.color = new Color(0.75f, 0.1f, 0.1f, i);
            yield return null;
        }
    }

    //Fonction permettant de heal le joueur
    public void AugmenterVie(float valeurVie)
    {
        //Ajouter de la vie au joueur
        vieJoueur += valeurVie;

        //Si nous avons trop de vie
        if (vieJoueur > vieMax)
        {
            //La mettre a son maximum
            vieJoueur = vieMax;
        }
    }

    //Fonction permettant de grossir le joueur
    public void AugmenterGrosseur(float valeurGrosseur)
    {
        //Augmenter le scale du joueur
        if(transform.localScale.magnitude <= 35)
        {
            transform.localScale += new Vector3(valeurGrosseur + bonusTaille, valeurGrosseur + bonusTaille, valeurGrosseur + bonusTaille);

            //Mettre a jour la force d'explosion
            GetComponent<ControleJoueur>().forceExplosion = GetComponent<ControleJoueur>().forceExplosionInitiale + (transform.localScale.magnitude * GetComponent<ControleJoueur>().multiplicateurForceExplosion);
        }
    }

    //Fonction permettant d'augmenter l'xp du joueur
    public void AugmenterXp(float valeurXp)
    {
        //Augmenter l'xp actuel du joueur
        xpActuel += valeurXp + bonusXp;

        //Si jamais on d�passe l'xp max
        if(Mathf.FloorToInt(xpActuel) >= Mathf.FloorToInt(xpMax))
        {
            //Trouver la difference si on depasse l'xp max
            float difference = xpActuel - xpMax;

            //Reset l'xp actuel
            xpActuel = 0;

            //Ajouter la difference
            if (difference > 0)
            {
                xpActuel += difference;
            }

            //Augmenter de niveau
            levelActuel++;

            //Augmenter l'xp max
            xpMax += xpMax / multiplicateurXp;

            //Jouer un sound effect
            GetComponent<AudioSource>().PlayOneShot(sonLevelUp);

            //Proposer une am�lioration
            GetComponent<ControleAmeliorations>().ActiverChoix();
        }
    }

    //Fonction permettant d'augmenter la vitesse du joueur
    public IEnumerator AugmenterVitesse(float valeur, float duree = 0f, bool permanent = false)
    {
        //Ajouter la vitesse
        GetComponent<ControleJoueur>().vitesse += valeur;

        //Si c'est temporaire
        if(permanent == false)
        {
            //Attendre un certain delai
            yield return new WaitForSeconds(duree);

            //Enlever la vitesse
            GetComponent<ControleJoueur>().vitesse -= valeur;
        } 
    }

    //Fonction permettant d'augmenter les degats du joueur
    public IEnumerator AugmenterDegats(float valeur, float duree)
    {
        //Ajouter les degats
        GetComponent<ControleTir>().degatsJoueur += valeur;

        //Attendre un certain delai
        yield return new WaitForSeconds(duree);

        //Enlever les degats
        GetComponent<ControleTir>().degatsJoueur -= valeur;
    }

    //Fonction permettant d'augmenter la hauteur de saut du joueur
    public IEnumerator AugmenterSaut(float valeur, float duree)
    {
        //Ajouter un jump boost
        GetComponent<ControleJoueur>().forceSaut += valeur;

        //Attendre un certain delai
        yield return new WaitForSeconds(6);

        //Enlever la force de saut
        GetComponent<ControleJoueur>().forceSaut -= valeur;
    }

    //Fonction permettant d'augmenter la hauteur de saut du joueur
    public IEnumerator Invulnerabilite(float duree)
    {
        //Indiquer que le joueur est invulnerable
        GetComponent<ComportementJoueur>().invulnerable = true;

        //Mettre l'ecran de degats en jaune
        ecranDegats.color = new Color(0.5f, 0.5f, 0.1f, 1f);

        //Attendre un certain delai
        yield return new WaitForSeconds(duree);

        //Effacer l'ecran de degats
        ecranDegats.color = new Color(1f, 1f, 1f, 0f);

        //Enlever l'invulnerabilite
        GetComponent<ComportementJoueur>().invulnerable = false;
    }

    //Fonction qui indique que la partie est termin�e
    public void FinJeu(string message, AudioClip son)
    {
        //Faire apparaitre un menu
        menuFin.SetActive(true);

        //Changer le texte s'affichant dans le menu
        menuFin.transform.Find("MenuFinBg1/Titre").GetComponent<TextMeshProUGUI>().text = message;

        //Jouer un son
        GetComponent<AudioSource>().PlayOneShot(son);

        //Enlever la v�locit� du joueur
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        //Indiquer que c'est la fin du jeu
        finJeu = true;
    }
}
