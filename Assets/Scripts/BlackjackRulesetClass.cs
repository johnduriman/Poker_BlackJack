using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackRulesetClass
{
    public int aceExistsPlayer = 0;
    public int aceExistsDealer = 0;

    public void setValue(string _cardValue, string handAffected)
    {
        int _actualValue;
        bool isAce = false;
        switch (_cardValue)
        {
            case "A":
                _actualValue = 11;
                isAce = true;
                break;
            case "J":
                _actualValue = 10;
                break;
            case "Q":
                _actualValue = 10;
                break;
            case "K":
                _actualValue = 10;
                break;
            default:
                _actualValue = System.Int32.Parse(_cardValue);
                break;
        }
        if (handAffected == "player")
        {
            GameManager.playerHand += _actualValue;
            if (isAce)
                aceExistsPlayer++;
            if (ButtonFunctions.playerHandCount < 7)
                ButtonFunctions.playerHandCount++;
            else
                ButtonFunctions.playerHandCount = 0;
        }
        else if (handAffected == "dealer")
        {
            GameManager.dealerHand += _actualValue;
            if (isAce)
                aceExistsDealer++;
            if (ButtonFunctions.dealerHandCount < 7)
                ButtonFunctions.dealerHandCount++;
            else
                ButtonFunctions.dealerHandCount = 0;
        }
    }

    public bool calculateFinalhand()
    {
        Debug.Log("Dealer " + GameManager.dealerHand);

        if (GameManager.playerHand > GameManager.dealerHand || GameManager.dealerHand > 21)
        {
            Debug.Log("Player Wins");
            GameManager.gameManager.PlayerWins(2f, 0f);
            return true;
        }
        else
        {
            Debug.Log("Dealer Wins");
            return false;
        }
    }

    public bool checkBust()
    {
        if (GameManager.playerHand > 21)
        {
            if (aceExistsPlayer > 0)
            {
                GameManager.playerHand -= 10;
                aceExistsPlayer--;
                return false;
            }
            return true;
        }
        return false;
    }

    public void resetCurrentCards()
    {
        GameManager.playerHand = 0;
        GameManager.dealerHand = 0;
        aceExistsDealer = 0;
        aceExistsPlayer = 0;
    }
}
