using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffetItemUI : MonoBehaviour
{
    public Image imageEffet; //Image de l'effet
    public Image imageCooldown; //Image du cooldown
    public float temps; //Duree de l'effet
    private bool isCooldown; //Verifier si nous sommes en cooldown
    private float tempsCooldown; //Temps du cooldown en soi
    public TextMeshProUGUI texteCompteurEffet; //Texte compteur de l'effet
    public TextMeshProUGUI texteNomEffet; //Nom de l'effet

    private void Start()
    {
        //Initialiser le timer
        tempsCooldown = temps;
    }
    // Update is called once per frame
    void Update()
    {
        //Diminuer le temps avec le temps
        tempsCooldown -= Time.deltaTime;

        //Si l'effet run out, detruire l'objet ui
        if (tempsCooldown < 0)
        {
            Destroy(gameObject);
        }

        //Mettre a jour le compteur
        texteCompteurEffet.text = Mathf.RoundToInt(tempsCooldown).ToString();
        imageCooldown.fillAmount = tempsCooldown / temps;
    }
}
