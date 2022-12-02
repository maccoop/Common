using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : SingletonBehaviour<UserManager>
{
    private string _accessToken;
    
    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
        GetLocalToken();
    }

    private void GetLocalToken()
    {
        if (PlayerPrefs.HasKey(nameof(_accessToken)){
            _accessToken = PlayerPrefs.GetString(nameof(_accessToken));
        }
        else
        {

        }
    }
}
