using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/*
 * Gestion des améliorations disponibles pour le joueur
 * Fait par : Tristan Lapointe et Samuel Séguin
 */

public class ControleAmeliorations : MonoBehaviour
{
    public GameObject parentListeAmeliorations; //Parent de la liste des améliorations
    public GameObject parentAmeliorations; //Parent du choix des améliorations
    public GameObject parentBoutons; //Parent des boutons d'améliorations
    public AudioClip sonChoix; //Son lorsque le joueur choisi une amélioration
    public GameObject carteAmelioration; //Carte vide d'une arme
    public int nombreAmeliorationsProposees; //Nombre d'ameliorations proposées
    public AudioClip sonSelection; //Son qui joue lorsque les choix son proposés
    public List<Amelioration> ameliorationsDisponibles = new List<Amelioration>(); //Liste des améliorations
    private List<Amelioration> ameliorationsSelectionnes = new List<Amelioration>(); //Liste des améliorations choisies
    public static bool pause; //Indiquer si nous sommes en pause pour le choix
    private int storeAmeliorations; //Variable qui store le nombre  d'améliorations, s'il y a lieu, suite à un autre level up pendant le choix d'améliorations

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

            //Démarer la coroutine
            StartCoroutine(ProposerChoix(0.65f));

            //Enlever un storage d'amélioration s'il y a lieu
            if(storeAmeliorations > 0)
            {
                storeAmeliorations--;
            }

