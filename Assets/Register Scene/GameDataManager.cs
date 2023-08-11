using System;
using Jackal;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameDataManager : PersistentSingleton<GameDataManager> {
    /*----Scriptable data-----------------------------------------------------------------------------------------------*/
    
    /*----Data variable-------------------------------------------------------------------------------------------------*/
    [HideInInspector] public PlayerData playerData;

    private void Start() {
        Application.targetFrameRate = Mathf.Max(60, Screen.currentResolution.refreshRate);
    }

    private void OnEnable() {
        playerData = new GameObject(Constant.DataKey_PlayerData).AddComponent<PlayerData>();
        playerData.transform.parent = transform;
        playerData.Init();
    }

    public void ResetData() {
        playerData.ResetData();
    }
}