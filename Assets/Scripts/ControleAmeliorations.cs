using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControleAmeliorations : MonoBehaviour
{
    public GameObject parentAmeliorations; //Parent du choix des ameliorations
    public AudioClip sonChoix; //Son lorsque le joueur choisi une amelioration
    public GameObject carteAmelioration; //Carte vide d'une arme
    public int nombreAmeliorationsProposees; //Nombre d'ameliorations proposees
    public AudioClip sonSelection; //Son qui joue lorsque les choix son proposés
    public List<Amelioration> ameliorationsDisponibles = new List<Amelioration>(); //Liste des ameliorations
    private List<Amelioration> ameliorationsSelectionnes = new List<Amelioration>(); //Liste des ameliorations choisies
    public static bool pause; //Indiquer si nous sommes en pause pour le choix

    private void Start()
    {
        //Reset la pause
        pause = false;
    }

    //Fonction permettant d'appeler la coroutine qui propose les choix
    public void ActiverChoix()
    {
        //Si nous sommes pas deja en pause
        if(pause == false)
        {
            //Indiquer que nous sommes en pause;
            pause = true;

            //Demarer la coroutine
            StartCoroutine(ProposerChoix(0.75f));

            //Enlever la vélocité du joueur
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    //Fonction permettant d'offrir des choix aux joueur
    public IEnumerator ProposerChoix(float delai)
    {
        //Activer le parent des choix
        parentAmeliorations.gameObject.SetActive(true);

        //Piger nos améliorations
        for (int i = 0; i < nombreAmeliorationsProposees; i++)
        {
            //Trouver une amélioration aléatoire
            Amelioration ameliorationAleatoire = ameliorationsDisponibles.ElementAt(Random.Range(0, ameliorationsDisponibles.Count));

            //L'ajouter à la liste de ceux selectionees
            ameliorationsSelectionnes.Add(ameliorationAleatoire);

            //Enlever l'amélioration choisie de la piscine
            ameliorationsDisponibles.Remove(ameliorationAleatoire);

            //Instancier une carte avec cette arme
            GameObject carte = Instantiate(carteAmelioration);
            carte.transform.SetParent(parentAmeliorations.transform, false);

            //Lui passer une reference de joueur
            carte.GetComponent<CarteAmelioration>().joueur = gameObject;

            //Lui assigner l'amelioration
            carte.GetComponent<CarteAmelioration>().amelioration = ameliorationAleatoire;

            //Jouer un son de selection
            GetComponent<AudioSource>().PlayOneShot(sonSelection);

            //Attendre un petit delai
            yield return new WaitForSeconds(delai);
        }
    }

    //Fonction d'amélioration
    public void AjoutAmelioration(string nomAmelioration, float valeur, float valeur2)
    {
        //Jouer un son de sélection
        GetComponent<AudioSource>().PlayOneShot(sonChoix);

        //Désactive le parent
        parentAmeliorations.gameObject.SetActive(false);

        //Enlever les constraintes de mouvement au joueur
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        //Enlever ses cartes de choix
        foreach (Transform child in parentAmeliorations.transform)
        {
            Destroy(child.gameObject);
        }

        //Rajouter les choix anciennement proposés
        foreach(Amelioration ameliorationChoisie in ameliorationsSelectionnes)
        {
            //Si on peut
            if (ameliorationChoisie.peutRepiger)
            {
                ameliorationsDisponibles.Add(ameliorationChoisie);
            }
        }

        //Clear la liste d'ameliorations selectionees
        ameliorationsSelectionnes.Clear();

        //Depause
        pause = false;

        //Selon l'amélioration, appeler la fonction
        switch (nomAmelioration)
        {
            /**************** AMÉLIORATIONS DE JOUEUR ******************/
            case "Soin instantané":
                GetComponent<ComportementJoueur>().AugmenterVie(valeur);
            break;

            case "Slime adhésif":
                StartCoroutine(GetComponent<ComportementJoueur>().AugmenterVitesse(valeur, 0, true));
            break;

            case "Taille démesurée":
                GetComponent<ComportementJoueur>().AugmenterGrosseur(transform.localScale.magnitude * 1.75f);
                GetComponent<ControleTir>().diviseurGrosseurBalle -= GetComponent<ControleTir>().diviseurGrosseurBalle / 2;
                break;

            case "Génie":
                GetComponent<ComportementJoueur>().bonusXp += valeur;
            break;

            case "Slime absorbant":
                GetComponent<ComportementJoueur>().bonusTaille += valeur;
            break;

            case "Bouclier invisible":
                GetComponent<ComportementJoueur>().armure += valeur;
            break;

            case "Boom!":
                GetComponent<ControleJoueur>().degatsZone += valeur;
                break;

            case "Explosion nucléaire":
                GameObject.Find("ZoneDegats").GetComponent<ZoneDegats>().bonusRayon += valeur;
                break;

            case "En bonne santé":
                GetComponent<ComportementJoueur>().vieMax += valeur;
                GetComponent<ComportementJoueur>().vieJoueur += valeur;
                break;

            case "Slime aimanté":
                GameObject.Find("Aimant").GetComponent<Aimant>().vitesse += (int)valeur;
                GameObject.Find("Aimant").GetComponent<Aimant>().rayonAimant += (int)valeur;
                break;

            case "Vers l'infini":
                GetComponent<ControleJoueur>().maxJump += (int)valeur;
                break;

            case "Comme un lapin":
                GetComponent<ControleJoueur>().forceSaut += valeur;
                break;

            case "Regénération améliorée":
                GetComponent<ComportementJoueur>().regenVie += valeur;
                break;

            /**************** AMÉLIORATIONS DE TIR ******************/

            case "Mitraillette":
                GetComponent<ControleTir>().shootCooldown -= valeur;
                break;

            case "Peau épaisse":
                GetComponent<ControleTir>().valeurPerteTir -= valeur;
                break;

            case "Tir perçant":
                GetComponent<ControleTir>().peutTirerATravers = true;
                break;

            case "Balles gluantes":
                GetComponent<ControleTir>().peutSlow = true;
                break;

            case "Vitesse de la lumière":
                GetComponent<ControleTir>().forceTir += valeur;
                break;

            case "Pluie de slime":
                GetComponent<ControleTir>().nombreBalles += (int)valeur;
                break;

            case "Slime meurtrier":
                GetComponent<ControleTir>().degatsJoueur += valeur;
                break;

            case "Tir arrière":
                GetComponent<ControleTir>().peutTirerArriere = true;
                break;

            case "Tir latéral":
                GetComponent<ControleTir>().peutTirerCotes = true;
                break;
            case "Balles explosives":
                GetComponent<ControleTir>().peutExploser = true;
                break;
        }
    }
}
