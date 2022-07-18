using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    private Planet homePlanet;

    // Start is called before the first frame update
    void Start()
    {
        healthText = GetComponent<TextMeshProUGUI>();
        homePlanet = GameObject.Find("HomePlanet").GetComponent<Planet>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health  " + homePlanet.getHealth();
    }
}
