using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ControleAmeliorations : MonoBehaviour
{
    public GameObject parentListeAmeliorations; //Parent de la liste des am�liorations
    public GameObject parentAmeliorations; //Parent du choix des ameliorations
    public AudioClip sonChoix; //Son lorsque le joueur choisi une amelioration
    public GameObject carteAmelioration; //Carte vide d'une arme
    public int nombreAmeliorationsProposees; //Nombre d'ameliorations proposees
    public AudioClip sonSelection; //Son qui joue lorsque les choix son propos�s
    public List<Amelioration> ameliorationsDisponibles = new List<Amelioration>(); //Liste des ameliorations
    private List<Amelioration> ameliorationsSelectionnes = new List<Amelioration>(); //Liste des ameliorations choisies
    public static bool pause; //Indiquer si nous sommes en pause pour le choix
    private int storeAmeliorations; //Variable storant le nombre  d'am�liorations, s'il y a lieu, suite � un autre level up pendant le choix d'am�liorations
    private void Start()
    {
        //Reset la pause
        pause = false;
    }

    //Fonction permettant d'appeler la coroutine qui propose les choix
    public void ActiverChoix()
    {
        //Si nous sommes en pause, storer qu'on a level up
        if (pause == true)
        {
            storeAmeliorations++;
        }

        //Si nous sommes pas en pause
        if (pause == false)
        {
            //Indiquer que nous sommes en pause;
            pause = true;

            //Demarer la coroutine
            StartCoroutine(ProposerChoix(0.55f));

            //Enlever la v�locit� du joueur
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            //Enlever un storage d'am�lioration s'il y a lieu
            if(storeAmeliorations > 0)
            {
                storeAmeliorations--;
            }
        } 
    }

    private void Update()
    {
        //Si nous avions stor� une am�lioration et que le choix est termin�
        if(storeAmeliorations > 0 && pause == false)
        {
            ActiverChoix();
        }
    }

    //Fonction permettant d'offrir des choix aux joueur
    public IEnumerator ProposerChoix(float delai)
    {
        //Activer le parent des choix
        parentListeAmeliorations.gameObject.SetActive(true);

        //Piger nos am�liorations
        for (int i = 0; i < nombreAmeliorationsProposees; i++)
        {
            //Attendre un petit delai
            yield return new WaitForSeconds(delai);

            //Trouver une am�lioration al�atoire
            Amelioration ameliorationAleatoire = ameliorationsDisponibles.ElementAt(Random.Range(0, ameliorationsDisponibles.Count));

            //L'ajouter � la liste de ceux selectionees
            ameliorationsSelectionnes.Add(ameliorationAleatoire);

            //Enlever l'am�lioration choisie de la piscine
            ameliorationsDisponibles.Remove(ameliorationAleatoire);

            //Instancier une carte avec cette arme
            GameObject carte = Instantiate(carteAmelioration);
            carte.transform.SetParent(parentAmeliorations.transform, false);

            //Disable l'int�ractivit�
            carte.GetComponent<Button>().enabled = false;

            //Lui passer une reference de joueur
            carte.GetComponent<CarteAmelioration>().joueur = gameObject;

            //Lui assigner l'amelioration
            carte.GetComponent<CarteAmelioration>().amelioration = ameliorationAleatoire;

            //Jouer un son de selection
            GetComponent<AudioSource>().PlayOneShot(sonSelection);    
        }

        //Un coup les choix propos�s, enable les boutons
        foreach (Transform boutonAmelioration in parentAmeliorations.transform)
        {
            boutonAmelioration.GetComponent<Button>().enabled = true;
        }
    }

    //Fonction d'am�lioration
    public void AjoutAmelioration(string nomAmelioration, float valeur, float valeur2)
    {
        //Jouer un son de s�lection
        GetComponent<AudioSource>().PlayOneShot(sonChoix);

        //D�sactive le parent
        parentListeAmeliorations.gameObject.SetActive(false);

        //Enlever ses cartes de choix
        foreach (Transform child in parentAmeliorations.transform)
        {
            Destroy(child.gameObject);
        }

        //Rajouter les choix anciennement propos�s
        foreach (Amelioration ameliorationNonChoisie in ameliorationsSelectionnes)
        {
            //Si on peut
            if (ameliorationNonChoisie.peutRepiger)
            {
                ameliorationsDisponibles.Add(ameliorationNonChoisie);
            }
        }

        //Clear la liste d'ameliorations selectionees
        ameliorationsSelectionnes.Clear();

        //Enlever les constraintes de mouvement au joueur
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

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
                GetComponent<ComportementJoueur>().AugmenterGrosseur(transform.localScale.magnitude * valeur);
                GetComponent<ControleTir>().diviseurGrosseurBalle -= GetComponent<ControleTir>().diviseurGrosseurBalle / valeur2;
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
                GetComponent<ControleJoueur>().forceExplosionInitiale += valeur2;
                GetComponent<ComportementJoueur>().AugmenterGrosseur(0);
                break;

            case "En bonne sant�":
                GetComponent<ComportementJoueur>().vieMax += valeur;
                GetComponent<ComportementJoueur>().vieJoueur += valeur;
                break;

            case "Slime aimant�":
                GameObject.Find("Aimant").GetComponent<Aimant>().vitesse += valeur;
                GameObject.Find("Aimant").GetComponent<Aimant>().rayonAimant += valeur;
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

        //Ajouter l'am�lioration dans la liste d'am�liorations pr�sentes
        ControleMenu refListe = GetComponent<ControleMenu>();
        // Si la liste est vide, ajouter l'am�lioration � la liste
        // Sinon, regarder si l'upgrade est d�j� dans la liste
        if (refListe.listUpgrades.Count == 0)
        {
            refListe.listUpgrades.Add(nomAmelioration);
        }
        else
        {
            int completedCount = 0;
            int totalCount = refListe.listUpgrades.Count;
            for (int i = 0; i < refListe.listUpgrades.Count; i++)
            {
                completedCount += 1;
                // Si l'am�lioration est d�j� pr�sente, ajouter un "+" � la fin
                if (refListe.listUpgrades[i].Contains(nomAmelioration))
                {
                    refListe.listUpgrades[i] += "+";
                    break;
                }
                if (completedCount == totalCount)
                {
                    refListe.listUpgrades.Add(nomAmelioration);
                    break;
                }
            }
        }
    }
}
