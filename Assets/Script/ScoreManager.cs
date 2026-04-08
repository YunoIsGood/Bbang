using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider batteryBar;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreText2;
    public void AddHp(int value)
    {
            hpBar.value = value;
            scoreText.text = $"HP: {value}";

    }
    public void Battery(float value)
    {
            batteryBar.value = value; 
            scoreText2.text = $"Battery: {value}";
       
        
    }

}
