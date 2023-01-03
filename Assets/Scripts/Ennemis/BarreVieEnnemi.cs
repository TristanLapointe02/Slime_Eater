using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/*
 * Description : Gère le display de la vie de l'ennemi
 * Fait par : Tristan Lapointe et Samuel Séguin
 */
public class BarreVieEnnemi : MonoBehaviour
{
    private Image barreVie; //Reference a l'image de la barre de vie
    private Canvas canvas; //Reference au canvas
    private Camera cam; //Reference a la camera
    private CanvasGroup canvasAlpha; //Canvas group contrôlant l'alpha de la barre de vie
    private GameObject joueur; //Ref au joueur
    [SerializeField] private float vitesseReduction; //Vitesse de réduction de la barre de vie
    [SerializeField] private float vitesseDisparition; //Vitesse de disparition de la barre de vie
    private float target = 1; //Target de l'animation de la barre de vie

    void Start()
    {
        //Assigner les références
        canvas = GetComponentInChildren<Canvas>();
        canvasAlpha = canvas.transform.Find("Background").GetComponent<CanvasGroup>();
        barreVie = canvasAlpha.transform.Find("Foreground").GetComponent<Image>();
        cam = Camera.main;
        joueur = SpawnJoueur.joueur;
    }

    void Update()
    {
        //Positionner correctement la barre de vie selon la taille du monstre
        canvas.transform.position = new Vector3(transform.position.x, transform.position.y + GetComponent<Collider>().bounds.size.y, transform.position.z);

        //Regarder la caméra
        canvas.transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        //Mettre a jour la progression de la barre de vie
        barreVie.fillAmount = Mathf.MoveTowards(barreVie.fillAmount, target, vitesseReduction * Time.deltaTime);

        //Si l'alpha de la barre de vie est pas déjà à 0, et que la barre de vie n'est pas en mouvement
        if(canvasAlpha.alpha > 0 && barreVie.fillAmount == target)
        {
            //Decrease l'alpha de la barre de vie
            canvasAlpha.alpha -= vitesseDisparition * Time.deltaTime;
        }

        //Montrer la barre de vie si nous sommes en train de hover l'ennemi avec la souris
        //Si nous avons présentement un curseur souris
        if (joueur.GetComponent<GamePadCursor>().playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            VerifPositionSouris(Mouse.current.position.ReadValue());
        }

        //Sinon, si nous avons le curseur de gamepad
        else if (joueur.GetComponent<GamePadCursor>().playerInput.currentControlScheme == "Gamepad")
        {
            VerifPositionSouris(joueur.GetComponent<GamePadCursor>().cursorTransform.position);
        }
    }

    //Fonction permettant de mettre à jour la barre de vie de l'ennemi
    public void MajBarreVie(float vieActuelle, float vieMax, bool montrerBarre = true)
    {
        //Montrer la barre de vie si nous sommes pas déja au max
        if(vieActuelle != vieMax && montrerBarre)
        {
            canvasAlpha.alpha = 1;
        }

        //Mettre a jour le target de la barre de vie
        target = vieActuelle / vieMax;
    }

    //Fonction permettant de vérifier la position de la souris
    public void VerifPositionSouris(Vector3 position)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(position);

        //Si nous touchons un ennemi
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ennemi")))
        {
            //Si c'est le collider de cet ennemi
            if (hit.collider == GetComponent<Collider>())
            {
                canvasAlpha.alpha = 1;
            }
        }
    }
}
