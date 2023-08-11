using UnityEngine;
using UnityEngine.UI;

public class UnityAds : MonoBehaviour
{
    public AudioClip soundReward;
    public Text rewardTxt;
    public int reward = 10;

    void Start()
    {
        rewardTxt.text = reward + "";
    }


    public void ShowRewardVideo()
    {

    }

    public void ShowNormalAd()
    {

    }
}