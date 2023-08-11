using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasingManager : MonoBehaviour
{
    public void OnPressDown(int i)
    {
        switch (i)
        {
            case 1:
                IAPManager.OnPurchaseSuccess = () => GlobalValue.Coin += 10;
                IAPManager.Instance.BuyProductID(IAPKey.PACK1);
                break;
            case 2:
                IAPManager.OnPurchaseSuccess = () => GlobalValue.Coin += 20;
                IAPManager.Instance.BuyProductID(IAPKey.PACK2);
                break;
            case 3:
                IAPManager.OnPurchaseSuccess = () => GlobalValue.Coin += 50;
                IAPManager.Instance.BuyProductID(IAPKey.PACK3);
                break;
            case 4:
                IAPManager.OnPurchaseSuccess = () => GlobalValue.Coin += 100;
                IAPManager.Instance.BuyProductID(IAPKey.PACK4);
                break;
        }
    }
}