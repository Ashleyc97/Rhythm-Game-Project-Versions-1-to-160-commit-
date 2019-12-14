﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;


public class BeatmapRanking : MonoBehaviour
{
    // Animators
    public GameObject[] buttonFlashAnimatorArray = new GameObject[10];

    // UI
    public TMP_ColorGradient xColorGradient, pColorGradient, sColorGradient, aColorGradient, bColorGradient, cColorGradient, dColorGradient, eColorGradient;

    public TMP_Dropdown leaderboardSortDropDown;

    public Scrollbar leaderboardScrollbar;
    public Texture2D englandFlagTexture;

    public Button personalBestButton;
    public GameObject[] leaderboardButtonContainer = new GameObject[10]; // All leaderboard buttons empty gameobjects
    public Button[] leaderboardProfileButton = new Button[10];
    public TextMeshProUGUI personalBestButtonUsernameText, personalBestButtonScoreText, personalBestButtonInformationText, personalBestButtonModText,
        personalBestButtonGradeText, personalBestNoRankingSetText;
    public GameObject personalBestButtonContainer;
    public Image[] rankedButtonFlagImage = new Image[10];
    public Image[] rankedButtonRankImage = new Image[10];
    public Image personalBestImage;
    public Image personalBestFlagImage;
    public GameObject personalBestRank;

    public GameObject loadingIcon;

    // Bools
    private bool notChecked;
    private bool hasPersonalBest;
    private bool hasCheckedPersonalBest;
    private bool hasLoadedLeaderbaord;
    private bool hasLoadedImages;
    private bool hasCheckedPlayerProfiles;
    private bool[] placeExists;
    private bool[] placeChecked;
    private bool completeLeaderboardReady;
    private bool playLeaderboardFlashAnimation;
    private bool playFullLeaderboardFlashAnimation;
    private bool fullLeaderboardFlashAnimationFinished;

    public LeaderboardButton[] leaderboardButtonArray = new LeaderboardButton[10];
    public Transform leaderboardButtonContentTransform;

    // Vector3
    private Vector3 defaultLeaderboardScrollbarPosition, offscreenLeaderboardScrollbarPosition;

    // Strings
    private string username;
    public string leaderboardTableName;
    private string player_id;
    private string personalBestScore, personalBestCombo, personalBestPercentage,
        personalBestGrade, personalBestUsername, personalBestMod;
    public List<string>[] placeLeaderboardData;
    public List<string> personalBestLeaderboardData;
    private string[] rankedButtonUsername, rankedButtonCombo, rankedButtonPercentage, rankedButtonGrade, rankedButtonScore, rankedButtonMod;

    // Integers
    private sbyte leaderboardPlaceToGet;
    private sbyte totalRankingPlacements;
    public int totalPlacesChecked, totalLeaderboardPlacementsUpdated, totalImagesUpdated, totalURLImagesUpdated;

    // Colors
    public Color ssColor, sColor, aColor, bColor, cColor, dColor, eColor, fColor, defaultColor;

    // Material
    public Material defaultMaterial;

    // Scripts
    private ScriptManager scriptManager;

    // Properties
    public string[] RankedButtonUsername
    {
        get { return rankedButtonUsername; }
    }

    public bool[] PlaceExists
    {
        get { return placeExists; }
    }

    public bool CompleteLeaderboardReady
    {
        get { return completeLeaderboardReady; }
    }

