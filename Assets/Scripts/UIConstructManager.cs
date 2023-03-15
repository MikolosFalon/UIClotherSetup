using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class UIConstructManager : MonoBehaviour
{
    #region Value
    //screen
    [Header ("screen")]
    private int screenIndex;
    [SerializeField] private List<GameObject> ScreenObjects;
    [SerializeField] private Animator ScreenAnimator;
    //type
    [Header("Collections items")]
    [SerializeField] private List<CollectionsItem> tShortCollections;
    private List<int> tShortColors;
    private int tShortSelected;
    [SerializeField] private List<CollectionsItem> logoCollections;
    private List<int> logoColors;
    private int logoSelected;
    [Header("Parts in screen")]
    [SerializeField] private List<CollectionsImage> tShortImage;
    [SerializeField] private List<Image> ColorTShortPartImage;
    [SerializeField] private List<CollectionsImage> logoImage;
    [SerializeField] private List<Image> ColorLogoPartImage;
    [SerializeField] private int ColorPartIndex;
    //team name
    [SerializeField] private TMP_InputField teamNameField;
    private string teamName;
    //selected indexes for collections
    //colors need for random
    [SerializeField] private List<Color> paletteColors;
    //collection for save
    private SetupData saveData;
    private SaveAndLoadManager saveAndLoadManager;
    #endregion Value

    private void Start() {
        //base
        //0 screen blocked
        //1 screen start screen
        screenIndex = 1;
        tShortSelected = 0;
        logoSelected = 0;
        ScreenObjects[screenIndex].SetActive(true);
        saveAndLoadManager = new SaveAndLoadManager();
        saveData = new SetupData();

        //set colors
        tShortColors = new List<int>();
        logoColors = new List<int>();
        for (int i = 0; i < tShortCollections[0].SpriteParts.Count-1; i++)
        {
            tShortColors.Add(0);
            logoColors.Add(0);
        }
        //fix color problem
        for (int i = 0; i < paletteColors.Count; i++)
        {
            paletteColors[i] = new Color(
                paletteColors[i].r, paletteColors[i].g, paletteColors[i].b, 255);
        }
        
        // check save
        if (File.Exists(Application.persistentDataPath + "/setup.sav"))
        {
            LoadDataSatParameters();
        }
        else{
            RandomTShort();
            RandomLogo();
        }
        ScreenAnimator.Play(ScreenObjects[screenIndex].name+"start");
    }
    // load save
    private void LoadDataSatParameters()
    {
        saveData = saveAndLoadManager.Load();

        tShortSelected = saveData.setupIndex["TShort"];
        tShortColors = saveData.setupColors["TShort"];
        for (int i = 0; i < tShortColors.Count; i++)
        {
            ColorTShortPartImage[i].color = paletteColors[tShortColors[i]];
        }

        logoSelected = saveData.setupIndex["Logo"];
        logoColors = saveData.setupColors["Logo"];
        for (int i = 0; i < logoColors.Count; i++)
        {
            ColorLogoPartImage[i].color = paletteColors[logoColors[i]];
        }
        teamName = saveData.teamName;
        teamNameField.text=teamName;

        SetItemsOnScreen(tShortSelected, tShortColors, tShortImage, tShortCollections);
        SetItemsOnScreen(logoSelected, logoColors, logoImage, logoCollections);
    }
    private void SaveDataSatParameters()
    {
        saveData.setupColors = new Dictionary<string, List<int>>();
        saveData.setupIndex = new Dictionary<string, int>();

        List<int> colorItem = new List<int>();
        foreach (var item in tShortColors)
        {
            colorItem.Add(item);
        }
        saveData.setupColors.Add("TShort", colorItem);
        saveData.setupIndex.Add("TShort", tShortSelected);


        colorItem = new List<int>();
        foreach (var item in logoColors)
        {
            colorItem.Add(item);
        }
        saveData.setupColors.Add("Logo", colorItem);
        saveData.setupIndex.Add("Logo", logoSelected);
        saveData.teamName = teamName;

        saveAndLoadManager.Save(saveData);
    }

    // random
    public void RandomTShort(){
        tShortSelected = Random.Range(0, tShortCollections.Count);
        //select colors
        for (int i = 0; i < tShortColors.Count; i++)
        {
            tShortColors[i]=Random.Range(0, paletteColors.Count);
            //set parts on panel 
            ColorTShortPartImage[i].color = paletteColors[tShortColors[i]];
        }
        //select and set 
        SetItemsOnScreen(tShortSelected, tShortColors, tShortImage, tShortCollections);

    }
    public void RandomLogo(){
        logoSelected = Random.Range(0, logoCollections.Count);
        //select colors
        for (int i = 0; i < logoCollections.Count; i++)
        {
            logoColors[i]=Random.Range(0, paletteColors.Count);
            //set parts on panel 
            ColorLogoPartImage[i].color = paletteColors[logoColors[i]];
        }
        //select and set 
        SetItemsOnScreen(logoSelected, logoColors, logoImage, logoCollections);
    }
    private void SetItemsOnScreen(int itemIndex, List<int> itemColors, 
        List<CollectionsImage> itemScreenCollection,List<CollectionsItem> itemCollection)
        {
            for (int ix = 0; ix < itemScreenCollection.Count; ix++)
            {
                for (int iy = 0; iy < itemCollection[0].SpriteParts.Count; iy++)
                {
                //set sprites
                itemScreenCollection[ix].ScreenParts[iy].sprite = 
                itemCollection[itemIndex].SpriteParts[iy];
                //set colors
                //last sprite do not change color
                if(iy < logoCollections[0].SpriteParts.Count-1)
                    {
                        itemScreenCollection[ix].ScreenParts[iy].color = 
                        paletteColors[itemColors[iy]];
                    }
                }
            }
        }
    
    //button next
    //need change for tz
    public void NextButtonScreen(){
        StartCoroutine(NextScreen());
    }

    IEnumerator NextScreen(){
        //block buttons
        ScreenObjects[0].SetActive(true);
        Debug.Log(ScreenObjects[screenIndex].name+"end");
        ScreenAnimator.Play(ScreenObjects[screenIndex].name+"end");
        yield return new WaitForSeconds(1);
        ScreenObjects[screenIndex].SetActive(false);
        screenIndex++;
        if(screenIndex >= ScreenObjects.Count){
            //reset screen index
            screenIndex = 1;
            //save
            SaveDataSatParameters();
        }
        ScreenObjects[screenIndex].SetActive(true);
        Debug.Log(ScreenObjects[screenIndex].name+"start");
        ScreenAnimator.Play(ScreenObjects[screenIndex].name+"start");
        yield return new WaitForSeconds(1);
        //unblock buttons
        ScreenObjects[0].SetActive(false);
    }

    public void NextTShort(int nextStep){
        //next t-short
        tShortSelected= Mathf.Clamp(tShortSelected+nextStep,0, tShortCollections.Count-1);
        SetItemsOnScreen(tShortSelected, tShortColors, tShortImage, tShortCollections);
    }
    public void NextLogo(int nextStep){
        //next logo
        logoSelected = Mathf.Clamp(logoSelected+nextStep, 0, logoCollections.Count-1);
        SetItemsOnScreen(logoSelected, logoColors, logoImage, logoCollections);
    }

    //color system
    public void PaletteColorButton(int indexColor){
        //screen with tShort
        if(screenIndex==1){
            ColorTShortPartImage[ColorPartIndex].color = paletteColors[indexColor];
            tShortColors[ColorPartIndex] = indexColor;
            SetItemsOnScreen(tShortSelected, tShortColors, tShortImage, tShortCollections);
        }
        //screen with logo
        if(screenIndex==2){
            ColorLogoPartImage[ColorPartIndex].color = paletteColors[indexColor];
            logoColors[ColorPartIndex] = indexColor;
            SetItemsOnScreen(logoSelected, logoColors, logoImage, logoCollections);
        }
    }
    public void PartColor(int selectPartColor){
        ColorPartIndex = selectPartColor;
    }


    //screen 3 team name
    private void Update() {
        //length text team name tz max 10 or red line
        string formatterTeamName = teamNameField.text;
        formatterTeamName = formatterTeamName.Replace("<u color=#af2216>", "");
        formatterTeamName = formatterTeamName.Replace("</u>","");
        if(formatterTeamName.Length>10){
            string teamString = teamNameField.text;
            //set stilly
            teamNameField.text="<u color=#af2216>"+formatterTeamName+"</u>";
        }else{
            //set for save
            teamName = teamNameField.text;
        }
    }
    
}
[System.Serializable]
public class CollectionsItem{
    public List<Sprite> SpriteParts;
}
[System.Serializable]
public class CollectionsImage{
    public List<Image> ScreenParts;
}

