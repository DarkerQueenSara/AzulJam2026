using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuzzControllerSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable InvertIf
// ReSharper disable SuggestVarOrType_Elsewhere

[RequireComponent(typeof(BuzzInput))]
public class MainSceneManager : MonoBehaviour
{
    private static readonly int Reveal = Animator.StringToHash("Reveal");
    private BuzzInput _buzzInput;

    public int deathCap;
    
    //there will be a list here with questions, but they should be
    //ScriptableObjects so the logic comes with them
    public string discussionString;
    public string betTargetString;
    public string betTypeString;

    public List<TextMeshProUGUI> scoreTexts;
    public List<Image> aliveImages;
    public List<Image> deadImages;

    public TextMeshProUGUI commandText;
    private Animator _commandTextAnimator;

    public GameObject votesHolder;
    private List<Animator> _votesAnimators;
    private List<TextMeshProUGUI> _votesText;

    public GameObject redButtonHolder;
    
    private List<Image> _redButtons = new List<Image>();
    
    private int[] _scores;

    //Two numbers: the player that voted, and who they voted for
    //they come in at different orders
    private List<KeyValuePair<int, int>> _votes;
    //A bet has a target and a type, and is associated with one player
    private List<KeyValuePair<int, KeyValuePair<int, int>>> _bets;
    
    private List<int> _recentlyDeceased;
    private bool[] _voted;
    private bool[] _canBet;
    private int _currentBetter;
    private bool[] _deadPlayers;
    private bool[] _readyPlayers;

    private int _currentBetTarget;
    private int _currentBetType;
    
    private bool _votingActive;
    
    private bool _bettingActive;

    private static string NumToColor(int i)
    {
        return i switch
        {
            1 => "Blue",
            2 => "Orange",
            3 => "Green",
            4 => "Yellow",
            _ => ""
        };
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _scores = new int[4];
        _voted = new bool[4];
        _canBet = new bool[4];
        _votes = new List<KeyValuePair<int, int>>();
        _deadPlayers = new bool[4];
        _readyPlayers = new bool[4];
        _recentlyDeceased = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            _scores[i] = 0;
            _redButtons.Add(redButtonHolder.transform.GetChild(i).GetComponent<Image>());
        }
        
        _commandTextAnimator = commandText.GetComponent<Animator>();
        _buzzInput = GetComponent<BuzzInput>();

        _votesAnimators = new List<Animator>();
        for (int i = 0; i < votesHolder.transform.childCount; i++)
        {
            _votesAnimators.Add(votesHolder.transform.GetChild(i).GetComponent<Animator>());
        }
        
