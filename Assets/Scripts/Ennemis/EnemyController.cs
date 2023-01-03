using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Gestion des actions des ennemis
 * Fait par : Tristan Lapointe et Samuel Séguin
 */

public class EnemyController : MonoBehaviour
{
    [Header("Statistiques ennemi")]
    public StatsEnemy enemy; //Type d'ennemi
    [HideInInspector] public Color couleurEnnemi; //Couleur de base de l'ennemi
    [HideInInspector] public float vitesse; //Vitesse de l'ennemi
    [HideInInspector] public float vie; //Vie de l'ennemi

    [Header("Références")]
    [HideInInspector] public ObjectSpawner spawner; //Référence au spawner qui l'a fait spawn
    [HideInInspector] public bool forceStop; //Dit a l'ennemi d'arrêter de bouger
    private Vector3 directionJoueur; //Distance et direction entre ennemi et joueur
    public GameObject slimeLoot; //Référence au loot de slime
    public AudioClip sonSuction; //Son de suction lorsque l'ennemi touche le joueur
    private GameObject zoneEnnemi; //zone de l'ennemi
    private GameObject joueur; //Ref au joueur

    private void Start()
    {
        //Trouver la ref au joueur
        joueur = SpawnJoueur.joueur;

        //Assigner les valeurs de maniere dynamique selon les stats
        //Vie
        vie = enemy.vieMax;

        //Taille
        gameObject.transform.localScale =  new Vector3(enemy.tailleEnnemi, enemy.tailleEnnemi, enemy.tailleEnnemi);

        //Couleur
        couleurEnnemi = GetComponentInChildren<Renderer>().material.color;

        //Vitesse
        vitesse = enemy.vitesse;

        //Mettre a jour la barre de vie au départ
        GetComponent<BarreVieEnnemi>().MajBarreVie(vie, enemy.vieMax, false);

        //Trouver la zone ennemi
        foreach (Transform enfant in transform)
        {
            if (enfant.tag == "ZoneEnnemie")
            {
                zoneEnnemi = enfant.gameObject;
            }
        }

        //Changer la teinte de la zone selon la couleur de l'ennemi
        zoneEnnemi.GetComponent<Renderer>().material.color = enemy.couleur;

        //Si l'ennemi en question n'est pas un boss
        if (enemy.boss == false)
        {
            //Changer le range de la zone indicatrice de l'ennemi selon la taille de l'ennemi
            zoneEnnemi.transform.localScale = new Vector3(enemy.tailleEnnemi / 2.65f + 1.65f, zoneEnnemi.transform.localScale.y, enemy.tailleEnnemi / 2.65f + 1.65f);
        }
        //Sinon, si il l'est
        else
        {
            //Empêcher à l'ennemi de bouger
            forceStop = true;
        } 
    }

    private void Update()
    {
        //Fixer la rotation de la zone
        var rotation = Quaternion.LookRotation(Vector3.right, Vector3.left);
        zoneEnnemi.transform.rotation = rotation;
    }

    private void FixedUpdate()
    {
        //Regarder le joueur, juste en x et z
        gameObject.transform.LookAt(new Vector3(joueur.transform.position.x, transform.position.y, joueur.transform.position.z));

        //Obtenir la distance et direction avec joueur
        directionJoueur = joueur.transform.position - transform.position;

        //Si l'ennemi n'attaque pas à distance
        if (enemy.ranged == false && forceStop == false)
        {
            //Appeler la fonction pour bouger normalement
            Move();
        }

        //Si l'ennemi attaque à distance
        if (enemy.ranged == true && forceStop == false)
        {
            // Si le joueur est trop loin, bouger vers lui
            if (InRangeJoueur() == false)
            {
                Move();
            }
        }

        //Si nous sommes pas un boss et que notre position avec le joueur est trop loin, despawn
        if(enemy.boss == false && spawner != null)
        {
            if (directionJoueur.magnitude >= spawner.rayonSpawn * 2f)
            {
                //Dire au spawner de spawn un autre ennemi
                spawner.canSpawn = true;
                spawner.Spawn();

                //Mourir
                MortEnnemi();
            }
        }
    }

