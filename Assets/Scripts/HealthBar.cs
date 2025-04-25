using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI healthText;
    public bool showText = true;

    public void SetHealth(int health)
    {
        slider.value = health;
        if (showText) healthText.text = health.ToString();
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        if (showText) healthText.text = health.ToString();
    }
}