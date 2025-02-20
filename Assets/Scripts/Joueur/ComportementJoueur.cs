using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Description : Gestion des statistiques du joueur tels que l'HP, l'XP, les d�g�ts, la vitesse, les visuels, etc...
 * Fait par : Tristan Lapointe et Samuel S�guin
 */

public class ComportementJoueur : MonoBehaviour
{
    [Header("Vie")]
    public float vieJoueur; //Vie du joueur
    public float vieMax; //Vie max du joueur
    public Slider sliderVie; //Slider de barre de vie
    public TextMeshProUGUI texteVie; //R�f�rence au texte de vie
    public float regenVie; //Vie a regen
    public float regenTemps; //Intervalle de temps que le joueur regen de la vie
    public float vieVampire; //Vie reg�n�r�e par l'am�lioration vampire
    public Gradient gradientVie; //Gradient de la couleur de vie

    [Header("Xp")]
    public int levelActuel; //R�f�rence au niveau actuel du joueur
    public float xpActuel; //Xp actuel du joueur
    public float xpMax; //Xp max du niveau actuel
    public float xpMaxLvl1; //Xp max de d�part
    public float multiplicateurXp; //Multiplicateur d'xp apr�s chaque lvl up
    public Slider sliderXp; //Slider de barre de vie
    public TextMeshProUGUI texteXp; //R�f�rence au texte de vie
    public TextMeshProUGUI texteLevelActuel; //Texte du level actuel du joueur
    [HideInInspector] public float bonusXp; //Bonus d'xp
    [HideInInspector] public float bonusTaille; //Bonus de taille
    [HideInInspector] public float armure; //Bonus permettant au joueur de subir moins de d�g�ts

    [Header("Autres References")]
    public int grosseurMax; //Grosseur maximum du joueur
    public int diviseurFallSpeed; //Multiplicateur de fall
    public bool invulnerable; //D�termine si le joueur est invuln�rable ou non
    public static bool mortJoueur; //D�tecte si le joueur est mort ou non
    public GameObject menuFin; //R�f�rence au menu de fin
    public AudioClip sonHit; //Son lorsque le joueur prend des d�g�ts
    public static float ennemisTues; //Nombre d'ennemis tu�s
    public static bool finJeu; //Indiquer que c'est la fin du jeu
    public AudioClip sonLevelUp; //Son lorsque le joueur level up
    public AudioClip sonPartiePerdue; //Son lorsque le joueur perd la partie
    public GameObject ecranEffet; //�cran de d�g�ts
    private bool fixInvulnerabilit�; //Bool permettant de fix l'invuln�rabilit� multiple
    public TextMeshProUGUI texteStage; //Texte affichant le stage actuel
    public Image fillProgression; //Image indiquant la progression du niveau
    [HideInInspector] public GestionSpawnPlancherV3 gestionnairePlancher; //Reference au gestionnaire de plancher
    public GameObject ParentLevelUI; //Parent pour l'affichage du UI (voir StageProgression)
    public TextMeshProUGUI texteLevelUp; //Texte de proposition d'am�liorations
    public string TexteBaseLevel; //Texte de base lorsqu'on level up
    [HideInInspector] public Color couleurJoueur; //Couleur initialle du joueur

    [Header("Couleurs d'effets")]
    public Color couleurDegats;
    public Color couleurHeal;
    public Color couleurInvincible;

    void Start()
    {
        //Assigner les r�f�rences
        gestionnairePlancher = GameObject.FindGameObjectWithTag("GestionnairePlancher").GetComponent<GestionSpawnPlancherV3>();

        //Augmenter le niveau du joueur
        AugmenterXp(xpMax);

        //Assigner quelques valeurs
        vieJoueur = vieMax;
        xpMax = xpMaxLvl1;
        xpActuel = 0;
        ennemisTues = 0;
        finJeu = false;
        mortJoueur = false;
        couleurJoueur = GetComponentInChildren<Renderer>().material.color;

        //D�marer le life regen
        StartCoroutine(RegenVie(regenVie, regenTemps));
    }

