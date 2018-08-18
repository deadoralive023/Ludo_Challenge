﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net;
using Assets.SimpleAndroidNotifications;
using GameSparks.Core;

public class GameController : MonoBehaviour
{

    public static string GAME_STATE;
    public static int playerTurn;
    private GameObject dice;
    private GameObject gameManager;
    private GameObject marker;
    private int markerNo;
    private bool checkLoop_AI_Dice;
    private bool checkLoop_AI_Marker;
    public int[,] playingArea;
    private int markerNo_AI;
    public static bool onlineMultiplayer_diceRoll;
    public static int change_turn = 0;
    public static bool ActivateTurnTakenListener;
    public static bool isApprunningInBackground;
    public static float timeToChangeTurn_ifNotTaken = 0;
    public static bool isNetworkAvaiable;
    public static bool internetCheck;
    public static bool startTimer;
    private bool timeToChangeTurnMethodCalled;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag(GameConstants.GAME_MANAGER);


        dice = GameObject.FindGameObjectWithTag(GameConstants.PLAYER_DICE[0]);

        playingArea = new int[4, 4];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                playingArea[i, j] = i;
            }
        }

        checkLoop_AI_Dice = false;
        checkLoop_AI_Marker = false;
        onlineMultiplayer_diceRoll = false;
        ActivateTurnTakenListener = false;
        isNetworkAvaiable = false;
        internetCheck = false;
        startTimer = false;
        timeToChangeTurnMethodCalled = false;

    }
    private void Start()
    {
        GAME_STATE = GameConstants.DICE_ROLL;
        isApprunningInBackground = false;

    }



    private void Update()
    {

        StartGame();
        //if (startTimer)
        //{
        //    ChangeTurnIfTimeExceeds();
        //}
        //if (ActivateTurnTakenListener)
        //{
        //    ActivateTurnTakenListener = false;
        //    StartCoroutine(TurnTakenListenerDelay());
        //}

        if (!internetCheck && GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER)
        {
            internetCheck = true;
            StartCoroutine(OnlineMultiplayer.checkInternetConnection((isConnected) =>
            {
            // handle connection status here
            if (isConnected)
                {
                    isNetworkAvaiable = true;
                    internetCheck = false;
                }
                else
                {
                    ShowToast.MyShowToastMethod("Netowrk Error!");
                    isNetworkAvaiable = false;
                    internetCheck = false;
                }
            }));
        }

    }



    void StartGame()
    {
        switch (GAME_STATE)
        {
            case "DiceRoll":

                if ((GameInitializer.GameType == GameConstants.LOCAL_MULTIPLAYER && dice.GetComponent<Dice>().isClicked) ||
                    (GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER && playerTurn == 0 && dice.GetComponent<Dice>().isClicked) ||
                    (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER && playerTurn == 0 && dice.GetComponent<Dice>().isClicked && isNetworkAvaiable))
                {
                    GameController.startTimer = false;
                    timeToChangeTurn_ifNotTaken = 0;
                    dice.GetComponent<Dice>().RollDice();
                    GAME_STATE = GameConstants.MARKER_SELECT;
                }
                else if (GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER && playerTurn != 0 && !checkLoop_AI_Dice)
                {
                    checkLoop_AI_Dice = true;
                    dice.GetComponent<Dice>().RollDice();
                    Debug.Log("Dice Roll");
                }
                else if (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER && playerTurn != 0 && !checkLoop_AI_Dice && onlineMultiplayer_diceRoll)
                {
                    checkLoop_AI_Dice = true;
                    onlineMultiplayer_diceRoll = false;
                    dice = GameObject.FindGameObjectWithTag(GameConstants.PLAYER_DICE[playerTurn]);
                    dice.GetComponent<Dice>().RollDice();
                }
                break;

            case "MarkerSelect":
                for (int i = 0; i < 4; i++)
                {
                    marker = GameObject.FindGameObjectWithTag(GameConstants.MARKER[playerTurn, i]);
                    if ((GameInitializer.GameType == GameConstants.LOCAL_MULTIPLAYER && marker.GetComponent<Marker>().isClicked) ||
                        (GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER && playerTurn == 0 && marker.GetComponent<Marker>().isClicked) ||
                        (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER && playerTurn == 0 && marker.GetComponent<Marker>().isClicked))
                    {
                        GAME_STATE = GameConstants.MARKER_MOVE;
                        markerNo = i;
                        break;
                    }
                    else if (GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER && playerTurn != 0 && !checkLoop_AI_Marker)
                    {
                        markerNo_AI = -1;
                        markerNo_AI = gameManager.GetComponent<AI>().ChooseBestMove(playerTurn);
                        checkLoop_AI_Marker = true;
                    }
                    else if (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER && playerTurn != 0 && !checkLoop_AI_Marker)
                    {
                        markerNo_AI = OnlineMultiplayer.markerNo;
                        marker.GetComponent<Marker>().canClick = true;
                        checkLoop_AI_Marker = true;
                        Debug.Log("Makrer Select 1");
                    }
                    if ((GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER && playerTurn != 0 && markerNo_AI != -1) ||
                       (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER && playerTurn != 0 && markerNo_AI != -1))
                    {
                        marker = GameObject.FindGameObjectWithTag(GameConstants.MARKER[playerTurn, markerNo_AI]);
                        GAME_STATE = GameConstants.MARKER_MOVE;
                        Debug.Log("Makrer Select 2");
                    }
                }
                break;

            case "MarkerMove":
                if ((GameInitializer.GameType == GameConstants.LOCAL_MULTIPLAYER) ||
                    (GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER && playerTurn == 0) ||
                    (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER && playerTurn == 0))
                {
                    marker.GetComponent<Marker>().MoveMarker(playerTurn, markerNo);
                }
                else if (((GameInitializer.GameType == GameConstants.PLAY_WITH_COMPUTER) && playerTurn != 0) ||
                         ((GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER) && playerTurn != 0))
                {
                    Debug.Log("Makrer Move");
                    marker.GetComponent<Marker>().MoveMarker(playerTurn, markerNo_AI);

                }
                if (!marker.GetComponent<Marker>().isMoving)
                {
                    GAME_STATE = GameConstants.CHECK_WIN;
                }
                break;

            case "CheckWin":
                if (gameManager.GetComponent<CheckWin>().Check_Win())
                {
                    print("Game Completed!");
                    GAME_STATE = GameConstants.CHANGE_PLAYER;
                    GameInitializer.EndGameMethod();
                }
                else
                {
                    GAME_STATE = GameConstants.CHANGE_PLAYER;
                }
                break;

            case "ChangePlayer":
                if (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER && playerTurn == 0)
                {
                    int temp_dice_value = dice.GetComponent<Dice>().diceValue;
                    if (temp_dice_value == 6 || marker.GetComponent<Marker>().markerHomed || marker.GetComponent<Marker>().markerKilled)
                    {
                        change_turn = 1;
                    }
                    else
                    {
                        change_turn = 0;
                    }
                    OnlineMultiplayer.SendValues(markerNo, temp_dice_value, change_turn, GameConstants.TWO_PLAYER_CHALLENGE_DATA_EVENT);
                }

                playerTurn = gameManager.GetComponent<PlayerActivation>().ChangePlayer(playerTurn);
                dice = GameObject.FindGameObjectWithTag(GameConstants.PLAYER_DICE[playerTurn]);
                checkLoop_AI_Dice = false;
                checkLoop_AI_Marker = false;
                GAME_STATE = GameConstants.DICE_ROLL;

                if (GameInitializer.GameType == GameConstants.ONLINE_MULTIPLAYER)
                {
                    if (OnlineMultiplayer.counter < OnlineMultiplayer.dice_value_list.Count)
                    {
                        OnlineMultiplayer.MoveOpponent(OnlineMultiplayer.markerNo_list[OnlineMultiplayer.counter], OnlineMultiplayer.dice_value_list[OnlineMultiplayer.counter]);
                    }
                    else
                    {
                        OnlineMultiplayer.markerNo_list.Clear();
                        OnlineMultiplayer.dice_value_list.Clear();
                        OnlineMultiplayer.counter = 0;
                    }
                }
                break;
        }
    }


    IEnumerator TurnTakenListenerDelay()
    {
        yield return new WaitForSeconds(1f);
        OnlineMultiplayer.check_LoadValues = false;
        OnlineMultiplayer.check_LoadValuesCount = 0;

    }

    private void ChangeTurnIfTimeExceeds()
    {

        // To check against the timer
        timeToChangeTurn_ifNotTaken += Time.deltaTime;
        //print(timeToChangeTurn_ifNotTaken);
        if (timeToChangeTurn_ifNotTaken > 20)
        {
            startTimer = false;
            timeToChangeTurn_ifNotTaken = 0;
            ActivateTurnTakenListener = false;
            StartCoroutine(TurnTakenListenerDelay());

            //GAME_STATE = GameConstants.CHANGE_PLAYER;
            //OnlineMultiplayer.SendValues(2, 0, 0, 0, GameConstants.TWO_PLAYER_CHALLENGE_DATA_EVENT);

            OnlineMultiplayer.SendValues(0, 0, 0, GameConstants.TWO_PLAYER_CHALLENGE_DATA_EVENT);
            setTurnAfterTimeOut();
        }

    }


    public  void setTurnAfterTimeOut(){
        dice = GameObject.FindGameObjectWithTag(GameConstants.PLAYER_DICE[playerTurn]);
        dice.GetComponent<Dice>().diceValue = 1;
        print(playerTurn);
        playerTurn = gameManager.GetComponent<PlayerActivation>().ChangePlayer(playerTurn);
        checkLoop_AI_Dice = false;
        checkLoop_AI_Marker = false;
        GAME_STATE = GameConstants.DICE_ROLL;
    }

    void OnApplicationFocus(bool pauseStatus)
    {
        if (pauseStatus)
        {
            //your app is NO LONGER in the background
            isApprunningInBackground = false;
        }
        else
        {
            //your app is now in the background

            isApprunningInBackground = true;
            //MyNotification notification = new MyNotification();
            //notification.MakeNotification("You must have to keep the game on foreground in Online Multiplayer Mode!");
        }
    }


}