    void Start()
    {
        // Initialize
        placeExists = new bool[10];
        placeChecked = new bool[10];

        hasPersonalBest = false;
        hasCheckedPersonalBest = false;
        hasLoadedLeaderbaord = false;
        notChecked = true;
        hasCheckedPlayerProfiles = false;
        playLeaderboardFlashAnimation = true;
        playFullLeaderboardFlashAnimation = true;
        fullLeaderboardFlashAnimationFinished = false;

        placeLeaderboardData = new List<string>[10];
        personalBestLeaderboardData = new List<string>();
        rankedButtonUsername = new string[10];
        rankedButtonCombo = new string[10];
        rankedButtonPercentage = new string[10];
        rankedButtonGrade = new string[10];
        rankedButtonScore = new string[10];
        rankedButtonMod = new string[10];

        totalImagesUpdated = 0;
        totalLeaderboardPlacementsUpdated = 0;
        totalPlacesChecked = 0;
        totalRankingPlacements = 10;
        leaderboardPlaceToGet = 1;
        totalURLImagesUpdated = 0;

        defaultLeaderboardScrollbarPosition = leaderboardScrollbar.transform.position;
        offscreenLeaderboardScrollbarPosition = new Vector3(2000, 2000, 0);

        // Reference
        scriptManager = FindObjectOfType<ScriptManager>();

        // Instantiate the lists
        for (int i = 0; i < placeLeaderboardData.Length; i++)
        {
            placeLeaderboardData[i] = new List<string>();
        }

        // GET ALL LEADERBAORD BUTTON REFERENCES 
        for (int i = 0; i < totalRankingPlacements; i++)
        {
            leaderboardButtonArray[i] = leaderboardButtonContentTransform.GetChild(i).GetComponent<LeaderboardButton>();

            buttonFlashAnimatorArray[i] = leaderboardButtonArray[i].transform.GetChild(2).gameObject;

            // Update placement text
            leaderboardButtonArray[i].placementText.text = (i + 1).ToString() + ".";

            // Create and assign new image material
            leaderboardButtonArray[i].profileImage.material = new Material(Shader.Find("UI/Unlit/Transparent"));
        }

        // Reset the leaderboard at the start
        ResetLeaderboard();
    }

