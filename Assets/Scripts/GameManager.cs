using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public static float balance = 1000;
    public static float bet = 10;
    public static int playerHand;
    public static int dealerHand;
    public List<SO_Card> FullDeck;

    public Stack<SO_Card> activeDeck = new Stack<SO_Card>();
    public List<SO_Card> discardDeck;

    [Header("References")]
    [SerializeField]
    private GameObject balanceText;

    [SerializeField]
    private GameObject betText;

    [SerializeField]
    private GameObject dealButton;

    [SerializeField]
    private GameObject goldImage;

    [SerializeField]
    private GameObject goldPrefab;

    private Animation balanceAnim;

    public static event System.Action onDestroyCard;

    public static GameManager gameManager { get; private set; }

    private void Awake()
    {
        if (gameManager != null && gameManager != this)
            Destroy(this);
        else
            gameManager = this;
    }

    void Start()
    {
        shuffle(FullDeck);

        balanceText.GetComponent<TMPro.TMP_Text>().text = balance.ToString();
        balanceAnim = balanceText.GetComponent<Animation>();
    }

    public void PlayerWins(float _rewards, float delay)
    {
        StartCoroutine(animateBalance(_rewards, delay));
        StartCoroutine(animateGold(delay));
    }

    private IEnumerator animateBalance(float _rewards, float delay)
    {
        yield return new WaitForSeconds(delay);
        float totalRewards = bet * _rewards;
        float newBalance = balance + totalRewards;

        balanceAnim.Play("Font_Oscillate");
        while (balance < newBalance)
        {
            balance += 1f;
            int temp = (int)balance;
            balanceText.GetComponent<TMPro.TMP_Text>().text = temp.ToString();
            yield return null;
        }
        balance = (int)newBalance;
        balanceAnim.Stop("Font_Oscillate");
    }

    private IEnumerator animateGold(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < 4; i++)
        {
            GameObject _coin = Instantiate(
                goldPrefab,
                ButtonFunctions.chipSelected.transform.position,
                goldImage.transform.rotation
            );
            yield return new WaitForSeconds(.2f);
        }
        yield return null;
    }

    public bool UpdateBalance()
    {
        if (balance - bet >= 0)
        {
            balance -= bet;
            int temp = (int)balance;
            balance = temp;

            balanceText.GetComponent<TMPro.TMP_Text>().text = balance.ToString();
            return true;
        }
        else
        {
            return false;
            print("Insufficient funds.");
        }
        if (balance <= 0)
            dealButton.GetComponent<Button>().interactable = false;
    }

    public void ActiveDeckCount()
    {
        if (activeDeck.Count <= 0)
        {
            onDestroyCard?.Invoke();
            shuffle(FullDeck);
        }
    }

    // Uses Fisher-Yates
    // Switched to IEnumerator to add delay since onDestroyCard causes issues
    void shuffle(List<SO_Card> _deck)
    {
        Random r = new Random();
        for (int i = _deck.Count - 1; i > 0; i--)
        {
            SO_Card temp = _deck[i];
            int index = r.Next(0, i + 1);
            _deck[i] = _deck[index];
            _deck[index] = temp;
        }

        foreach (SO_Card _card in _deck)
        {
            activeDeck.Push(_card);
        }
    }
}
