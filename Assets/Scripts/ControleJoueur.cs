using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleJoueur : MonoBehaviour
{
    [Header("Valeurs")]
    public float vitesse; //Vitesse du joueur
    public float dashVitesse; //Vitesse du dash
    public float dashTimer; //Timer du dash
    public float dashCooldown; //Cooldown du dash
    public float forceSaut; //Force de saut du joueur
    int jumpCounter; //Counter du jump
    public int maxJump; //Nombre de sauts maximals du joueur
    public float degatsZone; //Degats de la zone de saut
    public float forceExplosion; //Force de l'explosion de zone
    float xInput; //Inputs sur l'axe des x
    float zInput; //Inputs sur l'axe des x

    [Header("Sons")]
    public AudioClip sonJump; //Son lorsque le joueur saute
    public AudioClip sonAtterir; //Son lorsque le joueur atterit
    public AudioClip sonMangerSlime; //Son lorsque le joueur mange un slime

    [Header("References")]
    Rigidbody rb; //Rigidbody du joueur
    public ZoneDegats zone; //Reference a la zone de degats du joueur

    //AUTRES
    bool fixJump; //Bool permettant de fix le jump

    void Start()
    {
        //Assigner les références
        rb = GetComponent<Rigidbody>();

        //Reset des valeurs
        jumpCounter = maxJump;
    }

    
    void Update()
    {
        //Capturer les inputs
        if(ComportementJoueur.finJeu == false && ControleAmeliorations.pause == false)
        {
            InputProcess();
        }

        //Mettre a jour l'angular drag du joueur selon sa vitesse
        //GetComponent<Rigidbody>().angularDrag = (vitesse / 5) - 5;
    }

    //Fonction permettant de recevoir les inputs
    private void InputProcess()
    {
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
        }

        // Si on appuie sur left shift
        if (Input.GetButtonDown("Fire3") && dashTimer >= dashCooldown)
        {
            dashTimer = 0;
            Move(dashVitesse);
        }

        if (isGrounded() && fixJump == false)
        {
            jumpCounter = maxJump;
        }
        
        //Enable la zone de degats lorsqu'on "saute"
        if(zone.distance - GetComponent<Collider>().bounds.extents.y >= 0.5f)
        {
            //Enable les visuels de la zone
            zone.gameObject.GetComponent<MeshRenderer>().enabled = true;
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
        Move(vitesse);
    }

    //Fonction de collision
    private void OnCollisionEnter(Collision collision)
    {
        //Lorsque le joueur atterit sur le sol
        if(collision.gameObject.tag == "Sol" && zone.plusGrandeDistance - GetComponent<Collider>().bounds.extents.y >= 2f)
        {
            //Jouer un son
            GetComponent<AudioSource>().PlayOneShot(sonAtterir);

            //Appeler la fonction pour faire des dégâts aux ennemis
            Explosion(forceExplosion, degatsZone);

            //Reset l'explosion
            zone.plusGrandeDistance = 0;

            //Disable les visuels de la zone
            zone.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }


    //Fonction permettant de bouger le joueur
    private void Move(float vitesse)
    {
        //Ajouter de la force à la balle
        Vector3 deplacement = new Vector3(xInput, 0f, zInput);
        rb.AddForce(deplacement.normalized * vitesse);
    }

    //Fonction permettant de verifier si le joueur touche le sol
    bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, GetComponent<Collider>().bounds.extents.y, 10);
    }

    //Fonction permettant de reset le jump
    public void ResetJump()
    {
        fixJump = false;
    }

    //Fonction permettant de faire l'explosion
    public void Explosion(float forceExplosion, float degatsZone)
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, zone.rayonActuel / 2);

        //Pour tous les collider touchés
        foreach(var collider in hitColliders)
        {
            //Trouver les ennemis
            if(collider.gameObject.TryGetComponent(out EnemyController ennemy))
            {
                //Leur faire des degats
                ennemy.TakeDamage(degatsZone);

                //Faire une explosion
                ennemy.GetComponent<Rigidbody>().AddExplosionForce(forceExplosion, transform.position, zone.rayonActuel / 2);
            }
        }
    }


    //TEST POUR VOIR LA ZONE DE DEGATS
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireSphere(transform.position, zone.rayonActuel/2);
    }
}