    void Update()
    {
        // If the leaderboard placements and personal best has not been checked yet
        if (notChecked == true && hasCheckedPersonalBest == false)
        {
            // Reset 
            totalPlacesChecked = 0;

            // Retrieve all placement information for the leaderboard
            for (sbyte placementToCheck = 0; placementToCheck < totalRankingPlacements; placementToCheck++)
            {
                // Retrieve top players
                StartCoroutine(RetrieveBeatmapLeaderboard(leaderboardPlaceToGet));
                // Increment the placement to get
                leaderboardPlaceToGet++;
            }

            // Retrieve personal best information
            StartCoroutine(RetrievePersonalBest());

            // Set to false as the leaderbaord placements have now been checked
            notChecked = false;
        }

        // If the leaderboard placements and personal best have been checked and retrieved, and the leaderboard has not updated yet
        if (notChecked == false && hasCheckedPersonalBest == true && hasLoadedLeaderbaord == false && totalPlacesChecked == totalRankingPlacements)
        {
            // Reset

            // Loop through all the placements
            for (sbyte placementToCheck = 0; placementToCheck < totalRankingPlacements; placementToCheck++)
            {
                // If placement information was found for the position on the leaderboard
                if (placeExists[placementToCheck] == true)
                {
                    // Assign the database information to the variables
                    rankedButtonScore[placementToCheck] = placeLeaderboardData[placementToCheck][0];
                    rankedButtonCombo[placementToCheck] = placeLeaderboardData[placementToCheck][1];
                    rankedButtonUsername[placementToCheck] = placeLeaderboardData[placementToCheck][2];
                    rankedButtonGrade[placementToCheck] = placeLeaderboardData[placementToCheck][3];
                    rankedButtonPercentage[placementToCheck] = placeLeaderboardData[placementToCheck][4];
                    rankedButtonMod[placementToCheck] = placeLeaderboardData[placementToCheck][5];

                    // Update the username text for the placement on the leaderbaord
                    leaderboardButtonArray[placementToCheck].playernameText.text = rankedButtonUsername[placementToCheck].ToUpper();
                    leaderboardButtonArray[placementToCheck].scoreText.text = rankedButtonScore[placementToCheck];
                    leaderboardButtonArray[placementToCheck].comboAndPercentageText.text = rankedButtonCombo[placementToCheck] + "x | " + 
                        rankedButtonPercentage[placementToCheck] + "%";
                        

                    // Check if a mod was used, update the text with mod or without mod text
                    if (string.IsNullOrEmpty(rankedButtonMod[placementToCheck]) == false)
                    {
                        leaderboardButtonArray[placementToCheck].skillText.text = rankedButtonMod[placementToCheck];
                    }
                    else
                    {
                        leaderboardButtonArray[placementToCheck].skillText.text = "-";
                    }

                    // Update the flag
                    leaderboardButtonArray[placementToCheck].flagImage.material.mainTexture = englandFlagTexture;

                    // Set grade
                    leaderboardButtonArray[placementToCheck].rankText.text = rankedButtonGrade[placementToCheck];
                    // Set grade text color
                    leaderboardButtonArray[placementToCheck].rankText.colorGradientPreset = SetGradeColor(rankedButtonGrade[placementToCheck]);
                }

                totalLeaderboardPlacementsUpdated++;

                // Set has loaded leaderbaord to true to prevent the leaderbaord from continuing to upload every frame
                hasLoadedLeaderbaord = true;
            }

            if (MySQLDBManager.loggedIn == true)
            {
                if (hasPersonalBest == true)
                {
                    personalBestScore = personalBestLeaderboardData[0];
                    personalBestCombo = personalBestLeaderboardData[1];
                    personalBestUsername = personalBestLeaderboardData[2];
                    personalBestGrade = personalBestLeaderboardData[3];
                    personalBestPercentage = personalBestLeaderboardData[4];
                    personalBestMod = personalBestLeaderboardData[5];

                    personalBestButtonUsernameText.text = MySQLDBManager.username;
                    personalBestButtonScoreText.text = personalBestScore;
                    personalBestButtonInformationText.text = personalBestCombo + "x | " + personalBestPercentage;

                    if (string.IsNullOrEmpty(personalBestMod) == false)
                    {
                        personalBestButtonModText.text = personalBestMod;
                    }
                    else
                    {
                        personalBestButtonModText.text = "-";
                    }

                    // Update flag image
                    personalBestFlagImage.material.mainTexture = englandFlagTexture;

                    // Set grade
                    personalBestButtonGradeText.text = personalBestGrade;

                    // Set grade text color
                    personalBestButtonGradeText.colorGradientPreset = SetGradeColor(personalBestGrade);
                }
            }
        }

        // if all information has been loaded for the leaderboard
        if (notChecked == false && hasCheckedPersonalBest == true && totalLeaderboardPlacementsUpdated == totalRankingPlacements)
        {
            // If profiles have not been loaded yet
            if (hasCheckedPlayerProfiles == false)
            {
                // Reset all profiles information
                scriptManager.playerProfile.ResetPlayerProfileVariables();

                // Load player profile information
                scriptManager.playerProfile.GetPlayerProfiles();

                // Set to true to prevent reloading each frame
                hasCheckedPlayerProfiles = true;
            }
        }

        // If all player profile information has been collected and assigned to the arrays
        if (scriptManager.playerProfile.informationAssigned == true && totalImagesUpdated < scriptManager.playerProfile.TotalExistingProfiles)
        {
            LoadLeaderboardPlayerImages();
        }


        // If all images have been loaded
        if (hasLoadedImages == true)
        {
            // If all images have been uploaded
            if (totalImagesUpdated >= scriptManager.playerProfile.TotalExistingProfiles && totalURLImagesUpdated >=
                scriptManager.playerProfile.TotalURLImagesToUpload)
            {
                completeLeaderboardReady = true;
            }

            // Enable all leaderboard button containers if all information has been uploaded and all profile images have been uploaded
            if (completeLeaderboardReady == true)
            {
                // If the first button is not active
                if (leaderboardButtonArray[0].gameObject.activeSelf == false)
                {
                    // Turn off loading icon
                    loadingIcon.gameObject.SetActive(false);

                    // Activate all leaderboard button containers
                    ActivateAllLeaderboardButtons();
                }

                // Play full leaderboard flash animation
                if (playFullLeaderboardFlashAnimation == true)
                {
                    playFullLeaderboardFlashAnimation = false;
                    StartCoroutine(PlayFullLeaderboardFlashAnimation());
                }
            }
        }



        // Leaderboard animation
        if (completeLeaderboardReady == true && playLeaderboardFlashAnimation == true && fullLeaderboardFlashAnimationFinished == true)
        {
            playLeaderboardFlashAnimation = false;
            StartCoroutine(PlayLeaderboardFlashAnimation());
        }
    }

