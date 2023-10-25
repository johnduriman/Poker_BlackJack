// Poker Hand Rankings for Jacks or Better

// 1. Royal Flush       |   800
// 2. Straight Flush    |   50
// 3. Four of a Kind    |   25
// 4. Full House        |   10
// 5. Flush             |   5
// 6. Straight          |   4
// 7. 3 of a Kind       |   3
// 8. Two Pair          |   2
// 9. Pair              |   0.5


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerRulesetClass
{
    private int[] sortedCards;
    private bool aceExists = false;
    private int rankTracker = 20;

    public int GetHandRanking(GameObject[] _activeCards)
    {
        bool _flushExists = false;
        bool _straightExists = false;
        rankTracker = 20;

        if (isFlush(_activeCards))
        {
            _flushExists = true;
            finalRanking(5);
        }
        if (isStraight(_activeCards))
        {
            _straightExists = true;
            if (isRoyalFlush() && _flushExists)
            {
                return 1;
            }
            if (_flushExists && _straightExists)
            {
                return 2;
            }
            return 6;
        }

        return finalRanking(checkPair());
    }

    private int finalRanking(int _rank)
    {
        if (_rank < rankTracker)
        {
            rankTracker = _rank;
        }
        return rankTracker;
    }

    private int checkPair()
    {
        int[] _pairs = new int[sortedCards.Length];
        int[] pairValue = new int[sortedCards.Length];

        int pairCount = 0;
        for (int i = 0; i < sortedCards.Length - 1; i++)
        {
            for (int j = i; j < sortedCards.Length - 1; j++)
            {
                int _temp = sortedCards[j + 1];
                //Debug.Log(sortedCards[i] + " | " + _temp);
                if (sortedCards[i] == sortedCards[j + 1])
                {
                    _pairs[i]++;
                    pairValue[i] = sortedCards[i];
                }
            }
            // Debug.Log("Pairs: " + _pairs[i]);
            // Debug.Log("Pair Values: " + pairValue[i]);

            if (_pairs[i] > 0)
                pairCount++;
        }
        // Debug.Log("Pair Count: " + pairCount);

        for (int i = 0; i < _pairs.Length; i++)
        {
            // Check four of a kind
            if (_pairs[i] == 3)
            {
                return 3;
            }

            // Check for full house, otherwise three of a kind
            if (_pairs[i] == 2)
            {
                if (pairCount >= 2)
                {
                    if (checkFullHouse(pairValue))
                        return 4;
                }
                finalRanking(7);
            }

            // Check two pair, otherwise pair
            if (_pairs[i] == 1)
            {
                if (pairCount == 2)
                {
                    finalRanking(8);
                }
                finalRanking(9);
            }
        }
        return 20;
    }

    private bool checkFullHouse(int[] pairValue)
    {
        bool uniquePairs = false;
        for (int i = 0; i < pairValue.Length - 1; i++)
        {
            if (pairValue[i] == 0)
                continue;
            else
            {
                for (int j = 0; j < pairValue.Length - 1; j++)
                {
                    if (pairValue[i] != pairValue[j + 1] && pairValue[j + 1] != 0)
                    {
                        Debug.Log(pairValue[i] + " | " + pairValue[j + 1]);
                        uniquePairs = true;
                    }
                }
            }
        }
        return uniquePairs;
    }

    private bool isRoyalFlush()
    {
        if (sortedCards[0] != 1)
            return false;
        if (sortedCards[1] != 10)
            return false;
        if (sortedCards[2] != 11)
            return false;
        if (sortedCards[3] != 12)
            return false;
        if (sortedCards[4] != 13)
            return false;
        return true;
    }

    // Possible ranking for flush
    // 1. Royal Flush  |  2. Straight Flush  |  5. Flush
    private bool isFlush(GameObject[] _activeCards)
    {
        Suit _suit = _activeCards[0].GetComponent<Card>().cardProperty.suit;
        if (
            _suit == _activeCards[1].GetComponent<Card>().cardProperty.suit
            && _suit == _activeCards[2].GetComponent<Card>().cardProperty.suit
            && _suit == _activeCards[3].GetComponent<Card>().cardProperty.suit
            && _suit == _activeCards[4].GetComponent<Card>().cardProperty.suit
        )
        {
            return true;
        }
        return false;
    }

    // Assigns numerical values for the card and sorts them to check adjancency
    private bool isStraight(GameObject[] activeCards)
    {
        bool _straightExists = false;
        aceExists = false;
        sortedCards = new int[5];

        for (int i = 0; i < activeCards.Length; i++)
        {
            string _properties = activeCards[i].GetComponent<Card>().cardProperty.cardValue;
            switch (_properties)
            {
                case "A":
                    sortedCards[i] = 1;
                    aceExists = true;
                    break;
                case "J":
                    sortedCards[i] = 11;
                    break;
                case "Q":
                    sortedCards[i] = 12;
                    break;
                case "K":
                    sortedCards[i] = 13;
                    break;
                default:
                    sortedCards[i] = System.Int32.Parse(_properties);
                    break;
            }
        }
        System.Array.Sort(sortedCards);
        for (int i = 0; i < sortedCards.Length - 1; i++)
        {
            int _temp = sortedCards[i + 1];
            //Debug.Log(sortedCards[i] + " | " + _temp);
            if (sortedCards[i] + 1 == sortedCards[i + 1])
            {
                _straightExists = true;
            }
            else if (aceExists && sortedCards[i] + 9 == sortedCards[i + 1])
            {
                _straightExists = true;
            }
            else
            {
                _straightExists = false;
                break;
            }
        }
        if (_straightExists)
            return true;
        return false;
    }

    public string ShowRanking(int _rank)
    {
        string _winType = null;
        switch (_rank)
        {
            case 1:
                _winType = "Royal Flush";
                break;
            case 2:
                _winType = "Straight Flush";
                break;
            case 3:
                _winType = "Four of a Kind";
                break;
            case 4:
                _winType = "Full House";
                break;
            case 5:
                _winType = "Flush";
                break;
            case 6:
                _winType = "Straight";
                break;
            case 7:
                _winType = "3 of a Kind";
                break;
            case 8:
                _winType = "Two Pair";
                break;
            case 9:
                _winType = "Pair";
                break;
            default:
                _winType = "Try Again";

                break;
        }
        return _winType;
    }

    public string DeliverRewards(int _rank)
    {
        string _winType = null;
        switch (_rank)
        {
            case 1:
                GameManager.gameManager.PlayerWins(800f, 1f);
                _winType = "Royal Flush";
                break;
            case 2:
                GameManager.gameManager.PlayerWins(50f, 1f);
                _winType = "Straight Flush";
                break;
            case 3:
                GameManager.gameManager.PlayerWins(25f, 1f);
                _winType = "Four of a Kind";
                break;
            case 4:
                GameManager.gameManager.PlayerWins(10f, 1f);
                _winType = "Full House";
                break;
            case 5:
                GameManager.gameManager.PlayerWins(5f, 1f);
                _winType = "Flush";
                break;
            case 6:
                GameManager.gameManager.PlayerWins(4f, 1f);
                _winType = "Straight";
                break;
            case 7:
                GameManager.gameManager.PlayerWins(3f, 1f);
                _winType = "3 of a Kind";
                break;
            case 8:
                GameManager.gameManager.PlayerWins(2f, 1f);
                _winType = "Two Pair";
                break;
            case 9:
                GameManager.gameManager.PlayerWins(.5f, 1f);
                _winType = "Pair";
                break;
            default:
                _winType = "Try Again";
                break;
        }
        return _winType;
    }
}
