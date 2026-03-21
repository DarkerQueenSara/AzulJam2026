using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{

    //there will be a list here with questions, but they should be
    //ScriptableObjects so the logic comes with them
    public string discussionString;
    public string betTargetString;
    public string betTypeString;
    
    public TextMeshProUGUI commandText;
    private Animator _commandTextAnimator;
    
    public GameObject redButtonHolder;
    
    private List<Image> _redButtons = new List<Image>();
    
    private int[] _scores;
    //Two numbers: the player that voted, and who they voted for
    //they come in at different orders
    private List<KeyValuePair<int, int>> _votes;
    private bool[] _deadPlayers;
    private bool[] _readyPlayers;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _scores = new int[4];
        _votes = new List<KeyValuePair<int, int>>();
        _deadPlayers = new bool[4];
        _readyPlayers = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            _scores[i] = 0;
            _redButtons.Add(redButtonHolder.transform.GetChild(i).GetComponent<Image>());
        }
        
        _commandTextAnimator = commandText.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Logic TODO
    //get random question
    //display and trigger question animation
    //collect votes till there's = number of players
    //reveal votes with animation
    //check winner with tie resolving
    //apply score
    //check dead
    //allow bet
    //go to discuss phase
    //start new round when all ready

    private void NewRound()
    {
        
    }

    private void CheckForAdvance()
    {
        
    }
}
