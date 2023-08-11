using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Coin_UI : MonoBehaviour
{
    public Text coin;
    
    void Update()
    {
        coin.text = GlobalValue.Coin + "";
    }
}