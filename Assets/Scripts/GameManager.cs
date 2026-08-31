using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI coffeesLabel;
    [SerializeField] private TextMeshProUGUI lostCustomersLabel;
    [SerializeField] private GameObject startOverlay;
    [SerializeField] private Button startButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private CanvasGroup resultOverlay;
    [SerializeField] private TextMeshProUGUI resultTitle;
    [SerializeField] private TextMeshProUGUI resultDescription;
    [SerializeField] private Button resultRestartButton;
    [SerializeField] private Animator resultAnimator;
    [SerializeField] private ParticleSystem confettiFx;
    [SerializeField] private ParticleSystem smokeFx;

    private const int TargetCoffees = 10;
    private const int MaxLostCustomers = 10;

    private int coffeesServed;
    private int customersLost;
    private bool gameStarted;
    private bool isPaused;
    private bool gameFinished;

    public bool IsGameFinished => gameFinished;
    public bool CanInteract => gameStarted && !isPaused && !gameFinished;

    private void Awake()
    {
        Instance = this;
        startButton.onClick.AddListener(StartGame);
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        resultRestartButton.onClick.AddListener(RestartGame);
        resultOverlay.alpha = 0f;
        resultOverlay.interactable = false;
        resultOverlay.blocksRaycasts = false;
        UpdateHud();
        gameStarted = false;
        isPaused = true;
        startOverlay.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (gameFinished && Input.GetKeyDown(KeyCode.R))
            RestartGame();
    }

    public void RegisterCoffeeServed()
    {
        if (gameFinished) return;

        coffeesServed++;
        UpdateHud();

        if (coffeesServed >= TargetCoffees)
            FinishGame(true);
    }

    public void StartGame()
    {
        if (gameFinished) return;

        gameStarted = true;
        isPaused = false;
        startOverlay.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (!gameStarted || gameFinished) return;

        isPaused = true;
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!gameStarted || gameFinished) return;

        isPaused = false;
        pauseButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RegisterCustomerLost()
    {
        if (gameFinished) return;

        customersLost++;
        UpdateHud();

        if (customersLost >= MaxLostCustomers)
            FinishGame(false);
    }

    private void FinishGame(bool won)
    {
        gameFinished = true;
        isPaused = true;
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        resultTitle.text = won ? "Victory!" : "Game over";
        resultTitle.color = won ? new Color(0.68f, 0.4f, 0.14f) : new Color(0.52f, 0.25f, 0.22f);
        resultDescription.text = won
            ? "You served 10 coffees and completed the shift."
            : "10 customers left. Try serving them faster.";
        resultOverlay.interactable = true;
        resultOverlay.blocksRaycasts = true;
        resultAnimator.SetTrigger(won ? "Win" : "Lose");
        if (confettiFx != null) confettiFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (smokeFx != null) smokeFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ParticleSystem resultFx = won ? confettiFx : smokeFx;
        if (resultFx != null) resultFx.Play();
        StartCoroutine(FreezeAfterResultAnimation());
    }

    private IEnumerator FreezeAfterResultAnimation()
    {
        float duration = 0.4f;
        if (resultAnimator != null && resultAnimator.runtimeAnimatorController != null)
        {
            AnimatorStateInfo state = resultAnimator.GetCurrentAnimatorStateInfo(0);
            if (0f < state.length)
                duration = Mathf.Max(duration, state.length);
        }

        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateHud()
    {
        coffeesLabel.text = "<size=55%>Customers Lost</size>\n" + coffeesServed + " <size=65%>/ " + TargetCoffees + "</size>";
        lostCustomersLabel.text = "<size=55%>Customers Lost</size>\n" + customersLost + " <size=65%>/ " + MaxLostCustomers + "</size>";
    }
}
