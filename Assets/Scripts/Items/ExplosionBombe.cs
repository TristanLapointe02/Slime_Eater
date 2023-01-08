using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description : Gérer l'explosion de l'item de bombe
 * Fait par : Tristan Lapointe
 */

public class ExplosionBombe : MonoBehaviour
{
    public GameObject objetExplosion; //Objet visuel de l'explosion
    public AudioClip sonExplosion; //Son de l'explosion
    public GameObject particuleExplosion; //Particule d'explosion
    public bool pickUp; //Variable de pick-up pour la bombe
    public float delai; //Délai de la bombe
    EffetItem item; //Référence a l'item
    private GameObject joueur; //Référence au joueur

    // Start is called before the first frame update
    void Start()
    {
        //Assigner les references
        item = GetComponent<EffetItem>();
        joueur = SpawnJoueur.joueur;
    }

    // Update is called once per frame
    void Update()
    {
        //Si on ramasse la bombe
        if (pickUp)
        {
            //Se faire transporter à l'intérieur du joueur
            transform.position = new Vector3(joueur.transform.position.x, joueur.transform.position.y, joueur.transform.position.z);

            //Enlever notre collider
            GetComponent<Collider>().enabled = false;

            //Changer la taille de l'item selon la taille du joueur
            gameObject.transform.localScale = new Vector3(joueur.transform.localScale.x, joueur.transform.localScale.y, joueur.transform.localScale.z);
        }
    }

    //Fonction d'explosion
    public IEnumerator Explosion()
    {
        //Indiquer qu'on vient de ramasser l'item
        pickUp = true;

        //Attendre le délai de la bombe
        yield return new WaitForSeconds(delai);

        //Spawn un objet visuel
        GameObject effet = Instantiate(objetExplosion, gameObject.transform.position, Quaternion.identity);

        //Changer la taille de l'effet selon notre rayon
        effet.transform.localScale = new Vector3(item.valeur + joueur.transform.localScale.x, 0.1f, item.valeur + joueur.transform.localScale.z);

        //Le détruire tout de suite après
        Destroy(effet, 0.15f);

        //Jouer un sound effect
        AudioSource.PlayClipAtPoint(sonExplosion, gameObject.transform.position);

        //Pour tous les colliders dans la zone d'explosion
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, item.valeur + joueur.transform.localScale.magnitude);

        //Indiquer qu'on ne doit plus suivre le joueur
        pickUp = false;

        //Spawn une particule
        GameObject nouvelleParticule = Instantiate(particuleExplosion, transform.position, Quaternion.identity);

        //Changer le scale de la particule selon le scale de l'explosion
        nouvelleParticule.transform.localScale = new Vector3(joueur.transform.localScale.x * 3, joueur.transform.localScale.y * 3, joueur.transform.localScale.z * 3);

        //Se détruire
        Destroy(gameObject);

        //Indiquer au spawner qu'on est plus dans la scène
        GetComponent<EffetItem>().spawner.compteur--;

        //Pour tous les collider touchés
        foreach (var collider in hitColliders)
        {
            //Trouver les ennemis
            if (collider.gameObject.TryGetComponent(out EnemyController ennemy) && objetExplosion != null)
            {
                //Leur faire subir une explosion
                ennemy.SubirExplosion(3500, transform.position, item.valeur + joueur.transform.localScale.magnitude, item.valeur * 1000);
            }
        }
    }
}
