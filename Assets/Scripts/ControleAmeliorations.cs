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

    //Fonction permettant d'appeler la coroutine qui propose les choix
    public void ActiverChoix()
    {
        //Demarer la coroutine
        StartCoroutine(ProposerChoix(0.75f));

        //Indiquer que nous sommes en pause;
        pause = true;

        //Enlever la vélocité du joueur
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
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
    public void AjoutAmelioration(string nomAmelioration, float valeur)
    {
        //Jouer un son de sélection
        GetComponent<AudioSource>().PlayOneShot(sonChoix);

        //Désactive le parent
        parentAmeliorations.gameObject.SetActive(false);

        //Enlever les constraintes de mouvement au joueur
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        //Depause
        pause = false;

        //Enlever ses cartes de choix
        foreach (Transform child in parentAmeliorations.transform)
        {
            Destroy(child.gameObject);
        }

        //Rajouter les choix anciennement proposés
        foreach(Amelioration ameliorationChoisie in ameliorationsSelectionnes)
        {
            ameliorationsDisponibles.Add(ameliorationChoisie);
        }

        //Clear la liste d'ameliorations selectionees
        ameliorationsSelectionnes.Clear();

        //Selon l'amélioration, appeler la fonction
        switch (nomAmelioration)
        {
            case "test1":
                GetComponent<ComportementJoueur>().AugmenterVie(valeur);
            break;

            case "test2":
                StartCoroutine(GetComponent<ComportementJoueur>().AugmenterVitesse(valeur, 5));
            break;

            case "test3":
                GetComponent<ComportementJoueur>().AugmenterGrosseur(valeur);
                break;
        }
    }
}
