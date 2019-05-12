﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Login : MonoBehaviour {

    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public TextMeshProUGUI usernameFieldDescription;
    public TextMeshProUGUI passwordFieldDescription;
    public Button submitButton;
    public Button loggedInCanvas;
    public Button loginCanvas;
    public string username;
    public string password;
    public string error;

    // The loading icon for logging in and signing up
    public GameObject accountProgressIcon;

    void Update()
    {
        if (usernameInputField.isFocused)
        {

        }
        else if (passwordInputField.isFocused)
        {

        }
    }


    public void CallLogin()
    {
        StartCoroutine(LoginUser());

        // Enable the loading icon
        EnableAccountProgressLoadingIcon();
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
            EnableLoggedInCanvas();

            // Disable the loading icon
            DisableAccountProgressLoadingIcon();
        }
        // Error
        if (www.downloadHandler.text == "1")
        {
            usernameFieldDescription.gameObject.SetActive(true);
            Debug.Log("error");

            // Disable the loading icon
            DisableAccountProgressLoadingIcon();
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
        // Disable the loading icon
        DisableAccountProgressLoadingIcon();
    }

    // Enable the enter game canvas
    public void EnableLoggedInCanvas()
    {
        // Enable the enter game canvas
        loggedInCanvas.gameObject.SetActive(true);
    }

    // Disable the loading icon
    private void DisableAccountProgressLoadingIcon()
    {
        accountProgressIcon.gameObject.SetActive(false);
    }

    // Enable the loading icon
    private void EnableAccountProgressLoadingIcon()
    {
        accountProgressIcon.gameObject.SetActive(true);
    }
}