    // Play the full leaderboard flash animation
    private IEnumerator PlayFullLeaderboardFlashAnimation()
    {
        for (int i = 0; i < buttonFlashAnimatorArray.Length; i++)
        {
            buttonFlashAnimatorArray[i].gameObject.SetActive(false);
            buttonFlashAnimatorArray[i].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        playFullLeaderboardFlashAnimation = false;
        fullLeaderboardFlashAnimationFinished = true;
    }

    private IEnumerator PlayLeaderboardFlashAnimation()
    {
        for (int i = 0; i < buttonFlashAnimatorArray.Length; i++)
        {
            buttonFlashAnimatorArray[i].gameObject.SetActive(false);
            buttonFlashAnimatorArray[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);

        playLeaderboardFlashAnimation = true;
    }

    // Load all leaderboard player images
    private void LoadLeaderboardPlayerImages()
    {
        // Load leaderboard images
        if (notChecked == false && hasCheckedPersonalBest == true && totalLeaderboardPlacementsUpdated == totalRankingPlacements)
        {
            // If all images have not been loaded yet
            if (hasLoadedImages == false)
            {
                // Loop through all existing profiles on the leaderboard
                for (int i = 0; i < scriptManager.playerProfile.TotalExistingProfiles; i++)
                {
                    // If a url exists for the leaderboard spot
                    if (scriptManager.playerProfile.playerImageUrlArray[i] != "")
                    {
                        // Load the player image (passing the URL and index)
                        StartCoroutine(LoadPlayerImg(scriptManager.playerProfile.playerImageUrlArray[i], i));
                    }
                    else
                    {
                        // Load the default image
                        LoadDefaultMaterial(i);
                    }

                    // Activate leaderboard profile button
                    leaderboardButtonArray[i].profileImage.gameObject.SetActive(true);
                }

                // If a personal best record exists
                if (hasPersonalBest == true)
                {
                    // Retrieve the players image for personal best placement
                    personalBestImage.material = scriptManager.uploadPlayerImage.PlayerImage.material;
                    personalBestImage.gameObject.SetActive(false);
                    personalBestImage.gameObject.SetActive(true);
                }

                // Set to true as all images have been loaded
                hasLoadedImages = true;
            }
        }
    }

    // Load the default image
    private void LoadDefaultMaterial(int _placement)
    {
        // Set the material to default
        rankedButtonRankImage[_placement].material = defaultMaterial;

        // Set image to false then to true to activate new material
        rankedButtonRankImage[_placement].gameObject.SetActive(false);
        rankedButtonRankImage[_placement].gameObject.SetActive(true);

        // Increment
        totalImagesUpdated++;
    }

    // Load the player image
    IEnumerator LoadPlayerImg(string _url, int _placement)
    {
        if (_url != "")
        {
            if (_url != null)
            {
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        Debug.Log(uwr.error);

                        totalImagesUpdated++;
                    }
                    else
                    {
                        // Get downloaded asset bundle
                        var texture = DownloadHandlerTexture.GetContent(uwr);

                        // Update the image for the placement
                        leaderboardButtonArray[_placement].profileImage.material.mainTexture = texture;

                        // Set image to false then to true to activate new image
                        leaderboardButtonArray[_placement].profileImage.gameObject.SetActive(true);

                        totalImagesUpdated++;
                        // Increment total url images updated
                        totalURLImagesUpdated++;
                    }
                }
            }
            else
            {
                // Load the default image
                LoadDefaultMaterial(_placement);
            }
        }
    }

    // Retrieve beatmap leaderboard placement information for the position passed
    public IEnumerator RetrieveBeatmapLeaderboard(int leaderboardPlaceToGetPass)
    {
        WWWForm form = new WWWForm();
        form.AddField("leaderboardTableName", leaderboardTableName);
        form.AddField("leaderboardPlaceToGet", leaderboardPlaceToGetPass);

        UnityWebRequest www = UnityWebRequest.Post("http://rhythmgamex.knightstone.io/retrieveuserbeatmapscore.php", form);
        www.chunkedTransfer = false;
        yield return www.SendWebRequest();

        // Stores all the placement information from the database
        ArrayList placeList = new ArrayList();

        // Split the information retrieved from the database
        placeList.AddRange(Regex.Split(www.downloadHandler.text, "->"));

        // Loop through all the leaderboard data and assign
        for (sbyte dataType = 0; dataType < 6; dataType++)
        {
            /*
              DataType:
              [0] = score
              [1] = combo
              [2] = player_id
              [3] = grade
              [4] = percentage
              [5] = mod
            */

            // SUCCESS - LEADERBOARD DATA FOUND FOR THIS PLACEMENT
            if (www.downloadHandler.text != "1")
            {
                // Save to the placement data list for this placement number
                placeLeaderboardData[leaderboardPlaceToGetPass - 1].Add(placeList[dataType].ToString());
                placeExists[leaderboardPlaceToGetPass - 1] = true;
            }
            else
            {
                // ERROR - NO LEADERBOARD DATA FOR THIS PLACEMENT
                placeExists[leaderboardPlaceToGetPass - 1] = false;
            }
        }

        totalPlacesChecked++;
        placeChecked[leaderboardPlaceToGetPass - 1] = true;
    }

