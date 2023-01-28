using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
 * Description : Gestion des inputs et actions du joueur
 * Fait par : Tristan Lapointe et Samuel Séguin
 */

public class ControleJoueur : MonoBehaviour
{
    [Header("Valeurs")]
    public float vitesse; //Vitesse du joueur
    public float dashVitesse; //Vitesse du dash
    private float dashTimer; //Timer du dash
    public float dashCooldown; //Cooldown du dash
    public float forceSaut; //Force de saut du joueur
    int jumpCounter; //Compteur du jump
    public int maxJump; //Nombre maximum de sauts du joueur
    public float degatsZone; //Dégâts de la zone de saut
    public float forceExplosionInitiale; //Force d'explosion initiale
    [HideInInspector] public float forceExplosion; //Force de l'explosion de zone
    public float multiplicateurForceExplosion; //Par combien multiplier la force d'explosion selon la taille du joueur
    [HideInInspector] public float tailleDash; //Taille a regénérer pour l'amelioration de dash
    float xInput; //Inputs sur l'axe des x
    float zInput; //Inputs sur l'axe des z
    [HideInInspector] public bool peutExploser = true; //Gère si l'on peut exploser ou non
    public float fallMultiplier; //Multiplicateur de la force de gravité

    [Header("Sons")]
    public AudioClip sonJump; //Son lorsque le joueur saute
    public AudioClip sonAtterir; //Son lorsque le joueur atterit
    public AudioClip sonMangerSlime; //Son lorsque le joueur mange un item slime

    [Header("References")]
    Rigidbody rb; //Référence au Rigidbody du joueur
    public ZoneDegats zone; //Référence à la zone de dégâts du joueur
    public Image imageAttaque; //Reference a l'image du cooldown d'attaque
    public Image imageDash; //Reference a l'image du cooldown de dash

    //AUTRES
    bool fixJump; //Bool permettant de fix le jump

