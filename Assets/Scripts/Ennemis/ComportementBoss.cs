using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Comportement du boss ennemi
 * Fait par : Tristan Lapointe
 */

public class ComportementBoss : MonoBehaviour
{
    public int intervalleAttaqueSaut; //Intervalle de temps de l'attaque de saut
    public int rangeAleatoireDelai; //Délai aléatoire du saut
    public float offsetY; //Offset de bug fix sur l'axe des y
    public float forceSaut; //Force de saut de l'ennemi
    public float fallMultiplier; //Multiplicateur de plongée
    public LayerMask layerSol; //Layer du sol
    private float distance; //Distance actuelle avec le sol
    private float plusGrandeDistance; //Plus grande distance
    private float rayonActuel; //Rayon actuel de la zone
    public float bonusRayon; //Bonus de rayon
    public GameObject cercleZoneExplosion; //Objet de la zone d'explosion
    public float degatsZone; //Dégâts de la zone de saut
    public float forceExplosion; //Force de l'explosion de zone
    public AudioClip sonJump; //Son lorsque l'ennemi saute
    public AudioClip sonAtterir; //Son lorsque l'ennemi atterit
    public AudioClip bossKill; //Sound effect quand on tue le boss
    private void Start()
    {
        //Démarrer le comportement de saut
        StartCoroutine(Sauter(intervalleAttaqueSaut));

        //Montrer la zone
        cercleZoneExplosion.gameObject.GetComponent<MeshRenderer>().enabled = true;

        //Ignorer les collisions avec les autres ennemis
        Physics.IgnoreLayerCollision(9, 9);
    }

    void Update()
    {
        //Amélioration de gravité
        if (GetComponent<Rigidbody>().velocity.y < 0)
        {
            GetComponent<Rigidbody>().velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        //Changer la position du cercle selon l'ennemi
        cercleZoneExplosion.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Raycast avec le sol
        RaycastHit hit;
        // Si le raycast intersect quelconque objet sol
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerSol))
        {
            //Mettre a jour la distance
            distance = hit.distance;

            //Mettre a jour la position de la zone
            cercleZoneExplosion.gameObject.transform.position = new Vector3(cercleZoneExplosion.transform.position.x, hit.transform.position.y + hit.transform.gameObject.GetComponent<Collider>().bounds.extents.y + 0.05f, cercleZoneExplosion.transform.position.z);

            //Mettre a jour la lus grande valeur
            if (plusGrandeDistance < hit.distance)
            {
                plusGrandeDistance = hit.distance;
            }

            //Mettre a jour la taille de la zone
            rayonActuel = (plusGrandeDistance + transform.localScale.magnitude) * bonusRayon;
            cercleZoneExplosion.transform.localScale = new Vector3(rayonActuel/10, cercleZoneExplosion.transform.localScale.y, rayonActuel/10);
        }
    }

    //Fonction de collision
    private void OnCollisionEnter(Collision collision)
    {
        //Lorsque le boss atterit sur le sol
        if (collision.gameObject.tag == "Sol" && GetComponent<EnemyController>().forceStop == true)
        {
            //Jouer un son d'explosion
            GetComponent<AudioSource>().PlayOneShot(sonAtterir);

            //Indiquer qu'on peut maintenant attaquer
            GetComponent<EnnemiTir>().peutTirer = true;

            //Indiquer qu'il peut maintenant bouger
            GetComponent<EnemyController>().forceStop = false;

            //Appeler la fonction pour faire des dégâts au joueur
            Explosion(forceExplosion, degatsZone);

            //Reset l'explosion
            plusGrandeDistance = 0;

            //Enlever les visuels de la zone
            cercleZoneExplosion.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    //Fonction de saut du boss
    public IEnumerator Sauter(float delai)
    {
        //Attendre un certain délai
        yield return new WaitForSeconds(Random.Range(delai - rangeAleatoireDelai, delai + rangeAleatoireDelai));

        //Si nous sommes pas en pause,
        if(ControleAmeliorations.pause == false && ControleMenu.pauseMenu == false)
        {
            //Sauter
            GetComponent<Rigidbody>().AddForce(Vector3.up * forceSaut * 1000f);

            //Indiquer qu'on peut pas attaquer
            GetComponent<EnnemiTir>().peutTirer = false;

            //Empêcher le mouvement
            GetComponent<EnemyController>().forceStop = true;

            //Montrer la zone
            cercleZoneExplosion.gameObject.GetComponent<MeshRenderer>().enabled = true;

            //Jouer le son de saut
            GetComponent<AudioSource>().PlayOneShot(sonJump);
        }

        //Rappeler la coroutine
        StartCoroutine(Sauter(intervalleAttaqueSaut));
    }

    //Fonction d'explosion
    public void Explosion(float forceExplosion, float degatsZone)
    {
        Collider[] hitColliders = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y - offsetY, transform.position.z), rayonActuel/2);

        //Pour tous les collider touchés
        foreach (var collider in hitColliders)
        {
            //Trouver le joueur
            if (collider.gameObject.TryGetComponent(out ComportementJoueur joueur))
            {
                //Lui faire des dégâts
                joueur.TakeDamage(degatsZone);

                //Faire une explosion
                joueur.GetComponent<Rigidbody>().AddExplosionForce(forceExplosion, new Vector3(transform.position.x, transform.position.y - offsetY, transform.position.z), rayonActuel/2);
            }
        }
    }
}
