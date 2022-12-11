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
    public AudioClip sonSelection; //Son qui joue lorsque les choix son propos�s
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

            //Enlever la v�locit� du joueur
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    //Fonction permettant d'offrir des choix aux joueur
    public IEnumerator ProposerChoix(float delai)
    {
        //Activer le parent des choix
        parentAmeliorations.gameObject.SetActive(true);

        //Piger nos am�liorations
        for (int i = 0; i < nombreAmeliorationsProposees; i++)
        {
            //Trouver une am�lioration al�atoire
            Amelioration ameliorationAleatoire = ameliorationsDisponibles.ElementAt(Random.Range(0, ameliorationsDisponibles.Count));

            //L'ajouter � la liste de ceux selectionees
            ameliorationsSelectionnes.Add(ameliorationAleatoire);

            //Enlever l'am�lioration choisie de la piscine
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

    //Fonction d'am�lioration
    public void AjoutAmelioration(string nomAmelioration, float valeur, float valeur2)
    {
        //Jouer un son de s�lection
        GetComponent<AudioSource>().PlayOneShot(sonChoix);

        //D�sactive le parent
        parentAmeliorations.gameObject.SetActive(false);

        //Enlever les constraintes de mouvement au joueur
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        //Enlever ses cartes de choix
        foreach (Transform child in parentAmeliorations.transform)
        {
            Destroy(child.gameObject);
        }

        //Rajouter les choix anciennement propos�s
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

        //Selon l'am�lioration, appeler la fonction
        switch (nomAmelioration)
        {
            /**************** AM�LIORATIONS DE JOUEUR ******************/
            case "Soin instantan�":
                GetComponent<ComportementJoueur>().AugmenterVie(valeur);
            break;

            case "Slime adh�sif":
                StartCoroutine(GetComponent<ComportementJoueur>().AugmenterVitesse(valeur, 0, true));
            break;

            case "Taille d�mesur�e":
                GetComponent<ComportementJoueur>().AugmenterGrosseur(transform.localScale.magnitude * 1.75f);
                GetComponent<ControleTir>().diviseurGrosseurBalle -= GetComponent<ControleTir>().diviseurGrosseurBalle / 2;
                break;

            case "G�nie":
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

            case "Explosion nucl�aire":
                GameObject.Find("ZoneDegats").GetComponent<ZoneDegats>().bonusRayon += valeur;
                break;

            case "En bonne sant�":
                GetComponent<ComportementJoueur>().vieMax += valeur;
                GetComponent<ComportementJoueur>().vieJoueur += valeur;
                break;

            case "Slime aimant�":
                GameObject.Find("Aimant").GetComponent<Aimant>().vitesse += (int)valeur;
                GameObject.Find("Aimant").GetComponent<Aimant>().rayonAimant += (int)valeur;
                break;

            case "Vers l'infini":
                GetComponent<ControleJoueur>().maxJump += (int)valeur;
                break;

            case "Comme un lapin":
                GetComponent<ControleJoueur>().forceSaut += valeur;
                break;

            case "Reg�n�ration am�lior�e":
                GetComponent<ComportementJoueur>().regenVie += valeur;
                break;

            /**************** AM�LIORATIONS DE TIR ******************/

            case "Mitraillette":
                GetComponent<ControleTir>().shootCooldown -= valeur;
                break;

            case "Peau �paisse":
                GetComponent<ControleTir>().valeurPerteTir -= valeur;
                break;

            case "Tir per�ant":
                GetComponent<ControleTir>().peutTirerATravers = true;
                break;

            case "Balles gluantes":
                GetComponent<ControleTir>().peutSlow = true;
                break;

            case "Vitesse de la lumi�re":
                GetComponent<ControleTir>().forceTir += valeur;
                break;

            case "Pluie de slime":
                GetComponent<ControleTir>().nombreBalles += (int)valeur;
                break;

            case "Slime meurtrier":
                GetComponent<ControleTir>().degatsJoueur += valeur;
                break;

            case "Tir arri�re":
                GetComponent<ControleTir>().peutTirerArriere = true;
                break;

            case "Tir lat�ral":
                GetComponent<ControleTir>().peutTirerCotes = true;
                break;
            case "Balles explosives":
                GetComponent<ControleTir>().peutExploser = true;
                break;
        }
    }
}
