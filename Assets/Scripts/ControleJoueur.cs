using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleJoueur : MonoBehaviour
{
    [Header("Valeurs")]
    public float vitesse; //Vitesse du joueur
    public float forceSaut; //Force de saut du joueur
    int jumpCounter; //Counter du jump
    public int maxJump; //Nombre de sauts maximals du joueur
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
        InputProcess();
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

        if (isGrounded() && fixJump == false)
        {
            jumpCounter = maxJump;
        }
    }

    //Pour le mouvement, c'est mieux d'utiliser fixedUpdate
    private void FixedUpdate()
    {
        //Appeler la fonction de mouvement
        Move();
    }

    //Fonction de collision
    private void OnCollisionEnter(Collision collision)
    {
        //Lorsque le joueur atterit sur le sol
        if(collision.gameObject.tag == "Sol" && zone.plusGrandeDistance >= 1.5f)
        {
            //Jouer un son
            GetComponent<AudioSource>().PlayOneShot(sonAtterir);

            //Reset l'explosion
            zone.plusGrandeDistance = 0;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Si le joueur entre en collision avec un slime
        if (collision.gameObject.tag == "Slime")
        {
            //Grossir le joueur
            float valeurSlimeSize = collision.GetComponent<SlimeLoot>().sizeValue;
            gameObject.transform.localScale += new Vector3(valeurSlimeSize, valeurSlimeSize, valeurSlimeSize);

            //Jouer un son de pickup
            GetComponent<AudioSource>().PlayOneShot(sonMangerSlime);

            //Detruire le slime touché
            Destroy(collision.gameObject);
        }
    }

    //Fonction permettant de bouger le joueur
    private void Move()
    {
        //Ajouter de la force à la balle
        Vector3 deplacement = new Vector3(xInput, 0f, zInput);
        rb.AddForce(deplacement.normalized * vitesse);
    }

    //Fonction permettant de verifier si le joueur touche le sol
    bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, GetComponent<Collider>().bounds.size.y);
    }

    //Fonction permettant de reset le jump
    public void ResetJump()
    {
        fixJump = false;
    }
}
