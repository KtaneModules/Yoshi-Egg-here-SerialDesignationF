using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using System.Text.RegularExpressions;

public class ScriptforYoshiEgg : MonoBehaviour
{
    static int _moduleIdCounter = 1;
    int _moduleID = 0;
    public KMBombModule module;
    public KMBombInfo Bomb;
    public MeshRenderer spottedEgg;
    public Material BlackYoshiEgg;
    public Material BlueYoshiEgg;
    public Material BrownYoshiEgg;
    public Material CyanYoshiEgg;
    public Material GrayYoshiEgg;
    public Material GreenYoshiEgg;
    public Material OrangeYoshiEgg;
    public Material PinkYoshiEgg;
    public Material PurpleYoshiEgg;
    public Material RedYoshiEgg;
    public Material TanYoshiEgg;
    public Material White; // For Flashing only 
    public Material YellowYoshiEgg;
    public TextMesh colorblind_text;
    public KMColorblindMode colorblind;
    private const float _flashSpeed = .75f;
    private bool Flag = true;
    private List<int> colors = new List<int>();
    private int product;
    private readonly List<char> colorNames = new List<char>() { '-', 'G', 'R', 'B', 'Y', 'I', 'C', 'P', 'O', 'K', 'A', 'N', 'T'};
    private readonly bool colorblindModeEnabled;
    public MeshRenderer EggMesh;
    public KMSelectable Select;
    private readonly bool colorblindEnabled;
    private readonly bool colorblindMode;
    private readonly bool colorblindModeActive;
    private readonly bool colorblindModeEnable;
    public void ButtonPress()
    {
        int bombTimeCurrent = (int)Bomb.GetTime() % 60;
        if (bombTimeCurrent == product)
        {
            Flag = false;
            colorblind_text.color = new Color(0, 0, 0, 0);
            EggMesh.material = White;
            StartCoroutine(Animation());
            module.HandlePass();
            Debug.LogFormat("[Yoshi Egg #{0}] You pressed it when the seconds digits are at {1}, it was pressed at the correct time.", _moduleID, product);
        }
        else
        {
            module.HandleStrike();
            Debug.LogFormat("[Yoshi Egg #{0}] You pressed it when the seconds digits are at {1}, it was pressed at the wrong time. It should be pressed at {2}.", _moduleID, bombTimeCurrent, product); 
        }
    }
    private IEnumerator Animation()
    {
        int rando = Rnd.Range(0, 3);
        int rando2 = Rnd.Range(0, 2);
        int rando3 = Rnd.Range(0, 3);
        float[] getYurStuff = { 0.0f, .02f, -.02f };
        while (rando == 0 && rando2 == 0 && rando3 == 0)
        {
            rando = Rnd.Range(0, 3);
            rando2 = Rnd.Range(0, 2);
            rando3 = Rnd.Range(0, 3);
        }
        for (int i = 0; i < 200; i++)
        {
            Select.transform.localPosition = new Vector3(Select.transform.localPosition.x + getYurStuff[rando], Select.transform.localPosition.y + getYurStuff[rando2], Select.transform.localPosition.z + +getYurStuff[rando3]);
            yield return new WaitForSeconds(.02f);
        }
        Select.transform.localScale = new Vector3();
    }

    public static float GetFlashSpeed()
    {
        return _flashSpeed;
    }
    void Awake()
    {
        if (colorblind.ColorblindModeActive == true)
        {
            colorblind_text.color = new Color(0, 0, 0, 1);
        }
        else
        {
            colorblind_text.color = new Color(0, 0, 0, 0);
        }
        _moduleID = _moduleIdCounter++;
        Select.OnInteract += delegate { ButtonPress(); return false; };
        colors = Enumerable.Range(0, 7).Select(_ => Rnd.Range(1, 13)).ToList();
        colors.Add(0);
        product = 0;
        foreach (int color in colors)
            product += color;
        product %= 60;
        Debug.LogFormat("[Yoshi Egg #{0}] The color flashing are {1}{2}{3}{4}{5}{6}{7}.", _moduleID, colorNames[colors[0]], colorNames[colors[1]], colorNames[colors[2]], colorNames[colors[3]], colorNames[colors[4]], colorNames[colors[5]], colorNames[colors[6]], colorNames[colors[7]]);
        Debug.LogFormat("[Yoshi Egg #{0}] The egg should be pressed when the seconds digits are: {1}.", _moduleID, product);
        StartCoroutine(Flashing());
    }


        IEnumerator Flashing()
        {
            Material[] mats = new Material[] {White, GreenYoshiEgg, RedYoshiEgg, BlueYoshiEgg, YellowYoshiEgg, PinkYoshiEgg, CyanYoshiEgg, PurpleYoshiEgg, OrangeYoshiEgg, BlackYoshiEgg, GrayYoshiEgg, BrownYoshiEgg, TanYoshiEgg, White};
            yield return new WaitForSeconds(1.0f);
            int i = 0;
            while (Flag)
            {

            spottedEgg.material = White;
                yield return new WaitForSeconds(1.0f);
                spottedEgg.material = mats[colors[i]];
            colorblind_text.text = colorNames[colors[i]].ToString();
            yield return new WaitForSeconds(1.0f);
            i++;
                i %= colors.Count();
            }
            EggMesh.material = White;
        }
    //twitch plays
    private bool IsValid(string s)
    {
        string[] valids = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"};
        if (!valids.Contains(s.ElementAt(0).ToString()))
        {
            return false;
        }
        return true;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <#> [Presses yoshi egg when last digit of bomb timer is '#']";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length != 2)
            {
                yield return "sendtochaterror yoshi egg is confused.";
            }
            else
            {
                if (IsValid(parameters[1]))
                {
                    yield return null;
                    int temp = 0;
                    int.TryParse(parameters[1], out temp);
                    while ((int)Bomb.GetTime() % 60 != temp)
                    {
                        yield return "trycancel yoshi egg was told to stop.";
                    }
                    ButtonPress();
                }
                else
                {
                    yield return "sendtochaterror yoshi egg is confused.";
                }
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        int temp = 0;
        int.TryParse("" + product, out temp);
        while (temp != (int)Bomb.GetTime() % 60)
        {
            yield return true;
            yield return new WaitForSeconds(0.01f);
        }
        ButtonPress();
    }
}
