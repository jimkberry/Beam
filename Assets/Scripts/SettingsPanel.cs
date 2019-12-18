﻿using System;
using System.Collections.Generic;
using BeamBackend;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsPanel : MovableUISetItem
{  
    public GameObject screenNameField;
    public GameObject p2pConnectionField;
    public GameObject ethNodeField;
    public GameObject ethAcctField;
 

    public void LoadAndShow()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings settings = mainObj.frontend.GetUserSettings();

        screenNameField.GetComponent<InputField>().text = settings.screenName;
        p2pConnectionField.GetComponent<InputField>().text = settings.p2pConnectionString;
        ethNodeField.GetComponent<InputField>().text = settings.ethNodeUrl;
        ethAcctField.GetComponent<InputField>().text = settings.ethAcct;

        base.moveOnScreen();
    }

    public void SaveAndHide()
    {
        base.moveOffScreen();
    }



}