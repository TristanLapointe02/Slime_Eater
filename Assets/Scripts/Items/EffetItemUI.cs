using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Description : Effets visuels des items
 * Fait par : Tristan Lapointe
 */

public class EffetItemUI : MonoBehaviour
{
    public Image imageEffet; //Image de l'effet
    public Image imageCooldown; //Image du cooldown
    public float temps; //Dur�e de l'effet
    private bool isCooldown; //V�rifier si nous sommes en cooldown
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

        //Si l'effet se termine, detruire l'objet UI
        if (tempsCooldown < 0)
        {
            Destroy(gameObject);
        }

        //Mettre a jour le compteur
        texteCompteurEffet.text = Mathf.RoundToInt(tempsCooldown).ToString();
        imageCooldown.fillAmount = tempsCooldown / temps;
    }
}
