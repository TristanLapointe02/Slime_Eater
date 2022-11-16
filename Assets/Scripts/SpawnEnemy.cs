using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{

    public int SpawnAmount;
    public float spawnDelay;
    private bool canSpawn;
    public GameObject enemy;


    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
       if (canSpawn)
       {
            canSpawn = false;
            StartCoroutine(delaiSpawn(spawnDelay));
            Instantiate(enemy, this.transform.position, this.transform.rotation);
            Debug.Log("Spawned enemy");
       }
    }

    public IEnumerator delaiSpawn(float delai)
    {
        yield return new WaitForSeconds(delai);
        canSpawn = true;

    }
}