    void Start()
    {
        //Assigner les références
        rb = GetComponent<Rigidbody>();

        //Reset des valeurs
        jumpCounter = maxJump;
        forceExplosion = forceExplosionInitiale;

        //Montrer la zone d'explosion
        zone.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    
    void Update()
    {
        //Amélioration de gravité
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }

        //Capturer les inputs
        if(ComportementJoueur.finJeu == false && ControleAmeliorations.pause == false && ControleMenu.pauseMenu == false)
        {
            InputProcess();
        }

        //Si on est en pause ou fin du jeu, freeze le rigidbody
        if(ControleAmeliorations.pause || ControleMenu.pauseMenu || ComportementJoueur.finJeu)
        {
            //Enlever la vélocité du joueur
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            //Sinon, tout est beau
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        //Gestion des inputs de menu (ne sont pas dans InputProcess parce qu'ils ne sont pas affecté par pause ou fin du jeu) : 
        // Si on appuie sur Tab
        if (Input.GetButtonDown("Open Stats"))
        {
            // Afficher le menu de statistiques
            gameObject.GetComponent<ControleMenu>().OuvrirStatistiques();
        }
        // Si on relâches Tab
        if (Input.GetButtonUp("Open Stats"))
        {
            // Fermer le menu de statistiques
            gameObject.GetComponent<ControleMenu>().FermerStatistiques();
        }

        //Si le joueur appuie sur escape
        if (Input.GetButtonDown("Cancel"))
        {
            //Ouvrir/fermer menu pause
            GetComponent<ControleMenu>().MenuOptions();
        }
    }

    //Fonction permettant de recevoir les inputs
    private void InputProcess()
    {
        //Recevoir l'input de WASD ou des touches directionnelles 
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        //Si on appuie sur espace
        if (Input.GetButtonDown("Jump") && jumpCounter > 0)
        {
            //Faire sauter le joueur
            rb.AddForce(Vector3.up * forceSaut * 1000f);

            //Jouer le son de saut
            GetComponent<AudioSource>().PlayOneShot(sonJump);

            //Indiquer que nous venons de sauter
            fixJump = true;

            //Diminuer le nombre de jumps
            jumpCounter--;

            //Permettre au joueur de sauter après un petit delai
            Invoke("ResetJump", 0.15f);

            //Montrer la zone
            zone.gameObject.GetComponent<MeshRenderer>().enabled = true;

            //Indiquer que l'on peut exploser
            peutExploser = true;
        }

        // Si on appuie sur left shift
        if (Input.GetButtonDown("Fire3") && dashTimer >= dashCooldown)
        {
            //Afficher un UI nous permettant de voir le cooldown
            StartCoroutine(cooldownAction(dashCooldown, imageDash));

            //Réinitialiser le timer du dash
            dashTimer = 0;

            //Si on ne reçoit aucun input directionnel
            if (xInput == 0 && zInput == 0)
            {
                //Dash dans la direction que l'on vise avec la souris
                Move(dashVitesse, GetComponent<ControleTir>().gun.transform.forward);
            }
            //Si on reçoit un input directionnel
            else
            {
                //Dash dans la direction du input
                Move(dashVitesse, new Vector3(xInput, 0, zInput));
            }
            
            //Regénérer de la masse s'il y a lieu
            if(tailleDash > 0)
            {
                GetComponent<ComportementJoueur>().AugmenterGrosseur(tailleDash);
            }
        }

        //Réinitialiser le compteur de jump si on touche le sol
        if (isGrounded() && fixJump == false)
        {
            jumpCounter = maxJump;
        }
    }

    //Pour le mouvement, c'est mieux d'utiliser fixedUpdate
    private void FixedUpdate()
    {
        // Incrémenter le timer pour le cooldown du dash
        if (dashTimer < dashCooldown)
        {
            dashTimer += Time.deltaTime;
        }

        //Appeler la fonction de mouvement
        Move(vitesse, new Vector3(xInput, 0, zInput));
    }

    //Fonction de collision
    private void OnCollisionEnter(Collision collision)
    {
        //Lorsque le joueur atterit sur le sol
        if((collision.gameObject.CompareTag("Sol") || collision.gameObject.CompareTag("Ennemi")) && peutExploser)
        {
            //Appeler la fonction pour faire des dégâts aux ennemis
            Explosion(forceExplosion, degatsZone);

            //Empêcher l'explosion jusqu'à temps qu'on saute/change de niveau
            peutExploser = false;
        }

        //Si le joueur touche le sol invisible, il meurt
        else if (collision.gameObject.layer == 16){
            //GetComponent<ComportementJoueur>().FinJeu("Vous êtes sorti des limites", GetComponent<ComportementJoueur>().sonPartiePerdue);
        }
    }


    //Fonction permettant de bouger le joueur
    private void Move(float vitesse, Vector3 direction)
    {
        //Ajouter de la force à la balle
        Vector3 deplacement = new Vector3(direction.x, 0f, direction.z);
        rb.AddForce(deplacement.normalized * vitesse);
    }

    //Fonction permettant de verifier si le joueur touche au sol
    bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, GetComponent<Collider>().bounds.extents.y, 10);
    }

    //Fonction permettant de réinitialiser le jump
    public void ResetJump()
    {
        fixJump = false;
    }

    //Fonction permettant de faire l'explosion
    public void Explosion(float forceExplosion, float degatsZone)
    {
        Collider[] hitColliders = Physics.OverlapSphere(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - GetComponent<Collider>().bounds.extents.y, gameObject.transform.position.z), zone.rayonActuel / 2);

        //Pour tous les collider touchés
        foreach(var collider in hitColliders)
        {
            //Trouver les ennemis
            if(collider.gameObject.TryGetComponent(out EnemyController ennemy))
            {
                //Leur faire subir une explosion
                ennemy.SubirExplosion(forceExplosion, new Vector3(transform.position.x, transform.position.y - GetComponent<Collider>().bounds.extents.y, transform.position.z), zone.rayonActuel / 2, degatsZone);   
            }
        }

        //Jouer un son
        GetComponent<AudioSource>().PlayOneShot(sonAtterir);

        //Réinitialiser l'explosion
        zone.plusGrandeDistance = 0;

        //Désactiver les visuels de la zone
        zone.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    //Fonction permettant d'afficher un timer s'écoulant après nos actions
    public IEnumerator cooldownAction(float cooldown, Image image)
    {
        //Activer notre objet UI
        image.gameObject.SetActive(true);

        //Défénir notre compteur
        float counter = cooldown;

        //Pendant qu'on est en train de s'écouler
        while (counter >= 0)
        {
            //Augmenter le counter
            counter -= Time.deltaTime;

            //Mettre a jour le fill amount
            image.fillAmount = 1 - (counter / cooldown);

            //Changer la position de l'image pour suivre le curseur
            //Si nous avons présentement un curseur souris
            if (GetComponent<GamePadCursor>().playerInput.currentControlScheme == "Keyboard&Mouse")
            {
                if(image.gameObject.name == "CooldownDash")
                {
                    image.rectTransform.position = Mouse.current.position.ReadValue() + Vector2.down * 25f;
                }
                else
                {
                    image.rectTransform.position = Mouse.current.position.ReadValue();
                }
            }

            //Sinon, si nous avons le curseur de gamepad
            else if (GetComponent<GamePadCursor>().playerInput.currentControlScheme == "Gamepad")
            {
                if (image.gameObject.name == "CooldownDash")
                {
                    image.rectTransform.position = GetComponent<GamePadCursor>().cursorTransform.position + Vector3.down * 25f;
                }
                else
                {
                    image.rectTransform.position = GetComponent<GamePadCursor>().cursorTransform.position;
                }   
            }

            //Lorsque c'est fini, sortir de la boucle
            yield return null;
        }

        //Désacter l'image
        if (image.gameObject.activeSelf)
        {
            image.gameObject.SetActive(false);
        } 
    }
}