    void Update()
    {
        //Mettre a jour la valeur du slider de vie
        float fillValueHp = vieJoueur / vieMax;
        sliderVie.value = fillValueHp;

        //Mettre a jour la couleur
        sliderVie.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = gradientVie.Evaluate(1 - sliderVie.value);

        //Mettre a jour le texte de vie
        texteVie.text = Mathf.FloorToInt(vieJoueur).ToString();

        //Mettre a jour la valeur du slider d'xp
        float fillValueXp = xpActuel / xpMax;
        sliderXp.value = fillValueXp;

        //Mettre a jour le texte d'xp
        texteXp.text = Mathf.FloorToInt(xpActuel).ToString() + "/" + Mathf.FloorToInt(xpMax).ToString();
        texteLevelActuel.text = levelActuel.ToString();

        //Mettre a jour le texte du stage
        texteStage.text = StageProgression.etageActuel.ToString();

        //TEST, PRENDRE DEGATS
        /*if (Input.GetKeyDown(KeyCode.K))
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

        //TEST, AUGMENTER TAILLE
        if (Input.GetKeyDown(KeyCode.G))
        {
            AugmenterGrosseur(5);
        }

        //TEST, AUGMENTER VIE
        if (Input.GetKeyDown(KeyCode.L))
        {
            vieMax += 50;
            vieJoueur += 50;
        }*/
    }

    //Fonction permettant de regen de la vie
    public IEnumerator RegenVie(float amount, float delai)
    {
        //Si on est pas en pause et que le jeu n'est pas fini
        if(ControleAmeliorations.pause == false && ControleMenu.pauseMenu == false && finJeu == false )
        {
            AugmenterVie(amount, true);
        }

        //Attendre un certain d�lai
        yield return new WaitForSeconds(delai);

        //Recommencer la fonction
        StartCoroutine(RegenVie(regenVie, regenTemps));
    }

    //Fonction permettant au joueur de prendre des d�g�ts
    public void TakeDamage(float valeurDegat)
    {
        //Enlever de la vie au joueur
        if(vieJoueur > 0 && invulnerable == false)
        {
            //Enlever la vie
            vieJoueur -= valeurDegat/(1+ armure);

            //Jouer un sound effect
            GetComponent<AudioSource>().PlayOneShot(sonHit);

            //Clamp la valeur entre 0 et 1
            float valeurAlphaEcran = Mathf.Clamp(valeurDegat / 10, 0, 1);

            //Faire appara�tre l'image de d�g�ts
            StartCoroutine(AjoutEcranEffet(couleurDegats, 1f, valeurAlphaEcran));

            //Changer le mat�riel pendant 0.15 secondes
            GetComponent<MeshRenderer>().material.color = Color32.Lerp(GetComponent<MeshRenderer>().material.color, Color.red, 0.85f); ;
            Invoke("AppliquerMat", 0.15f);
        }

        //Si le joueur �tait pour mourir
        if(Mathf.FloorToInt(vieJoueur) <= 0 && mortJoueur == false)
        {
            //Indiquer qu'il est mort
            mortJoueur = true;

            //Mettre sa vie � 0 peu importe
            vieJoueur = 0;

            //Appeler une fonction affichant le menu de fin
            FinJeu("Vous �tes mort", sonPartiePerdue);
        }
    }

    //Fonction permettant d'instancier l'effet de couleur UI
    public IEnumerator AjoutEcranEffet(Color couleurEffet, float duree, float valeurAlpha = 1)
    {
        //Instancier l'�l�ment de UI
        GameObject nouvelEcranEffetUI = Instantiate(ecranEffet);
        nouvelEcranEffetUI.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").gameObject.transform, false);

        //Lui donner une couleur
        nouvelEcranEffetUI.GetComponent<Image>().color = new Color(couleurEffet.r, couleurEffet.g, couleurEffet.b, valeurAlpha);

        //Storer la variable de l'image
        Color couleurEcran = nouvelEcranEffetUI.GetComponent<Image>().color;

        //Diminuer l'opacit� avec le temps
        for (float i = 0; i < 1; i += Time.deltaTime / duree)
        {
            //Changer l'opacit� de la couleur
            var nouvelleCouleur = new Color(couleurEcran.r, couleurEcran.g, couleurEcran.b, Mathf.Lerp(couleurEcran.a, 0, i));
            nouvelEcranEffetUI.GetComponent<Image>().color = nouvelleCouleur;
            yield return null;
        }

        //Enlever l'�l�ment de UI
        Destroy(nouvelEcranEffetUI);
    }