        _votesText = new List<TextMeshProUGUI>();
        foreach (var t in _votesAnimators)
        {
            _votesText.Add(t.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        }

        NewRound();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_votingActive)
        {
            if (_votes.Count == 4)
            {
                _votingActive = false;
                DisplayResults();
            }

            //Update Player 1
            if (!_voted[0])
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (_buzzInput.GetButtonDown(0, (BuzzInput.BuzzButton)i))
                    {
                        _votes.Add(new KeyValuePair<int, int>(0, i));
                        _voted[0] = true;
                        AddVote(_votes.Count -1, i);
                    }
                }
            }

            //Update Player 2
            if (!_voted[1])
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (_buzzInput.GetButtonDown(1, (BuzzInput.BuzzButton)i))
                    {
                        _votes.Add(new KeyValuePair<int, int>(1, i));
                        _voted[1] = true;
                        AddVote(_votes.Count - 1, i);
                    }
                }
            }

            //Update Player 3
            if (!_voted[2]) {
                for (int i = 1; i <= 4; i++)
                {
                    if (_buzzInput.GetButtonDown(2, (BuzzInput.BuzzButton)i))
                    {
                        _votes.Add(new KeyValuePair<int, int>(2, i));
                        _voted[2] = true;
                        AddVote(_votes.Count - 1, i);
                    }
                }
            }

            //Update Player 4
            if (!_voted[3])
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (_buzzInput.GetButtonDown(3, (BuzzInput.BuzzButton)i))
                    {
                        _votes.Add(new KeyValuePair<int, int>(3, i));
                        _voted[3] = true;
                        AddVote(_votes.Count - 1, i);
                    }
                }
            }
        }
        
        if (_bettingActive)
        {
            //Update Player 1
            if (_canBet[0] && _currentBetter == 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (_buzzInput.GetButtonDown(0, (BuzzInput.BuzzButton)i))
                    {
                        if (!_deadPlayers[i - 1] && i != 1)
                        {
                            _bettingActive = false;

                            //place bet that's twofold, so there will be an if here
                        }
                    }
                }
            }

            //Update Player 2
            if (_canBet[1] && _currentBetter == 1)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (_buzzInput.GetButtonDown(1, (BuzzInput.BuzzButton)i))
                    {
                        if (!_deadPlayers[i - 1] && i != 2)
                        {
                            _bettingActive = false;

                            //place bet that's twofold, so there will be an if here
                        }
                    }
                }
            }

            //Update Player 3
            if (_canBet[2] && _currentBetter == 2) {
                for (int i = 1; i <= 4; i++)
                {
                    if (_buzzInput.GetButtonDown(2, (BuzzInput.BuzzButton)i))
                    {

                        if (!_deadPlayers[i - 1] && i != 3)
                        {
                            _bettingActive = false;

                            //place bet that's twofold, so there will be an if here
                        }
                    }
                }
            }

            //Update Player 4
            if (_canBet[3] && _currentBetter == 3)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (_buzzInput.GetButtonDown(3, (BuzzInput.BuzzButton)i))
                    {
                        if (!_deadPlayers[i - 1] && i != 4)
                        {
                            _bettingActive = false;

                            //place bet that's twofold, so there will be an if here
                        }
                    }
                }
            }
        }
    }
    
    //Logic TODO
    //get random question

    //apply score
    //allow bet
    //go to discuss phase
    //start new round when all ready

    private void CleanUp()
    {
        _recentlyDeceased.Clear();
        _votes.Clear();
        _voted = Enumerable.Repeat(false, 4).ToArray();

        foreach (TextMeshProUGUI text in _votesText)
        {
            text.text = "";
            text.gameObject.SetActive(false);
        }

        foreach (Animator animator in _votesAnimators)
        {
            animator.gameObject.SetActive(false);
            animator.Rebind();
            animator.Update(0);
        }

    }
    
    private void NewRound()
    {
        commandText.text = "Who should win 1 piece of evidence?";
        commandText.gameObject.SetActive(true);
        _commandTextAnimator.Rebind();
        _commandTextAnimator.Update(0f);
        StartCoroutine(EnableVoting(3));
    }

    private IEnumerator EnableVoting(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        votesHolder.SetActive(true);
        _votingActive = true;
    }

    private void AddVote(int numberVote, int value)
    {
        _votesAnimators[numberVote].transform.gameObject.SetActive(true);
        _votesAnimators[numberVote].Rebind();
        _votesAnimators[numberVote].Update(0);

        _votesText[numberVote].text = NumToColor(value);
    }
    
    private void DisplayResults()
    {
        for (int i = 0; i < _votesAnimators.Count; i++)
        {
            _votesAnimators[i].SetTrigger(Reveal);
            StartCoroutine(ShowVote(3, i));
        }

        List<int> votesCount = _votes.Select(t => t.Value).ToList();

        int winner = votesCount.GroupBy(i=>i).OrderByDescending(grp=>grp.Count())
            .Select(grp=>grp.Key).First();
        
        StartCoroutine(ShowWinner(3, winner));

    }
    
    private IEnumerator ShowVote(float seconds, int index)
    {
        yield return new WaitForSeconds(seconds);
        _votesText[index].gameObject.SetActive(true);
    }
    
    private IEnumerator ShowWinner(float seconds, int winner)
    {
        yield return new WaitForSeconds(seconds);

        for (int i = 0; i < _votes.Count; i++)
        {
            if (_votes[i].Value != winner)
            {
                _votesAnimators[i].gameObject.SetActive(false);
            }
        }
        
        // yield return new WaitForSeconds(seconds);
        // apply score to people
        
        yield return new WaitForSeconds(seconds);
        CheckDead();
        
        yield return new WaitForSeconds(seconds);
        PlaceBetTargets();
    }

    private void CheckDead()
    {
        for (int i = 0; i < _scores.Length; i++)
        {
            if (!_deadPlayers[i] && _scores[i] >= deathCap)
            {
                _recentlyDeceased.Add(i);
                _deadPlayers[i] = true;
                aliveImages[i].gameObject.SetActive(false);
                deadImages[i].gameObject.SetActive(true);
            }
        }
    }

    private void PlaceBetTargets()
    {
        //nao pode ser só ciclo, tem de haver mecanismo para ser só um a dar bet de cada vez
        for (int i = 0; i < _recentlyDeceased.Count; i++)
        {
            _canBet[i] = true;
            commandText.gameObject.SetActive(false);
            _commandTextAnimator.Rebind();
            _commandTextAnimator.Update(0);
            commandText.text = betTargetString.Replace("X", NumToColor(_recentlyDeceased[i]));
            commandText.gameObject.SetActive(true);
            _bettingActive = true;
            _currentBetter = i;
        }
    }
    
    private void PlaceBetType()
    {
        //nao pode ser só ciclo, tem de haver mecanismo para ser só um a dar bet de cada vez
        for (int i = 0; i < _recentlyDeceased.Count; i++)
        {
            _canBet[i] = true;
            commandText.gameObject.SetActive(false);
            _commandTextAnimator.Rebind();
            _commandTextAnimator.Update(0);
            commandText.text = betTypeString;
            commandText.gameObject.SetActive(true);
            _bettingActive = true;
            _currentBetter = i;
        }
    }
}
