using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public StatsEnemy enemy; // type d'ennemi
    public GameObject joueur; // Référence au joueur
    private Vector3 directionJoueur; // distance et direction entre ennemi et joueur
    public float vie; //Vie de l'ennemi
    public GameObject slimeLoot; //Reference au loot de slime
    public AudioClip sonSuction; //Son de suction lorsque l'ennemi touche le joueur
    public Color couleurEnnemi; //Couleur de base de l'ennemi
    public float vitesse; //Vitesse de l'ennemi
    public GameObject zoneEnnemi; //Lumiere de l'ennemi

    private void Start()
    {
        //Assigner les valeurs de maniere dynamique selon les stats
        //Vie
        vie = enemy.vieMax;

        //Taille
        gameObject.transform.localScale =  new Vector3(enemy.tailleEnnemi, enemy.tailleEnnemi, enemy.tailleEnnemi);

        //Changer le range de la zone indicatrice de l'ennemi selon la taille de l'ennemi
        zoneEnnemi.transform.localScale = new Vector3(enemy.tailleEnnemi*2, zoneEnnemi.transform.localScale.y, enemy.tailleEnnemi*2);

        //Changer la teinte de la zone selon la couleur de l'ennemi
        zoneEnnemi.GetComponent<Renderer>().material.color = enemy.couleur;

        //Couleur
        couleurEnnemi = GetComponentInChildren<Renderer>().material.color;

        //Vitesse
        vitesse = enemy.vitesse;

        //Trouver le joueur
        TrouverJoueur();
    }

    private void Update()
    {
        //Fix la rotation de la zone
        var rotation = Quaternion.LookRotation(Vector3.right, Vector3.left);
        zoneEnnemi.transform.rotation = rotation;
    }

    private void FixedUpdate()
    {
        //Trouver le joueur
        TrouverJoueur();

        //Regarder le joueur
        gameObject.transform.LookAt(joueur.transform);

        //Obtenir la distance et direction avec joueur
        directionJoueur = joueur.transform.position - transform.position;

        //Si l'ennemi n'est pas ranged
        if (enemy.ranged == false)
        {
            //Appeler la fonction pour bouger normalement
            Move();
        }

        //Si l'ennemi est ranged
        if (enemy.ranged == true)
        {
            // Si le joueur est trop loin, bouger vers lui
            if (InRangeJoueur() == false)
            {
                Move();
            }
        }

        //Si c'est la fin du jeu, mourir
        if (ComportementJoueur.finJeu)
        {
            MortEnnemi();
        }
    }

    //Fonction permettant de bouger vers le joueur
    private void Move()
    {
        //Modifier la position de l'ennemi selon la vitesse, si nous sommes pas en pause
        if(ControleAmeliorations.pause == false)
        {
            transform.position += directionJoueur.normalized * vitesse * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Lorsqu'on collide avec le joueur
        if (collision.gameObject.tag == "Player")
        {
            // Faire perdre de la vie au joueur
            collision.gameObject.GetComponent<ComportementJoueur>().TakeDamage(enemy.degats);

            //Faire jouer un son
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(sonSuction);

            //Mourir
            MortEnnemi();
        }
    }

    //Fonction qui fait perdre de la vie a l'ennemi
    public void TakeDamage(float damage)
    {
        //Diminuer la vie
        vie -= damage;

        //Si la vie tombe a 0
        if (vie <= 0)
        {
            //Mourir
            MortEnnemi();
        }

        //Changer le matériel pendant 0.15 secondes
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
        Invoke("AppliquerMat", 0.15f);
    }

    //Fonction qui slow l'ennemi
    public IEnumerator SlowMovement(float pourcentage, float duree)
    {
        //Calculer le pourcentage
        float vitesseEnlevee = vitesse * pourcentage / 100;

        //Enlever la vitesse
        vitesse -= vitesseEnlevee;

        //Attendre un delai
        yield return new WaitForSeconds(duree);

        //Remettre la vitesse a la normale
        vitesse += vitesseEnlevee;
    }

    //Fonction de mort de l'ennemi
    public void MortEnnemi()
    {
        //Spawn du loot selon le nombre a spawn
        for (int i = 0; i < enemy.nombreLootSpawn; i++)
        {
            //Spawn le loot
            GameObject loot = Instantiate(slimeLoot, transform.position, Quaternion.identity);

            //Changer la valeur du loot selon celle max qui faut donner
            loot.GetComponent<EffetItem>().valeur = Random.Range(0, enemy.valeurLoot);
        }

        //Indiquer qu'on est mort
        ComportementJoueur.ennemisTues++;

        //Se detruire
        Destroy(gameObject);
    }

    //Fonction permettant de remettre a la normale le matériel de l'ennemi
    public void AppliquerMat()
    {
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = couleurEnnemi;
    }

    //Fonction permettant de trouver le joueur
    public void TrouverJoueur()
    {
        //Trouver le joueur si nous l'avons pas
        if (joueur == null)
        {
            joueur = GameObject.FindGameObjectWithTag("Player");
        }
    }

    //Fonction retournant si nous sommes dans notre range
    public bool InRangeJoueur()
    {
        return directionJoueur.magnitude <= enemy.range + joueur.GetComponent<Collider>().bounds.size.x;
    }
}