    // Get the leaderboard table name to load the beatmap leaderboard from
    public void GetLeaderboardTableName()
    {
        // Get it from the database gameobject
        leaderboardTableName = Database.database.LoadedLeaderboardTableName;

        // Make sure to do a check on whether the leaderboard is ranked, if it is then get the leaderboard name and enable all the leaderbaord checks, if not disable
    }

    // Retrieve the users personal best score
    public IEnumerator RetrievePersonalBest()
    {
        // Check if the user has logged in
        if (MySQLDBManager.loggedIn)
        {
            WWWForm form = new WWWForm();
            form.AddField("leaderboardTableName", leaderboardTableName);
            form.AddField("player_id", MySQLDBManager.username);


            UnityWebRequest www = UnityWebRequest.Post("http://rhythmgamex.knightstone.io/retrieveuserpersonalbest.php", form);
            www.chunkedTransfer = false;
            yield return www.SendWebRequest();

            // Stores personal best leaderboard data from the database
            ArrayList placeList = new ArrayList();

            // Splits the data retrieved
            placeList.AddRange(Regex.Split(www.downloadHandler.text, "->"));

            // Loop through all the data retrieved and assign to the personal best leaderboard data list
            for (int dataType = 0; dataType < 6; dataType++)
            {
                // If it succeeded
                if (www.downloadHandler.text != "1")
                {
                    personalBestLeaderboardData.Add(placeList[dataType].ToString());
                    hasPersonalBest = true;
                    hasCheckedPersonalBest = true;
                }
                else
                {
                    // FAILED - NO PERSONAL BEST DATA FOUND FOR THIS BEATMAP LEADERBOARD
                }
            }

            // Set to true as the personal best leaderboard information has now been checked
            hasCheckedPersonalBest = true;
        }
        else
        {
            // Set to false as no data was found for personal best
            hasPersonalBest = false;
            // Set to true as the personal best leaderboard information has now been checked
            hasCheckedPersonalBest = true;
            yield return null;
        }
    }

    // Reset the leaderboard
    public void ResetLeaderboard()
    {
        // Loop through all the placements and reset the leaderboard data for each placement
        for (sbyte placementToCheck = 0; placementToCheck < totalRankingPlacements; placementToCheck++)
        {
            // Reset variables used for the leaderboard
            rankedButtonScore[placementToCheck] = "";
            rankedButtonCombo[placementToCheck] = "";
            rankedButtonUsername[placementToCheck] = "";
            rankedButtonGrade[placementToCheck] = "";
            rankedButtonPercentage[placementToCheck] = "";
            rankedButtonMod[placementToCheck] = "";

            // Clear the leaderboard data 
            placeLeaderboardData[placementToCheck].Clear();
            // Reset all places that exist
            placeExists[placementToCheck] = false;
            // Reset all places checked
            placeChecked[placementToCheck] = false;

            // Reset all images
            personalBestImage.gameObject.SetActive(false);
            leaderboardButtonArray[placementToCheck].profileImage.gameObject.SetActive(false);

            // Null all text
            leaderboardButtonArray[placementToCheck].playernameText.text = "";
            leaderboardButtonArray[placementToCheck].scoreText.text = "";
            leaderboardButtonArray[placementToCheck].comboAndPercentageText.text = "";
            leaderboardButtonArray[placementToCheck].skillText.text = "";
            leaderboardButtonArray[placementToCheck].rankText.text = "";

            personalBestButtonUsernameText.text = "-";
            personalBestButtonScoreText.text = "-";
            personalBestButtonInformationText.text = "-";
            personalBestButtonModText.text = "-";
            personalBestButtonGradeText.text = "";
        }

        // Deactivate all leaderboard buttons
        DeactivateAllLeaderboardButtons();

        // Reset personal best information
        personalBestScore = "";
        personalBestCombo = "";
        personalBestPercentage = "";
        personalBestGrade = "";
        personalBestMod = "";

        // Reset personal best text
        personalBestButtonUsernameText.text = "";
        personalBestButtonScoreText.text = "";
        personalBestButtonInformationText.text = "";
        personalBestButtonModText.text = "";

        // Reset personal best leaderboard data
        personalBestLeaderboardData.Clear();

        // Reset the leaderbaord loading animation
        hasLoadedLeaderbaord = false;

        totalURLImagesUpdated = 0;

        // Reset scroll bar
        leaderboardScrollbar.value = 1;

        // Turn on loading icon
        loadingIcon.gameObject.SetActive(true);
    }

