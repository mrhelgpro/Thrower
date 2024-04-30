using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject startWindow;
    [SerializeField] private GameObject resultWindow;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() => playerController.MenuGame();

    public void StartGame()
    {
        startWindow.SetActive(false);
        resultWindow.SetActive(false);
        playerController.StartGame();
    }
    
    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
    public void ResultGame() => resultWindow.SetActive(true);
}