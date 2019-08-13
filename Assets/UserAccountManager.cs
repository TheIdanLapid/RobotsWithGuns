using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;

public class UserAccountManager : MonoBehaviour {

    public static UserAccountManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    //These store the username and password of the player when they have logged in
    public static string LoggedIn_Username { get; protected set; }
    private static string LoggedIn_Password = "";

    public static bool IsLoggedIn { get; protected set; }

    public void LogOut()
    {
        LoggedIn_Username = "";
        LoggedIn_Password = "";

        IsLoggedIn = false;
    }

    public void LogIn(string username, string password)
    {
        LoggedIn_Username = username;
        LoggedIn_Password = password;

        IsLoggedIn = true;
    }

}
