﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public InputField usernameInputField;
    public InputField passwordInputField;
    public Text usernameFieldDescription;
    public Text passwordFieldDescription;
    public Button submitButton;
    public Button enterGameCanvas;
    public Button loginCanvas;
    public string username;
    public string password;
    public string error;

    void Update()
    {
        if (usernameInputField.isFocused)
        {
            usernameFieldDescription.gameObject.SetActive(true);
            passwordFieldDescription.gameObject.SetActive(false);
        }
        else if (passwordInputField.isFocused)
        {
            usernameFieldDescription.gameObject.SetActive(false);
            passwordFieldDescription.gameObject.SetActive(true);
        }
    }


    public void CallLogin()
    {
        StartCoroutine(LoginUser());
    }
    
    IEnumerator LoginUser()
    {
        username = usernameInputField.text;
        password = passwordInputField.text;

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post("http://rhythmgamex.knightstone.io/login.php", form);
        www.chunkedTransfer = false;
        yield return www.SendWebRequest();


        // Success
        if (www.downloadHandler.text == "0")
        {
            error = "success";
            MySQLDBManager.username = usernameInputField.text;
            DisableLoginCanvas();
            EnableEnterGameCanvas();
        }
        // Error
        if (www.downloadHandler.text == "1")
        {
            Debug.Log("error");
        }

        /*
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("error");
        }
        else
        {
            Debug.Log("Logged in");
            error = "success";
            MySQLDBManager.username = usernameInputField.text;
            DisableLoginCanvas();
            EnableEnterGameCanvas();
        }
        */
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (usernameInputField.text.Length >= 4 && usernameInputField.text.Length <= 10 && passwordInputField.text.Length >= 5);
    }

    // Disable the login canvas
    public void DisableLoginCanvas()
    {
        // Disable the register canvas
        loginCanvas.gameObject.SetActive(false);
    }

    // Enable the enter game canvas
    public void EnableEnterGameCanvas()
    {
        // Enable the enter game canvas
        enterGameCanvas.gameObject.SetActive(true);
    }
}