            //Pour tous les boutons de modification d'amélioration
            foreach(Transform child in parentBoutons.transform)
            {
                child.GetComponent<ControleBoutonAmelioration>().VerifierCharge();
            }
        } 
    }

    private void Update()
    {
        //Si nous avions storé une amélioration et que le choix est terminé
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

        //Piger 3 améliorations
        for (int i = 0; i < nombreAmeliorationsProposees; i++)
        {
            //Attendre un petit delai
            yield return new WaitForSeconds(delai);

            //Choisir une amélioration
            Amelioration ameliorationAleatoire = ChoisirAmelioration();

            //Instancier une carte avec cette arme
            GameObject carte = Instantiate(carteAmelioration);
            carte.transform.SetParent(parentAmeliorations.transform, false);

            //Lui assigner ses valeurs selon le choix
            carte.GetComponent<CarteAmelioration>().AssignerValeurs(ameliorationAleatoire);

            //Jouer un son de sélection
            GetComponent<AudioSource>().PlayOneShot(sonSelection);    
        }

        //Un coup les choix proposés, pour tous les boutons
        foreach (Transform child in parentAmeliorations.transform)
        {
            //Activer les boutons
            child.GetComponent<Button>().interactable = true;
        }
    }

    //Fonction permettant de choisir une amélioration
    public Amelioration ChoisirAmelioration()
    {
        //Trouver une amélioration avec laquelle remplacer
        Amelioration ameliorationAleatoire = ameliorationsDisponibles.ElementAt(Random.Range(0, ameliorationsDisponibles.Count));

        //L'ajouter à la liste de ceux sélectionnés
        ameliorationsSelectionnes.Add(ameliorationAleatoire);

        //Enlever l'amélioration choisie de la pool
        ameliorationsDisponibles.Remove(ameliorationAleatoire);

        return ameliorationAleatoire;
    }

    //Fonction permettant de rafraîchir le choix d'améliorations avec de nouvelles options
    public void RafraichirOptions()
    {
        //Pour chaque carte proposée
        foreach(Transform child in parentAmeliorations.transform)
        {
            //S'enlever de la liste de ceux choisis
            ameliorationsSelectionnes.Remove(child.GetComponent<CarteAmelioration>().amelioration);

            //Retourner dans la pool
            ameliorationsDisponibles.Add(child.GetComponent<CarteAmelioration>().amelioration);

            //Choisir une amélioration
            Amelioration ameliorationAleatoire = ChoisirAmelioration();

            //Lui assigner ses valeurs selon le choix
            child.GetComponent<CarteAmelioration>().AssignerValeurs(ameliorationAleatoire);
        }
    }

    //Fonction permettant de tout choisir les améliorations proposées
    public void ToutChoisir()
    {
        //Pour chaque carte proposée
        foreach (Transform child in parentAmeliorations.transform)
        {
            //Ajouter l'amélioration et ferme les choix
            child.GetComponent<CarteAmelioration>().FermerAmelioration();
        }
    }
    //Fonction permettant de prendre en double l'amélioration choisie, si possible
    public void ChoisirDouble()
    {
        //Indiquer qu'on peut choisir en double
        foreach (Transform child in parentAmeliorations.transform)
        {
            //Ajouter l'amélioration et ferme les choix
            child.GetComponent<CarteAmelioration>().pigerDouble = true;
        }
    }
    //Fonction permettant d'ajouter une amélioration au joueur suite à son choix
    public void AjoutAmelioration(string nomAmelioration, float valeur, float valeur2)
    {
        //Jouer un son de sélection
        GetComponent<AudioSource>().PlayOneShot(sonChoix);

        //Désactive le parent
        parentListeAmeliorations.gameObject.SetActive(false);

        //Enlever ses cartes de choix
        foreach (Transform child in parentAmeliorations.transform)
        {
            Destroy(child.gameObject);
        }

        //Rajouter les choix anciennement proposés
        foreach (Amelioration ameliorationNonChoisie in ameliorationsSelectionnes)
        {
            //Si on peut
            if (ameliorationNonChoisie.peutRepiger)
            {
                ameliorationsDisponibles.Add(ameliorationNonChoisie);
            }
        }

        //Vider la liste d'ameliorations sélectionées
        ameliorationsSelectionnes.Clear();

        //Enlever la pause
        pause = false;

        //Selon l'amélioration, appeler la fonction
        switch (nomAmelioration)
        {
            /**************** AMÉLIORATIONS DE JOUEUR ******************/
            case "Soin instantané":
                GetComponent<ComportementJoueur>().AugmenterVie(GetComponent<ComportementJoueur>().vieMax - GetComponent<ComportementJoueur>().vieJoueur);
                break;

            case "Slime adhésif":
                StartCoroutine(GetComponent<ComportementJoueur>().AugmenterVitesse(valeur, 0, true));
                break;

            case "Taille démesurée":
                GetComponent<ComportementJoueur>().AugmenterGrosseur(transform.localScale.magnitude * valeur);
                GetComponent<ControleTir>().diviseurGrosseurBalle -= GetComponent<ControleTir>().diviseurGrosseurBalle / valeur2;
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
                GetComponent<ControleJoueur>().forceExplosionInitiale += valeur2;
                GetComponent<ComportementJoueur>().AugmenterGrosseur(0);
                break;

            case "En bonne santé":
                GetComponent<ComportementJoueur>().vieMax += valeur;
                GetComponent<ComportementJoueur>().vieJoueur += valeur;
                break;

            case "Slime aimanté":
                GameObject.Find("Aimant").GetComponent<Aimant>().vitesse += valeur;
                GameObject.Find("Aimant").GetComponent<Aimant>().rayonAimant += valeur;
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

            case "Géant élan":
                GetComponent<ControleJoueur>().dashVitesse += valeur * 1000;
                GetComponent<ControleJoueur>().tailleDash += valeur2;
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

            case "Vampire":
                GetComponent<ComportementJoueur>().vieVampire += valeur;
                break;
        }

        //Ajouter l'amélioration dans la liste d'améliorations présentes : 
        //Référence à la liste d'améliorations du menu Statistiques
        ControleMenu refListe = GetComponent<ControleMenu>();

        // Si la liste est vide, ajouter l'amélioration à la liste
        if (refListe.listUpgrades.Count == 0)
        {
            refListe.listUpgrades.Add(nomAmelioration);
        }
        // Sinon, regarder si l'upgrade est déjà dans la liste
        else
        {
            //Mettre le compteur à zéro
            int completedCount = 0;

            //Parcourir la liste d'améliorations
            for (int i = 0; i < refListe.listUpgrades.Count; i++)
            {
                //Incrémenter le compteur
                completedCount += 1;

                // Si l'amélioration est déjà présente
                if (refListe.listUpgrades[i].Contains(nomAmelioration))
                {
                    //ajouter un "+" à la fin de l'amélioration dans la liste
                    refListe.listUpgrades[i] += "+";
                    
                    //Arrêter de parcourir la liste
                    break;
                }

                //Si la liste à été entièrement parcourue
                if (completedCount == refListe.listUpgrades.Count)
                {
                    //Ajouter l'amélioration à la liste
                    refListe.listUpgrades.Add(nomAmelioration);

                    //Arrêter de parcourir la liste
                    break;
                }
            }
        }
    }
}
