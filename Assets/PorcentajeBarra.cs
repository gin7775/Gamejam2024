using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class PorcentajeBarra : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private MMFeedbacks feedbackAlMoverSlider;
    [SerializeField] private float tiempoEntreSonidos = 0.1f;

    private float tiempoUltimoSonido;

    void Start()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = (v * 100).ToString("0") + " %";

            if (Time.time - tiempoUltimoSonido >= tiempoEntreSonidos)
            {
                feedbackAlMoverSlider?.PlayFeedbacks();
                tiempoUltimoSonido = Time.time;
            }
        });

        sliderText.text = (slider.value * 100).ToString("0") + " %";
    }
}