    //Fonction permettant de bouger vers le joueur
    private void Move()
    {
        //Modifier la position de l'ennemi selon la vitesse, si nous ne sommes pas en pause
        if(ControleAmeliorations.pause == false && ControleMenu.pauseMenu == false)
        {
            transform.position += directionJoueur.normalized * vitesse * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Lorsqu'on collide avec le joueur
        if (collision.gameObject.tag == "Player" && enemy.boss == false)
        {
            // Faire perdre de la vie au joueur
            collision.gameObject.GetComponent<ComportementJoueur>().TakeDamage(enemy.degats);

            //Faire jouer un son
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(sonSuction);

            //Spawn du loot
            SpawnLoot();

            //Mourir
            MortEnnemi();
        }
    }

    //Fonction qui fait perdre de la vie à l'ennemi
    public void TakeDamage(float damage)
    {
        //Diminuer la vie
        vie -= damage;

        //Si la vie tombe a 0
        if (vie <= 0)
        {
            //Si c'était le boss, jouer un sound effect
            if (enemy.boss)
            {
                AudioSource.PlayClipAtPoint(GetComponent<ComportementBoss>().bossKill, transform.position);
            }

            //Spawn du loot
            SpawnLoot();

            //Mourir
            MortEnnemi();

            //Indiquer qu'un ennemi est mort
            ComportementJoueur.ennemisTues++;
        }

        //Mettre a jour la barre de vie
        GetComponent<BarreVieEnnemi>().MajBarreVie(vie, enemy.vieMax);

        //Changer le matériel pendant 0.15 secondes
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
        Invoke("AppliquerMat", 0.15f);
    }

    //Fonction qui ralentit l'ennemi
    public IEnumerator SlowMovement(float pourcentage, float duree)
    {
        //Calculer le pourcentage
        float vitesseEnlevee = vitesse * pourcentage / 100;

        //Enlever la vitesse
        vitesse -= vitesseEnlevee;

        //Attendre un délai
        yield return new WaitForSeconds(duree);

        //Remettre la vitesse à la normale
        vitesse += vitesseEnlevee;
    }

    //Fonction permettant de spawn du loot
    public void SpawnLoot()
    {
        //Spawn du loot selon le nombre à spawn
        for (int i = 0; i < enemy.nombreLootSpawn; i++)
        {
            //Spawn le loot
            GameObject loot = Instantiate(slimeLoot, transform.position, Quaternion.identity);

            //Changer la valeur du loot selon la valeur max qu'elle peut avoir
            loot.GetComponent<EffetItem>().valeur = Random.Range(enemy.valeurLoot/2, enemy.valeurLoot);
        }

        //Redonner de la vie au joueur, s'il y a lieu
        if(joueur.GetComponent<ComportementJoueur>().vieVampire > 0)
        {
            joueur.GetComponent<ComportementJoueur>().AugmenterVie(joueur.GetComponent<ComportementJoueur>().vieVampire, true);
        } 
    }

    //Fonction de mort de l'ennemi
    public void MortEnnemi()
    {
        //Se détruire
        Destroy(gameObject);
    }

    //Fonction permettant de remettre le matériel de l'ennemi à la normale 
    public void AppliquerMat()
    {
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = couleurEnnemi;
    }

    //Fonction retournant le joueur est dans la range de l'ennemi
    public bool InRangeJoueur()
    {
        return directionJoueur.magnitude <= enemy.range + joueur.GetComponent<Collider>().bounds.size.x;
    }

    //Fonction permettant à l'ennemi de subir une explosion dans une direction donnée
    public void SubirExplosion(float force, Vector3 centre, float rayon, float degats)
    {
        //Faire une explosion, si nous sommes pas le boss
        if (enemy.boss == false)
        {
            GetComponent<Rigidbody>().AddExplosionForce(force, centre, rayon);
        }

        //Prendre des dégâts
        TakeDamage(degats);
    }
}
