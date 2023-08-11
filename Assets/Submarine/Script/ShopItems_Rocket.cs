using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopItems_Rocket : MonoBehaviour
{
    
    // Use this for initialization
    void Start()
    {
        priceTxt.text = price + "";
        currentTxt.text = GlobalValue.Rocket + "/" + max;
    }

    public void Buy()
    {
        if (GlobalValue.Coin >= price && GlobalValue.Rocket < max)
        {
            SoundManager.PlaySfx(soundPurchased);
            GlobalValue.Coin -= price;
            GlobalValue.Rocket += amount;
            currentTxt.text = GlobalValue.Rocket + "/" + max;
        }
        else
            SoundManager.PlaySfx(soundFail);
    }
    
    public int price = 5;
    public int amount = 1;
    public int max = 100;
    public Text priceTxt;
    public Text currentTxt;
    public AudioClip soundPurchased;
    public AudioClip soundFail;

}