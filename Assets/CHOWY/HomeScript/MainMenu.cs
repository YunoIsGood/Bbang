using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject setting; 
    
    private void Start()
    {
        setting.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
    public void OnClickSetting()
    {
        setting.SetActive(true);
    }
    public void OffClickSetting()
    {
        setting.SetActive(false);
    }
}
