﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckWin : MonoBehaviour {

    private GameObject gameManager;
    public static string playerWon;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag(GameConstants.GAME_MANAGER);
    }
    // Update is called once per frame
    void Update () {
        if (GameInitializer.Game == GameConstants.LUDO_CHALLENGE)
        {
            CheckMarkerHome();
        }
	}

    private void Start()
    {
        playerWon = "None";
    }

    void CheckMarkerHome(){
        gameManager = GameObject.FindGameObjectWithTag(GameConstants.GAME_MANAGER);
        for (int i = 0; i < 4; i++){
            for (int j = 0; j < 4; j++){
                if(gameManager.GetComponent<GameInitializer>().marker[i,j].GetComponent<Marker>().boxCount == 56){
                    if (Register.isSoundPlaying)
                    {
                        gameManager.GetComponent<GameInitializer>().markerClear.Play();
                    }
                    gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Marker>().temp_boxCount = ((i*i * j*j) + i*i) + Random.Range(0, 10000);
                    gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Marker>().noOfMarkersOnSameSpot = 0;
                    gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Marker>().isOpen = false;
                    gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Marker>().markerHomed = true;
                    gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Marker>().markerPassed = true;
                    gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Marker>().boxCount++;
                    gameManager.GetComponent<GameController>().playingArea[i, j] = ((i * j) + i) + Random.Range(0, 10000);
                    GameObject home_marker = GameObject.FindGameObjectWithTag(GameConstants.MARKER_HOME[i, j]);
                    home_marker.GetComponent<Image>().sprite = gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Image>().sprite;
                    Color color = home_marker.GetComponent<Image>().color;
                    color.a = 1f;
                    home_marker.GetComponent<Image>().color = color;
                    color = gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Image>().color;
                    color.a = 0f;
                    gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Image>().color = color;
                    gameManager.GetComponent<GameInitializer>().marker[i, j].transform.localScale = new Vector2(0, 0);
                    gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Image>().enabled= false;
                    break;
                }
            }
        }
    }


    public bool Check_Win_SAL(int playerTurn){
        bool win = false;
        if (gameManager.GetComponent<GameInitializer>().marker[playerTurn, 0].GetComponent<Marker>().boxCount == 99)
        {
            if (GameInitializer.GameType == GameConstants.LOCAL_MULTIPLAYER || GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER)
            {
                if (playerTurn == 0)
                {
                    playerWon = GameInitializer.Player1Name.text;
                }
                else if (playerTurn == 1)
                {
                    playerWon = GameInitializer.Player2Name.text;
                }
                else if (playerTurn == 2)
                {
                    playerWon = GameInitializer.Player3Name.text;
                }
                else if (playerTurn == 3)
                {
                    playerWon = GameInitializer.Player4Name.text;
                }
            }
            else if (GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER)
            {
                if (playerTurn == 0)
                {
                    playerWon = GameInitializer.Player1Name.text;
                }
                else
                {
                    playerWon = "Computer";
                }
            }
            win = true;
        }
        return win;
    }


    public bool Check_Win(){
        for (int i = 0; i < 4; i++){
            bool[] win = new bool[4] { false, false, false, false };
            for (int j = 0; j < 4; j++)
            {
                if (gameManager.GetComponent<GameInitializer>().marker[i, j].GetComponent<Marker>().boxCount == 57){
                    win[j] = true;
                }
            }
            if (win[0] && win[1] && win[2] && win[3])
            {
                if(GameInitializer.GameType == GameConstants.LOCAL_MULTIPLAYER || (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER && GameController.playerTurn == 0)){
                    if (i == 0)
                    {
                        playerWon = GameInitializer.Player1Name.text;
                    }
                    else if (i == 1)
                    {
                        playerWon = GameInitializer.Player2Name.text;
                    }
                    else if (i == 2)
                    {
                        playerWon = GameInitializer.Player3Name.text;
                    }
                    else if (i == 3)
                    {
                        playerWon = GameInitializer.Player4Name.text;
                    }
                }
                else if(GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER && GameController.playerTurn == 0){
                    playerWon = GameInitializer.Player1Name.text;
                }
                else if(GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER && GameController.playerTurn != 0){
                    playerWon = "Computer";
                }
                return true;
            }
        }
        return false;
    }
}
