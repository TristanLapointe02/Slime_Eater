using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Description : Faire que l'objet gun se déplace et tourne correctement selon la position du joueur et de la souris
 * Fait par : Tristan Lapointe
 */

public class GunFollowPlayer : MonoBehaviour
{
    private GameObject joueur; //Référence au joueur
    public LayerMask layersPlancher; //Layers du plancher
    private void Start()
    {
        joueur = SpawnJoueur.joueur;
    }

    void Update()
    {
        //Si le joueur existe dans la scène
        if(joueur != null)
        {
            //Suivre la position du joueur, mais toujours se placer à ses pieds, peu importe sa taille
            transform.position = new Vector3(joueur.transform.position.x, joueur.transform.position.y - joueur.GetComponent<Collider>().bounds.size.y / 2.25f , joueur.transform.position.z);
        }

        //Si nous avons présentement un curseur souris
        if (joueur.GetComponent<GamePadCursor>().playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            TracerRayon(Mouse.current.position.ReadValue());
        }

        //Sinon, si nous avons le curseur de gamepad
        else if (joueur.GetComponent<GamePadCursor>().playerInput.currentControlScheme == "Gamepad")
        {
            TracerRayon(joueur.GetComponent<GamePadCursor>().cursorTransform.position);
        } 
    }

    public void TracerRayon(Vector3 traceur)
    {
        //Changer la rotation du gun selon la position de la souris
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(traceur);

        //Si nous touchons le sol
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layersPlancher))
        {
            //Regarder vers cette direction
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
