using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI coffeesLabel;
    [SerializeField] private TextMeshProUGUI lostCustomersLabel;

    private const int TargetCoffees = 10;
    private const int MaxLostCustomers = 10;

    private int coffeesServed;
    private int customersLost;
    private bool gameFinished;

    public bool IsGameFinished => gameFinished;

    private void Awake()
    {
        Instance = this;
        UpdateHud();
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
        coffeesLabel.text = won ? "SERVICIO COMPLETADO" : "DEMASIADOS CLIENTES SE FUERON";
        lostCustomersLabel.text = won
            ? "Entregaste 10 cafes. Pulsa R para reintentar."
            : "Se fueron 10 clientes. Pulsa R para reintentar.";
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateHud()
    {
        coffeesLabel.text = "<size=55%>Cafes servidos</size>\n" + coffeesServed + " <size=65%>/ " + TargetCoffees + "</size>";
        lostCustomersLabel.text = "<size=55%>Clientes perdidos</size>\n" + customersLost + " <size=65%>/ " + MaxLostCustomers + "</size>";
    }
}