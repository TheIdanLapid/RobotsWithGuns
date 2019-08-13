using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class UserAccountManager : MonoBehaviour
{

    public static UserAccountManager instance;

    public string loggedInScene = "Lobby";
    public string loggedOutScene = "LoginMenu";
    public static string LoggedIn_Username { get; protected set; } //stores username once logged in
    private static string LoggedIn_Password = ""; //stores password once logged in

    public static bool IsLoggedIn { get; protected set; }
    public delegate void OnDataReceivedCallback(string data);

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public void LogOut()
    {
        LoggedIn_Username = "";
        LoggedIn_Password = "";
        IsLoggedIn = false;
        SceneManager.LoadScene(loggedOutScene);
    }

    public void LogIn(string _Username, string _Password)
    {
        LoggedIn_Username = _Username;
        LoggedIn_Password = _Password;
        IsLoggedIn = true;
        SceneManager.LoadScene(loggedInScene);
    }

    public void SendData(string data)
    {
        if (IsLoggedIn)
            StartCoroutine(SetData(data));
    }

    IEnumerator SetData(string data)
    {
        IEnumerator e = DCF.SetUserData(LoggedIn_Username, LoggedIn_Password, data); // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            //The data string was set correctly. Goes back to LoggedIn UI
            Debug.Log("Succes sending data");
            // loggedInParent.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Error: Unknown Error. Please try again later. Send data problem");
        }
    }

    public void GetData(OnDataReceivedCallback onDataReceived)
    {
        //Called when the player hits 'Get Data' to retrieve the data string on their account. Switches UI to 'Loading...' and starts coroutine to get the players data string from the server
        if (IsLoggedIn)
            StartCoroutine(GetData_numerator(onDataReceived));
    }

    IEnumerator GetData_numerator(OnDataReceivedCallback onDataReceived)
    {
        string data = "ERROR";
        IEnumerator e = DCF.GetUserData(LoggedIn_Username, LoggedIn_Password); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error")
        {
            Debug.Log("Error: Unknown Error. Please try again later. GetDataProblem");
        }
        else
        {
            //The player's data was retrieved. Goes back to loggedIn UI and displays the retrieved data in the InputField
            data = response;
        }

        if (onDataReceived != null)
            onDataReceived.Invoke(data);
    }
}
