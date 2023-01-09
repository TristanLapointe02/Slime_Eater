using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/*
 * Gestion des am�liorations disponibles pour le joueur
 * Fait par : Tristan Lapointe et Samuel S�guin
 */

public class ControleAmeliorations : MonoBehaviour
{
    public GameObject parentListeAmeliorations; //Parent de la liste des am�liorations
    public GameObject parentAmeliorations; //Parent du choix des am�liorations
    public GameObject parentBoutons; //Parent des boutons d'am�liorations
    public AudioClip sonChoix; //Son lorsque le joueur choisi une am�lioration
    public GameObject carteAmelioration; //Carte vide d'une arme
    public int nombreAmeliorationsProposees; //Nombre d'ameliorations propos�es
    public AudioClip sonSelection; //Son qui joue lorsque les choix son propos�s
    public List<Amelioration> ameliorationsDisponibles = new List<Amelioration>(); //Liste des am�liorations
    private List<Amelioration> ameliorationsSelectionnes = new List<Amelioration>(); //Liste des am�liorations choisies
    public static bool pause; //Indiquer si nous sommes en pause pour le choix
    private int storeAmeliorations; //Variable qui store le nombre  d'am�liorations, s'il y a lieu, suite � un autre level up pendant le choix d'am�liorations

    private void Awake()
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

        //Si nous ne sommes pas en pause
        if (pause == false)
        {
            //Indiquer que nous sommes en pause;
            pause = true;

            //D�marer la coroutine
            StartCoroutine(ProposerChoix(0.65f));

            //Enlever un storage d'am�lioration s'il y a lieu
            if(storeAmeliorations > 0)
            {
                storeAmeliorations--;
            }

            //Pour tous les boutons de modification d'am�lioration
            foreach(Transform child in parentBoutons.transform)
            {
                child.GetComponent<ControleBoutonAmelioration>().VerifierCharge();
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

        //Piger 3 am�liorations
        for (int i = 0; i < nombreAmeliorationsProposees; i++)
        {
            //Attendre un petit delai
            yield return new WaitForSeconds(delai);

            //Choisir une am�lioration
            Amelioration ameliorationAleatoire = ChoisirAmelioration();

            //Instancier une carte avec cette arme
            GameObject carte = Instantiate(carteAmelioration);
            carte.transform.SetParent(parentAmeliorations.transform, false);

            //Lui assigner ses valeurs selon le choix
            carte.GetComponent<CarteAmelioration>().AssignerValeurs(ameliorationAleatoire);

            //Jouer un son de s�lection
            GetComponent<AudioSource>().PlayOneShot(sonSelection);    
        }

        //Un coup les choix propos�s, pour tous les boutons
        foreach (Transform child in parentAmeliorations.transform)
        {
            //Activer les boutons
            child.GetComponent<Button>().interactable = true;
        }
    }

    //Fonction permettant de choisir une am�lioration
    public Amelioration ChoisirAmelioration()
    {
        //Trouver une am�lioration avec laquelle remplacer
        Amelioration ameliorationAleatoire = ameliorationsDisponibles.ElementAt(Random.Range(0, ameliorationsDisponibles.Count));

        //L'ajouter � la liste de ceux s�lectionn�s
        ameliorationsSelectionnes.Add(ameliorationAleatoire);

        //Enlever l'am�lioration choisie de la pool
        ameliorationsDisponibles.Remove(ameliorationAleatoire);

        return ameliorationAleatoire;
    }

    //Fonction permettant de rafra�chir le choix d'am�liorations avec de nouvelles options
    public void RafraichirOptions()
    {
        //Pour chaque carte propos�e
        foreach(Transform child in parentAmeliorations.transform)
        {
            //S'enlever de la liste de ceux choisis
            ameliorationsSelectionnes.Remove(child.GetComponent<CarteAmelioration>().amelioration);

            //Retourner dans la pool
            ameliorationsDisponibles.Add(child.GetComponent<CarteAmelioration>().amelioration);

            //Choisir une am�lioration
            Amelioration ameliorationAleatoire = ChoisirAmelioration();

            //Lui assigner ses valeurs selon le choix
            child.GetComponent<CarteAmelioration>().AssignerValeurs(ameliorationAleatoire);
        }
    }

    //Fonction permettant de tout choisir les am�liorations propos�es
    public void ToutChoisir()
    {
        //Pour chaque carte propos�e
        foreach (Transform child in parentAmeliorations.transform)
        {
            //Ajouter l'am�lioration et ferme les choix
            child.GetComponent<CarteAmelioration>().FermerAmelioration();
        }
    }
    //Fonction permettant de prendre en double l'am�lioration choisie, si possible
    public void ChoisirDouble()
    {
        //Indiquer qu'on peut choisir en double
        foreach (Transform child in parentAmeliorations.transform)
        {
            //Ajouter l'am�lioration et ferme les choix
            child.GetComponent<CarteAmelioration>().pigerDouble = true;
        }
    }
    //Fonction permettant d'ajouter une am�lioration au joueur suite � son choix
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

        //Vider la liste d'ameliorations s�lection�es
        ameliorationsSelectionnes.Clear();

        //Enlever la pause
        pause = false;

        //Selon l'am�lioration, appeler la fonction
        switch (nomAmelioration)
        {
            /**************** AM�LIORATIONS DE JOUEUR ******************/
            case "Soin instantan�":
                GetComponent<ComportementJoueur>().AugmenterVie(GetComponent<ComportementJoueur>().vieMax - GetComponent<ComportementJoueur>().vieJoueur);
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

            case "G�ant �lan":
                GetComponent<ControleJoueur>().dashVitesse += valeur * 1000;
                GetComponent<ControleJoueur>().tailleDash += valeur2;
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

            case "Vampire":
                GetComponent<ComportementJoueur>().vieVampire += valeur;
                break;
        }

        //Ajouter l'am�lioration dans la liste d'am�liorations pr�sentes : 
        //R�f�rence � la liste d'am�liorations du menu Statistiques
        ControleMenu refListe = GetComponent<ControleMenu>();

        // Si la liste est vide, ajouter l'am�lioration � la liste
        if (refListe.listUpgrades.Count == 0)
        {
            refListe.listUpgrades.Add(nomAmelioration);
        }
        // Sinon, regarder si l'upgrade est d�j� dans la liste
        else
        {
            //Mettre le compteur � z�ro
            int completedCount = 0;

            //Parcourir la liste d'am�liorations
            for (int i = 0; i < refListe.listUpgrades.Count; i++)
            {
                //Incr�menter le compteur
                completedCount += 1;

                // Si l'am�lioration est d�j� pr�sente
                if (refListe.listUpgrades[i].Contains(nomAmelioration))
                {
                    //ajouter un "+" � la fin de l'am�lioration dans la liste
                    refListe.listUpgrades[i] += "+";
                    
                    //Arr�ter de parcourir la liste
                    break;
                }

                //Si la liste � �t� enti�rement parcourue
                if (completedCount == refListe.listUpgrades.Count)
                {
                    //Ajouter l'am�lioration � la liste
                    refListe.listUpgrades.Add(nomAmelioration);

                    //Arr�ter de parcourir la liste
                    break;
                }
            }
        }
    }
}
