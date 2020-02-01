﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public bool seeingStore = false;
    public DatabaseManager databaseManager;
    public MenuManager menuManager;

    public Sprite[] boatSprites;
    public Sprite[] faceSprites;
    public Sprite[] waveSprites;

    public Image boatImage;
    public Image faceImage;

    public int[] boatChargeList = { 2, 2, 2, 2, 2 };
    public int[] faceChargeList = { 2, 2, 2, 2, 2 };
    public int[] waveChargeList = { 2, 2, 2, 2, 2 };
    public Image[] boatLockImage;
    public Image[] faceLockImage;
    public Image[] waveLockImage;
    public Sprite lockImage;
    public Sprite checkImage;
    public Text[] boatPrice;
    public Text[] facePrice;
    public Text[] wavePrice;
    public Sprite storeLoading;
    public Sprite nullSprite;

    public GameObject facePanel;
    public GameObject boatPanel;
    public GameObject wavePanel;

    private int faceIndex = 0;
    private int faceCount = 5;
    private int boatIndex = 0;
    private int boatCount = 5;
    private int waveIndex = 0;
    private int waveCount = 5;

    public Text listText;

    public int[] faceSoulPrice = { 0, 500, 1000, 2000, 5000 };
    public int[] boatSoulPrice = { 0, 500, 1000, 2000, 5000 };
    public int[] waveSoulPrice = { 0, 500, 500, 500, 500 };

    private bool skipRunning = false;

    private enum PanelState { Face, Boat, Wave }
    private PanelState panelState;


    void Start()
    {
        InitStoreAsync();
        InitFaceCharge();
        InitBoatCharge();
        InitWaveCharge();
        panelState = PanelState.Face;
    }


    void Update()
    {
        // Store가 열려있을 때 CurrentCustom을 계속 추적함
        if (seeingStore)
        {
            UpdateCurrentCustom();
        }
    }


    public void OnBtnPanelState(int onClickPanel) // onClickPanel - 0:face, 1:boat, 2:wave
    {
        switch (onClickPanel)
        {
            case 0:
                panelState = PanelState.Face;
                facePanel.SetActive(true);
                if(boatPanel.activeInHierarchy) boatPanel.SetActive(false);
                if(wavePanel.activeInHierarchy) wavePanel.SetActive(false);
                UpdateFaceSprite();
                InitFaceCharge();
                break;
            case 1:
                panelState = PanelState.Boat;
                if (facePanel.activeInHierarchy) facePanel.SetActive(false);
                boatPanel.SetActive(true);
                if (wavePanel.activeInHierarchy) wavePanel.SetActive(false);
                UpdateBoatSprite();
                InitBoatCharge();
                break;
            case 2:
                panelState = PanelState.Wave;
                if (facePanel.activeInHierarchy) facePanel.SetActive(false);
                if (boatPanel.activeInHierarchy) boatPanel.SetActive(false);
                wavePanel.SetActive(true);
                UpdateWaveSprite();
                InitWaveCharge();
                break;
        }
    }


    public void Skip(int arrow) // arrow - 0:Left, 1:Right
    {
        if (skipRunning) return; // skip중이면 무시

        // Left Skip
        if (arrow == 0)
        {
            if (panelState == PanelState.Face)
            {
                faceIndex -= 1;
                if (faceIndex < 0) faceIndex = 0;
                else StartCoroutine(SkipLeft(facePanel));
                listText.text = faceIndex + 1 + " / " + faceCount;
            }
            else if (panelState == PanelState.Boat)
            {
                boatIndex -= 1;
                if (boatIndex < 0) boatIndex = 0;
                else StartCoroutine(SkipLeft(boatPanel));
                listText.text = boatIndex + 1 + " / " + boatCount;
            }
            else
            {
                waveIndex -= 1;
                if (waveIndex < 0) waveIndex = 0;
                else StartCoroutine(SkipLeft(wavePanel));
                listText.text = waveIndex + 1 + " / " + waveCount;
            }
        }
        // Right Skip
        else
        {
            if (panelState == PanelState.Face)
            {
                faceIndex += 1;
                if (faceIndex > faceCount - 1) faceIndex = faceCount - 1;
                else StartCoroutine(SkipRight(facePanel));
                listText.text = faceIndex + 1 + " / " + faceCount;
            }
            else if (panelState == PanelState.Boat)
            {
                boatIndex += 1;
                if (boatIndex > boatCount - 1) boatIndex = boatCount - 1;
                else StartCoroutine(SkipRight(boatPanel));
                listText.text = boatIndex + 1 + " / " + boatCount;
            }
            else
            {
                waveIndex += 1;
                if (waveIndex > waveCount - 1) waveIndex = waveCount - 1;
                else StartCoroutine(SkipRight(wavePanel));
                listText.text = waveIndex + 1 + " / " + waveCount;
            }
        }
    }


    IEnumerator SkipLeft(GameObject panel)
    {
        skipRunning = true;
        for(int i=0; i<9; i++)
        {
            panel.transform.localPosition += new Vector3(100, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        skipRunning = false;
    }


    IEnumerator SkipRight(GameObject panel)
    {
        skipRunning = true;
        for (int i=0; i<9; i++)
        {
            panel.transform.localPosition -= new Vector3(100, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        skipRunning = false;
    }


    private void UpdateFaceSprite()
    {
        listText.text = faceIndex + 1 + " / " + faceCount;

    }


    private void UpdateBoatSprite()
    {
        listText.text = boatIndex + 1 + " / " + boatCount;

    }


    private void UpdateWaveSprite()
    {
        listText.text = waveIndex + 1 + " / " + waveCount;

    }


    public void InitStoreAsync()
    {
        PlayerInformation.customs = databaseManager.GetCurrentCustom();
        // customs[0] : boat, 1 : face, 2: wave
    }


    public void InitBoatCharge()
    {
        for (int i = 0; i < boatChargeList.Length; i++)
        {
            boatChargeList[i] = databaseManager.BoatCharge(i);
        }
        UpdateLock();
    }


    public void InitFaceCharge()
    {
        for (int i = 0; i < faceChargeList.Length; i++)
        {
            faceChargeList[i] = databaseManager.FaceCharge(i);
        }
        UpdateLock();
    }


    public void InitWaveCharge()
    {
        for (int i = 0; i < waveChargeList.Length; i++)
        {
            waveChargeList[i] = databaseManager.WaveCharge(i);
        }
        UpdateLock();
    }


    public Text tmp;
    public void UpdateCurrentCustom()
    {
        // boat
        if (PlayerInformation.customs[0] == 0)
        {
            boatImage.sprite = boatSprites[0];
        }
        else if(PlayerInformation.customs[0] == 1)
        {
            boatImage.sprite = boatSprites[1];
        }
        else if (PlayerInformation.customs[0] == 2)
        {

        }
        else if (PlayerInformation.customs[0] == 3)
        {

        }
        else if (PlayerInformation.customs[0] == 4)
        {

        }


        // face
        if (PlayerInformation.customs[1] == 0)
        {
            faceImage.sprite = faceSprites[0];
        }
        else if(PlayerInformation.customs[1] == 1)
        {
            faceImage.sprite = faceSprites[1];
        }
        else if (PlayerInformation.customs[1] == 2)
        {
            
        }
        else if (PlayerInformation.customs[1] == 3)
        {
            
        }
        else if (PlayerInformation.customs[1] == 4)
        {
            
        }


        // wave
        if (PlayerInformation.customs[2] == 0)
        {
            // default wave particle
        }
        else if (PlayerInformation.customs[2] == 1)
        {
            
        }
        else if (PlayerInformation.customs[2] == 2)
        {
            
        }
        else if (PlayerInformation.customs[2] == 3)
        {
            
        }
        else if (PlayerInformation.customs[2] == 4)
        {
            
        }
    }

    
    public void UpdateLock()
    {
        CheckEquip();
        for (int i=0; i<boatCount; i++)
        {
            if (boatChargeList[i] == -1) boatLockImage[i].sprite = lockImage;
            else if (boatChargeList[i] == 0)
            {
                boatLockImage[i].sprite = nullSprite;
                boatPrice[i].text = " ";
            }
            else if (boatChargeList[i] == 1)
            {
                boatLockImage[i].sprite = checkImage;
                boatPrice[i].text = " ";
            }
            else boatLockImage[i].sprite = storeLoading;
        }
        
        for (int i = 0; i < faceCount; i++)
        {
            if (faceChargeList[i] == -1) faceLockImage[i].sprite = lockImage;
            else if (faceChargeList[i] == 0)
            {
                faceLockImage[i].sprite = nullSprite;
                facePrice[i].text = " ";
            }
            else if (faceChargeList[i] == 1)
            {
                faceLockImage[i].sprite = checkImage;
                facePrice[i].text = " ";
            }
            else faceLockImage[i].sprite = storeLoading;
        }

        for (int i = 0; i < waveCount; i++)
        {
            if (waveChargeList[i] == -1) waveLockImage[i].sprite = lockImage;
            else if (waveChargeList[i] == 0)
            {
                waveLockImage[i].sprite = nullSprite;
                wavePrice[i].text = " ";
            }
            else if (waveChargeList[i] == 1)
            {
                waveLockImage[i].sprite = checkImage;
                wavePrice[i].text = " ";
            }
            else waveLockImage[i].sprite = storeLoading;
        }
    }


    public void OnBtnEquip()
    {
        if (skipRunning) return;

        if (panelState == PanelState.Face)
        {
            for(int i=0; i<faceCount; i++)
            {
                if(faceIndex == i)
                {
                    if(faceChargeList[i] == -1) // 구매 X
                    {
                        menuManager.OnBtnChargePanel(); // 구매 패널 띄우기
                    }
                    else if(faceChargeList[i] == 0) // 구매 O, 장착 X
                    {
                        faceChargeList[i] = 1;
                        PlayerInformation.customs[1] = i; // 장착하기
                    }
                    break;
                }
            }
        }
        else if(panelState == PanelState.Boat)
        {
            for (int i=0; i<boatCount; i++)
            {
                if (boatIndex == i)
                {
                    if (boatChargeList[i] == -1) // 구매 X
                    {
                        menuManager.OnBtnChargePanel();
                    }
                    else if (boatChargeList[i] == 0) // 구매 O, 장착 X
                    {
                        boatChargeList[i] = 1;
                        PlayerInformation.customs[0] = i;
                    }
                    break;
                }
            }
        }
        else if(panelState == PanelState.Wave)
        {
            for (int i=0; i < waveCount; i++)
            {
                if (waveIndex == i)
                {
                    if (waveChargeList[i] == -1) // 구매 X
                    {
                        menuManager.OnBtnChargePanel();
                    }
                    else if (waveChargeList[i] == 0) // 구매 O, 장착 X
                    {
                        waveChargeList[i] = 1;
                        PlayerInformation.customs[2] = i;
                    }
                    break;
                }
            }
        }
        databaseManager.SetCurrentCustom(PlayerInformation.customs);
        UpdateLock();
    }
    

    public void CheckEquip()
    {
        int boat = PlayerInformation.customs[0];
        int face = PlayerInformation.customs[1];
        int wave = PlayerInformation.customs[2];

        for(int i=0; i<faceCount; i++)
        {
            if(faceChargeList[i] == 1 && face != i)
            {
                faceChargeList[i] = 0;
            }
        }

        for (int i = 0; i < boatCount; i++)
        {
            if (boatChargeList[i] == 1 && boat != i)
            {
                boatChargeList[i] = 0;
            }
        }

        for (int i = 0; i < waveCount; i++)
        {
            if (waveChargeList[i] == 1 && wave != i)
            {
                waveChargeList[i] = 0;
            }
        }
    }


    public void OnBtnPurchasing()
    {
        if (!menuManager.seeingChargePanel) return;

        if (panelState == PanelState.Face)
        {
            for (int i = 0; i < faceCount; i++)
            {
                if (faceIndex == i)
                {
                    if(PlayerInformation.SoulMoney > faceSoulPrice[i])
                    {
                        databaseManager.SetChargeNewData("face", i);
                        InitFaceCharge();
                        faceChargeList[i] = 0;
                        UpdateLock();
                        menuManager.OnBtnChargeNo();
                        PlayerInformation.SoulMoney -= faceSoulPrice[i];
                        databaseManager.UpdateMoney(-faceSoulPrice[i]);
                    }
                    else
                    {
                        menuManager.ChargeText.text = "Don't have" + "\nenough money";
                    }
                    break;
                }
            }
        }
        else if (panelState == PanelState.Boat)
        {
            for (int i = 0; i < boatCount; i++)
            {
                if (boatIndex == i)
                {
                    if (PlayerInformation.SoulMoney > boatSoulPrice[i])
                    {
                        databaseManager.SetChargeNewData("boat", i);
                        InitBoatCharge();
                        boatChargeList[i] = 0;
                        UpdateLock();
                        menuManager.OnBtnChargeNo();
                        PlayerInformation.SoulMoney -= boatSoulPrice[i];
                        databaseManager.UpdateMoney(-boatSoulPrice[i]);
                    }
                    else
                    {
                        menuManager.ChargeText.text = "Don't have" + "\nenough money";
                    }
                    break;
                }
            }
        }
        else if (panelState == PanelState.Wave)
        {
            for (int i = 0; i < waveCount; i++)
            {
                if (waveIndex == i)
                {
                    if (PlayerInformation.SoulMoney > waveSoulPrice[i])
                    {
                        databaseManager.SetChargeNewData("wave", i);
                        InitWaveCharge();
                        waveChargeList[i] = 0;
                        UpdateLock();
                        menuManager.OnBtnChargeNo();
                        PlayerInformation.SoulMoney -= waveSoulPrice[i];
                        databaseManager.UpdateMoney(-waveSoulPrice[i]);
                    }
                    else
                    {
                        menuManager.ChargeText.text = "Don't have" + "\nenough money";
                    }
                    break;
                }
            }
        }
    }
}