    // Reset the leaderboard checking variables
    public void ResetNotChecked()
    {
        // Reset leaderboard checking
        notChecked = true;
        hasPersonalBest = false;
        hasCheckedPersonalBest = false;
        hasCheckedPlayerProfiles = false;
        hasLoadedImages = false;
        playLeaderboardFlashAnimation = true;
        playFullLeaderboardFlashAnimation = true;
        fullLeaderboardFlashAnimationFinished = false;

        leaderboardPlaceToGet = 1;
        totalLeaderboardPlacementsUpdated = 0;
        totalPlacesChecked = 0;
        totalImagesUpdated = 0;
    }

    // Set the grade icon color based on the grade passed
    public TMP_ColorGradient SetGradeColor(string _grade)
    {
        switch (_grade)
        {
            case "X":
                return xColorGradient;
            case "P":
                return pColorGradient;
            case "S":
                return sColorGradient;
            case "A":
                return aColorGradient;
            case "B":
                return bColorGradient;
            case "C":
                return cColorGradient;
            case "D":
                return dColorGradient;
            case "E":
                return eColorGradient;
            default:
                return eColorGradient;
        }
    }

    // Deactivate all leaderboard buttons
    private void DeactivateAllLeaderboardButtons()
    {
        for (int i = 0; i < totalRankingPlacements; i++)
        {
            leaderboardButtonArray[i].gameObject.SetActive(false);
        }

        personalBestButton.gameObject.SetActive(false);

        // Update scrollbar position to be offscreen
        leaderboardScrollbar.transform.position = offscreenLeaderboardScrollbarPosition;
        // Turn off scroll functionality
        leaderboardScrollbar.interactable = false;

        completeLeaderboardReady = false;
    }

    private IEnumerator ActivateAllLeaderboardButtonsAnimation()
    {
        for (int i = 0; i < leaderboardButtonArray.Length; i++)
        {
            leaderboardButtonArray[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(0.02f);
        }

        personalBestButton.gameObject.SetActive(true);
    }

    // Activate all leaderboard buttons
    private void ActivateAllLeaderboardButtons()
    {
        // Control content panel and no record text
        for (int i = 0; i < totalRankingPlacements; i++)
        {
            // Activate the content panel only if place exists
            if (placeExists[i] == true)
            {
                leaderboardButtonArray[i].contentPanel.gameObject.SetActive(true);
                leaderboardButtonArray[i].noRecordSetText.gameObject.SetActive(false);
            }

            if (placeExists[i] == false)
            {
                leaderboardButtonArray[i].contentPanel.gameObject.SetActive(false);
                leaderboardButtonArray[i].noRecordSetText.gameObject.SetActive(true);
            }
        }

        if (hasPersonalBest == true)
        {
            personalBestButtonContainer.gameObject.SetActive(true);
            personalBestNoRankingSetText.gameObject.SetActive(false);
        }
        else
        {
            personalBestButtonContainer.gameObject.SetActive(false);
            personalBestNoRankingSetText.gameObject.SetActive(true);
        }

        // Turn on scroll functionality
        leaderboardScrollbar.interactable = true;

        // Update scrollbar position to be viewable
        leaderboardScrollbar.transform.position = defaultLeaderboardScrollbarPosition;

        // Play activation animation
        StartCoroutine(ActivateAllLeaderboardButtonsAnimation());
    }
}