    //Fonction permettant de heal le joueur
    public void AugmenterVie(float valeurVie, bool pasMontrer = false)
    {
        //Si nous sommes plus low que notre max de vie
        if(vieJoueur < vieMax)
        {
            //Ajouter de la vie
            vieJoueur += valeurVie;

            //Ajout � l'�cran un effet, si le healing ne vient pas de regen
            if(pasMontrer == false)
            {
                StartCoroutine(AjoutEcranEffet(couleurHeal, 0.5f));
            }
        }

        //Si nous avons trop de vie
        else if (vieJoueur > vieMax)
        {
            //La mettre a son maximum
            vieJoueur = vieMax;
        }
    }

    //Fonction permettant de grossir le joueur
    public void AugmenterGrosseur(float valeurGrosseur)
    {
        //Si nous sommes pas � notre traille maximale
        if(transform.localScale.magnitude <= grosseurMax)
        {
            //Augmenter la taille du joueur
            transform.localScale += new Vector3(valeurGrosseur + bonusTaille, valeurGrosseur + bonusTaille, valeurGrosseur + bonusTaille);

            //Mettre � jour la force d'explosion
            GetComponent<ControleJoueur>().forceExplosion = GetComponent<ControleJoueur>().forceExplosionInitiale + (transform.localScale.magnitude * GetComponent<ControleJoueur>().multiplicateurForceExplosion);

            //Mettre a jour le rayon de spawn des tuiles
            gestionnairePlancher.rayon = gestionnairePlancher.rayonDeBase + (transform.localScale.magnitude*2);

            //Augmenter le fall multiplicateur
            GetComponent<ControleJoueur>().fallMultiplier += valeurGrosseur / diviseurFallSpeed;
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

            //Changer le texte de level up
            texteLevelUp.text = TexteBaseLevel;
            texteLevelUp.color = Color.white;

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

    //Fonction permettant d'augmenter les d�g�ts du joueur
    public IEnumerator AugmenterDegats(float valeur, float duree)
    {
        //Ajouter les d�g�ts
        GetComponent<ControleTir>().degatsJoueur += valeur;

        //Ajouter un effet a l'ecran
        StartCoroutine(AjoutEcranEffet(couleurDegats, duree));

        //Attendre un certain delai
        yield return new WaitForSeconds(duree);

        //Enlever les d�g�ts
        GetComponent<ControleTir>().degatsJoueur -= valeur;
    }

    //Fonction permettant d'augmenter la hauteur de saut du joueur
    public IEnumerator AugmenterSaut(float valeur, float duree)
    {
        //Ajouter un jump boost
        GetComponent<ControleJoueur>().forceSaut += valeur;

        //Attendre un certain d�lai
        yield return new WaitForSeconds(duree);

        //Enlever la force de saut
        GetComponent<ControleJoueur>().forceSaut -= valeur;
    }

    //Fonction permettant d'augmenter la hauteur de saut du joueur
    public IEnumerator Invulnerabilite(float duree)
    {
        //Si on avait d�j� l'effet
        if(GetComponent<ComportementJoueur>().invulnerable == true)
        {
            fixInvulnerabilit� = true;
        }

        //Indiquer que le joueur est invuln�rable
        GetComponent<ComportementJoueur>().invulnerable = true;

        //Mettre l'�cran de d�g�ts en jaune
        StartCoroutine(AjoutEcranEffet(couleurInvincible, duree));

        //Attendre un certain delai
        yield return new WaitForSeconds(duree);

        //Enlever l'invuln�rabilit�
        if(fixInvulnerabilit� == false)
        {
            GetComponent<ComportementJoueur>().invulnerable = false;
        }
        else
        {
            fixInvulnerabilit� = false;
        }
    }

    //Fonction qui indique que la partie est termin�e
    public void FinJeu(string message, AudioClip son)
    {
        //Indiquer que c'est la fin du jeu
        finJeu = true;

        //Faire appara�tre un menu
        menuFin.SetActive(true);

        //Changer le texte s'affichant dans le menu
        menuFin.transform.Find("MenuFinBg1/Titre").GetComponent<TextMeshProUGUI>().text = message;

        //Jouer un son
        GetComponent<AudioSource>().PlayOneShot(son);

        //Trouver tous les ennemis et les tuer
        EnemyController[] ennemis = GameObject.FindObjectsOfType<EnemyController>();
        foreach(EnemyController ennemi in ennemis)
        {
            ennemi.MortEnnemi();
        }

        //Fermer le menu d'options
        GetComponent<ControleMenu>().FermerOptions();
    }

    //Fonction permettant de remettre le mat�riel du joueur
    public void AppliquerMat()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = couleurJoueur;
    }
}
