using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField]
    private GameObject deckPosition;

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private AudioClip dealSoundClip;

    [SerializeField]
    private GameObject resultPanel;

    [SerializeField]
    private GameObject dealButton;

    [SerializeField]
    private GameObject hitButton;

    [SerializeField]
    private GameObject standButton;

    [SerializeField]
    private GameObject discardButton;

    [SerializeField]
    private GameObject pokerChipPanel;

    [SerializeField]
    private GameObject[] playerPositions;

    [SerializeField]
    private GameObject[] dealerPositions;

    [SerializeField]
    private GameObject[] pokerChips;

    private PokerRulesetClass Poker = new PokerRulesetClass();
    private BlackjackRulesetClass BlackJack = new BlackjackRulesetClass();

    private GameObject card4;
    private bool flipOnce;

    private GameObject[] activeCards;
    private int[] sortedCards;

    public static GameObject chipSelected;
    public static int playerHandCount = 0;
    public static int dealerHandCount = 0;
    public static event System.Action onDiscardSelected;
    public static event System.Action onDiscard;

    private void Start()
    {
        UpdateBet(10);
        enableButton(discardButton, false);
        enableButton(hitButton, false);
        enableButton(standButton, false);

        flipOnce = false;
    }

    /*************************************  BLACKJACK  ********************************************/

    // Spawns 4 cards, sets its properties, and moves it to designated positions
    public void Deal()
    {
        if (GameManager.gameManager.UpdateBalance() == false)
        {
            return;
        }
        flipOnce = false;

        onDiscard?.Invoke();
        resultPanel.SetActive(false);
        BlackJack.resetCurrentCards();
        newPlayerCard("player", .5f);
        newPlayerCard("dealer", .75f);
        newPlayerCard("player", 1f);
        card4 = newPlayerCard("dealer", 1.25f);
        card4.transform.GetComponent<SpriteRenderer>().enabled = false;
        enableButton(hitButton, true);
        enableButton(standButton, true);
        print("Player: " + GameManager.playerHand);
    }

    public void Hit()
    {
        flipCard();
        newPlayerCard("player", .5f);
        //print("Hit | Player: " + GameManager.playerHand);

        if (BlackJack.checkBust())
        {
            resultPanel.SetActive(true);
            enableButton(hitButton, false);
            enableButton(standButton, false);
            pokerChipPanel.SetActive(true);
            resultPanel.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "Player Bust";
        }
    }

    public void Stand()
    {
        flipCard();
        print("Stand | Player: " + GameManager.playerHand);
        print("Dealer: " + GameManager.dealerHand);
        StartCoroutine(dealerMove());
    }

    // Sets properties and move to designated positions
    private GameObject newPlayerCard(string owner, float spawnTime)
    {
        checkEmpty();
        GameObject _card = Instantiate(
            cardPrefab,
            deckPosition.transform.position,
            deckPosition.transform.rotation
        );
        SO_Card _cardProperty = gameObject.transform.GetComponent<GameManager>().activeDeck.Pop();
        _card.transform.GetComponent<SpriteRenderer>().sprite = _cardProperty.sprite;

        if (owner == "player")
        {
            _card.GetComponent<SpriteRenderer>().sortingOrder = playerHandCount * 2 + 1;
            _card.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder =
                playerHandCount * 2;
            StartCoroutine(placeCard(_card, playerPositions[playerHandCount], spawnTime));
        }
        else if (owner == "dealer")
        {
            _card.GetComponent<SpriteRenderer>().sortingOrder = dealerHandCount * 2 + 1;
            _card.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder =
                dealerHandCount * 2;
            StartCoroutine(placeCard(_card, dealerPositions[dealerHandCount], spawnTime));
        }
        BlackJack.setValue(_cardProperty.cardValue, owner);
        return _card;
    }

    public IEnumerator dealerMove()
    {
        if (GameManager.dealerHand > 21)
        {
            if (BlackJack.aceExistsDealer > 0)
            {
                GameManager.dealerHand -= 10;
                BlackJack.aceExistsDealer--;
            }
        }
        if (GameManager.dealerHand < 17)
        {
            newPlayerCard("dealer", .5f);
            yield return new WaitForSeconds(.5f);
            StartCoroutine(dealerMove());
        }
        else
        {
            if (!BlackJack.calculateFinalhand())
            {
                resultPanel.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text =
                    "Dealer Wins";
                resultPanel.SetActive(true);
            }
            else
            {
                resultPanel.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text =
                    "Player Wins";
                resultPanel.SetActive(true);
            }
            enableButton(hitButton, false);
            enableButton(standButton, false);
            pokerChipPanel.SetActive(true);
        }
        yield return null;
    }

    /***************************************  POKER  **********************************************/

    // Spawns 4 cards, sets its properties, and moves it to designated positions
    public void DealPoker()
    {
        if (!GameManager.gameManager.UpdateBalance())
        {
            return;
        }
        onDiscard?.Invoke();
        enableButton(discardButton, true);
        enableButton(standButton, true);
        enableButton(dealButton, false);

        resultPanel.SetActive(false);
        pokerChipPanel.SetActive(false);

        activeCards = new GameObject[5];
        newPokerCard(0, .5f);
        newPokerCard(1, .75f);
        newPokerCard(2, 1f);
        newPokerCard(3, 1.25f);
        newPokerCard(4, 1.5f);
        int _temp = Poker.GetHandRanking(activeCards);
        print(Poker.ShowRanking(_temp));
        updateResultPanel(Poker.ShowRanking(_temp), true);
    }

    public void Discard()
    {
        onDiscardSelected?.Invoke();
        foreach (GameObject _card in activeCards)
        {
            Card _properties = _card.GetComponent<Card>();
            if (_properties.isSelected == true)
            {
                newPokerCard(_card.GetComponent<Card>().pokerPosition, .5f);
            }
        }
        enableButton(discardButton, false);
        enableButton(standButton, false);
        enableButton(dealButton, true);

        int _temp = Poker.GetHandRanking(activeCards);
        print(Poker.DeliverRewards(_temp));
        updateResultPanel(Poker.ShowRanking(_temp), false);
    }

    public void Keep()
    {
        int _temp = Poker.GetHandRanking(activeCards);
        print(Poker.DeliverRewards(_temp));
        updateResultPanel(Poker.ShowRanking(_temp), false);
        enableButton(discardButton, false);
        enableButton(standButton, false);
        enableButton(dealButton, true);
    }

    private void updateResultPanel(string _result, bool first)
    {
        if (first && _result == "Try Again")
            return;
        resultPanel.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = _result;
        resultPanel.SetActive(true);
    }

    // Simplified version of newPlayerCard
    public GameObject newPokerCard(int spawnPosition, float spawnTime)
    {
        checkEmpty();
        GameObject _card = Instantiate(
            cardPrefab,
            deckPosition.transform.position,
            deckPosition.transform.rotation
        );
        SO_Card _cardProperty = gameObject.transform.GetComponent<GameManager>().activeDeck.Pop();
        _card.transform.GetComponent<SpriteRenderer>().sprite = _cardProperty.sprite;
        _card.GetComponent<SpriteRenderer>().sortingOrder = playerHandCount * 2 + 1;
        _card.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder =
            playerHandCount * 2;
        StartCoroutine(placeCard(_card, playerPositions[spawnPosition], spawnTime));
        _card.transform.GetComponent<Card>().pokerPosition = spawnPosition;
        _card.transform.GetComponent<Card>().cardProperty = _cardProperty;
        activeCards[spawnPosition] = _card;
        return _card;
    }

    /*************************************  GENERAL  ********************************************/

    public void UpdateBet(int betAmount)
    {
        GameManager.bet = betAmount;
        for (int i = 0; i < pokerChips.Length; i++)
        {
            pokerChips[i].transform.GetChild(0).gameObject.SetActive(false);
            pokerChips[i].transform.localScale = new Vector3(1f, 1f, 1);
            chipSelected = pokerChips[0];
        }
        switch (betAmount)
        {
            case 1:
                pokerChips[0].transform.GetChild(0).gameObject.SetActive(true);
                pokerChips[0].transform.localScale = new Vector3(1.1f, 1.1f, 1);
                chipSelected = pokerChips[0];
                break;
            case 5:
                pokerChips[1].transform.GetChild(0).gameObject.SetActive(true);
                pokerChips[1].transform.localScale = new Vector3(1.1f, 1.1f, 1);
                chipSelected = pokerChips[1];
                break;
            case 10:
                pokerChips[2].transform.GetChild(0).gameObject.SetActive(true);
                pokerChips[2].transform.localScale = new Vector3(1.1f, 1.1f, 1);
                chipSelected = pokerChips[2];
                break;
            case 25:
                pokerChips[3].transform.GetChild(0).gameObject.SetActive(true);
                pokerChips[3].transform.localScale = new Vector3(1.1f, 1.1f, 1);
                chipSelected = pokerChips[3];
                break;
            case 100:
                pokerChips[4].transform.GetChild(0).gameObject.SetActive(true);
                pokerChips[4].transform.localScale = new Vector3(1.1f, 1.1f, 1);
                chipSelected = pokerChips[4];
                break;
            default:
                break;
        }
    }

    private void flipCard()
    {
        if (!flipOnce)
        {
            card4.transform.GetComponent<Card>().flipCardAnim();
            flipOnce = true;
        }
    }

    private void checkEmpty()
    {
        gameObject.transform.GetComponent<GameManager>().ActiveDeckCount();
    }

    // Uses lerp to move card to designated position
    IEnumerator placeCard(GameObject currentPosition, GameObject targetPosition, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        float dist = Vector3.Distance(
            currentPosition.transform.position,
            targetPosition.transform.position
        );
        float startTime = Time.time;
        SoundFXManager.soundFXManager.PlaySoundFXClip(dealSoundClip, deckPosition.transform, 1f);

        while (Mathf.Abs(dist) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * 3f;

            currentPosition.transform.position = Vector3.Lerp(
                currentPosition.transform.position,
                targetPosition.transform.position,
                distCovered / dist
            );

            dist = Vector3.Distance(
                currentPosition.transform.position,
                targetPosition.transform.position
            );
            yield return null;
        }
        currentPosition.transform.rotation = targetPosition.transform.rotation;
    }

    private void enableButton(GameObject _button, bool _tf)
    {
        _button.GetComponent<Button>().interactable = _tf;
        if (_tf)
        {
            _button.transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            _button.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            _button.transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, .5f);
            _button.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
