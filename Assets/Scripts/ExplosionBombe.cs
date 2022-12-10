using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBombe : MonoBehaviour
{
    public GameObject objetExplosion; //Objet visuel de l'explosion
    public AudioClip sonExplosion; //Son de l'explosion
    public GameObject particuleExplosion; //Particule d'explosion
    public bool pickUp; //Variable de pick-up pour la bombe
    public float delai; //Delai de la bombe
    EffetItem item; //Ref a l'item

    // Start is called before the first frame update
    void Start()
    {
        //Assigner les references
        item = GetComponent<EffetItem>();
    }

    // Update is called once per frame
    void Update()
    {
        //Si on pick-up la bombe
        if (pickUp)
        {
            //Suivre le joueur au-dessus de sa tête
            transform.position = new Vector3(item.player.transform.position.x, item.player.transform.position.y, item.player.transform.position.z);

            //Enlever notre collider
            GetComponent<Collider>().enabled = false;

            //Changer le scale de l'item
            gameObject.transform.localScale = new Vector3(item.player.transform.localScale.x, item.player.transform.localScale.y, item.player.transform.localScale.z);
        }
    }

    //Fonction d'explosion
    public IEnumerator Explosion()
    {
        //Indiquer qu'on vient de pickup l'item
        pickUp = true;

        //Attendre le délai de la bombe
        yield return new WaitForSeconds(delai);

        //Spawn un objet visuel
        GameObject effet = Instantiate(objetExplosion, gameObject.transform.position, Quaternion.identity);

        //Changer la grosseur de l'effet selon notre rayon
        effet.transform.localScale = new Vector3(item.valeur + item.player.transform.localScale.x, 0.1f, item.valeur + item.player.transform.localScale.z);

        //Le detruire tout de suite après
        Destroy(effet, 0.15f);

        //Jouer un sound effect
        AudioSource.PlayClipAtPoint(sonExplosion, gameObject.transform.position);

        //Pour tous les colliders dans la zone d'explosion
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, item.valeur + item.player.transform.localScale.magnitude);

        //Indiquer qu'on ne doit plus follow le joueur
        pickUp = false;

        //Spawn une particule
        GameObject nouvelleParticule = Instantiate(particuleExplosion, transform.position, Quaternion.identity);

        //Changer son scale selon le scale de l'explosion
        nouvelleParticule.transform.localScale = new Vector3(item.player.transform.localScale.x * 3, item.player.transform.localScale.y * 3, item.player.transform.localScale.z * 3);

        //Se detruire
        Destroy(gameObject);

        //Pour tous les collider touchés
        foreach (var collider in hitColliders)
        {
            //Trouver les ennemis
            if (collider.gameObject.TryGetComponent(out EnemyController ennemy) && objetExplosion != null)
            {
                //Leur faire des degats
                ennemy.TakeDamage(item.valeur * 1000);

                //Faire une explosion
                ennemy.GetComponent<Rigidbody>().AddExplosionForce(3500, transform.position, item.valeur + item.player.transform.localScale.magnitude);
            }
        }
    }
}
