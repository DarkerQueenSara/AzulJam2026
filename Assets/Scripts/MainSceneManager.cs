using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuzzControllerSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static WitchGameplay;
using Random = UnityEngine.Random;

// ReSharper disable InvertIf
// ReSharper disable SuggestVarOrType_Elsewhere

public class MainSceneManager : MonoBehaviour
{
    private static readonly int SlideIn = Animator.StringToHash("SlideIn");
    private static readonly int Reveal = Animator.StringToHash("Reveal");
    private BuzzInput _buzzInput;
    private WitchInput _input;

    [Header("Game Rules")]
    public int deathCap;

    [Header("Game Questions")]
    //there will be a list here with questions, but they should be
    //ScriptableObjects so the logic comes with them
    public List<string> questions;
    public AnimationCurve pointsToGainCurve;
    public AnimationCurve pointsToLoseCurve;
    
    [Header("UI Strings")]
    public string discussionString;
    public string betTargetString;
    public string betTypeString;
    public string allDeadString;
    public string winnerString;

    [Header("UI elements")]
    public List<TextMeshProUGUI> scoreTexts;
    public Image skulls;
    
    public TextMeshProUGUI commandText;
    private Animator _commandTextAnimator;

    public GameObject votesHolder;
    private Animator[] _votesAnimators;
    private TextMeshProUGUI[] _votesText;
    private (Animator animator, TextMeshProUGUI textmesh)[] _voteScrolls;

    public CheckReady checkReady;

    [Header("Game State")]
    private int[] _scores;

    //Two numbers: the player that voted, and who they voted for
    //they come in at different orders
    private List<(Player from, Player to)> _votes;
    //A bet has a target and a type, and is associated with one player
    private List<(Player from, Player target, Bet type)>  _bets;
    
    private List<int> _recentlyDeceased;
    private bool[] _voted;
    private bool[] _canBet;
    private int _currentBetter;
    private bool[] _deadPlayers;

    private int _currentBetTarget;
    private Bet _currentBetType;
    
    private bool _votingActive;
    
    private bool _bettingActive;

    private bool _gameOver;

    private int _currentQuestion = -1;
    private int _currentScore = 0;
    private int _currentLoss = 0;
    private int _currentVoteTarget = -1;

    private int _betsThisRound = 0;
    private bool _roundDone = false;
    private bool _advanceCalled = false;

    private static string NumToColor(int i)
    {
        return i switch
        {
            0 => "Blue",
            1 => "Orange",
            2 => "Green",
            3 => "Yellow",
            _ => ""
        };
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _scores = new int[4];
        _voted = new bool[4];
        _canBet = new bool[4];
        _bets = new List<(Player from, Player target, Bet type)>(capacity: 4);
        _votes = new List<(Player from, Player to)>(capacity: 4);
        _deadPlayers = new bool[4];
        _recentlyDeceased = new List<int>();

        _buzzInput = GetComponent<BuzzInput>();
        _commandTextAnimator = commandText.GetComponent<Animator>();
        
        var corners = votesHolder.transform.Cast<Transform>().ToArray();
        _votesAnimators = corners.Select(c => c.GetComponent<Animator>()).ToArray();
        _votesText = corners.Select(c => c.GetComponentInChildren<TextMeshProUGUI>()).ToArray();
        
        _voteScrolls = (
            from Transform corner in votesHolder.transform
            select (animator: corner.GetComponent<Animator>(),
                    textmesh: corner.GetComponentInChildren<TextMeshProUGUI>())
        ).ToArray();
    }

    private void Start()
    {
        _input = WitchInput.current;
        
        checkReady.enabled = false;
        skulls.gameObject.SetActive(false);
        
        NewRound();
    }

    // Update is called once per frame
    private void Update()
    {

        if (_recentlyDeceased.Count() == _betsThisRound && _roundDone && !_advanceCalled)
        {
            StartCoroutine(AdvanceToDiscussion());
        }

        if (_votingActive)
        {
            if (_votes.Count == 4)
            {
                skulls.gameObject.SetActive(false);
                _votingActive = false;
                StartCoroutine(DisplayResults(1.0f));
            }

            //Update Player p [0..3]
            for (int p = 0; p <= 3; p++)
            {
                if (!_voted[p])
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        if (_input.GetButtonDown(p, i))
                        {
                            _votes.Add(((Player)p, (Player)(i - 1)));
                            _voted[p] = true;
                            AddVote(_votes.Count() - 1, i - 1);
                        }
                    }
                }
            }

        }
        
        if (_bettingActive)
        {
            //Debug.Log("Betting active");
            //Update Player p
            for (int p = 0; p <= 3; p++)
            {
                //Debug.Log($"Checking player {p} for betting, canBet: {_canBet[p]}, currentBetter: {_currentBetter}");
                if (_canBet[p] && _currentBetter == p)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        //Debug.Log("Entrou no ciclo");
                        if (_input.GetButtonDown(p, i))
                        {
                            Debug.Log($"current bet target = {_currentBetTarget}  |  {!_deadPlayers[i - 1]}   |   {i != p+1}  (i={i}, p={p}, p+1={p+1})   |   {_currentBetTarget == -1}");
                            if (!_deadPlayers[i - 1] && i != p+1 && _currentBetTarget == -1)
                            {
                                Debug.Log($"Player {p} bet on player {i - 1}");
                                _currentBetTarget = i - 1;
                                StartCoroutine(WaitForBetType(0.0f));
                            }
                            else if (_currentBetTarget >= 0)
                            {
                                _currentBetType = i > 2 ? Bet.Demise : Bet.Survival;
                                Debug.Log($"Player {p} bet on player {i-1} on type {_currentBetType}");
                                _bets.Add(
                                    (from: (Player)p, target: (Player)_currentBetTarget, type: _currentBetType));
                                _canBet[p] = false;
                                _currentBetter = -1;
                                _betsThisRound++;
                                StartCoroutine(WaitForNextBeter(1.0f));
                            }
                        }
                    }
                }
            }

        }
    }
    private void CleanUp()
    {
        Debug.Log($"Entrou no CleanUp");
        checkReady.HideAllImages();
        checkReady.enabled = false;

        _recentlyDeceased.Clear();
        _votes.Clear();
        _voted = Enumerable.Repeat(false, 4).ToArray();

        foreach (TextMeshProUGUI text in _votesText)
        {
            text.text = "";
            text.gameObject.SetActive(false);
        }
        
        _currentBetTarget = -1;
        _currentVoteTarget = -1;
        _currentScore = 0;
        _currentQuestion = -1;
        _currentLoss = 0;
        


        _betsThisRound = 0;
        _roundDone = false;
        _advanceCalled = false;
        NewRound();
    }
    
    private void NewRound()
    {
        Debug.Log($"Entrou no NewRound");
        _currentQuestion = Random.Range(0, 4);
        _buzzInput.StopLightSequence();
        _buzzInput.SetLight(0, false);
        _buzzInput.SetLight(1, false);
        _buzzInput.SetLight(2, false);
        _buzzInput.SetLight(3, false);
        
        switch (_currentQuestion)
        {
            case < 2:
            {
                var auxList = _currentQuestion == 0 ? pointsToGainCurve : pointsToLoseCurve;
            
                _currentScore = (int)auxList.Evaluate(Random.value);
                commandText.text = questions[_currentQuestion]
                    .Replace("X", _currentScore.ToString())
                    .Replace("Z", (_currentScore > 1) ? "s" : "");
                break;
            }
            case 2:
                _currentVoteTarget = Random.Range(0, 4);
                commandText.text = questions[_currentQuestion]
                    .Replace("Y", NumToColor(_currentVoteTarget));
                break;
            case 3:
                _currentScore = (int)pointsToGainCurve.Evaluate(Random.value);
                _currentLoss = (int)pointsToLoseCurve.Evaluate(Random.value);;
                commandText.text = questions[_currentQuestion]
                    .Replace("X", _currentScore.ToString())
                    .Replace("O", _currentLoss.ToString())
                    .Replace("Z", (_currentScore > 1) ? "s" : "");
                break;
        }
        // preencher com nrs random e/ou target random e guardar
        commandText.gameObject.SetActive(true);
        _commandTextAnimator.Update(0f);
        StartCoroutine(EnableVoting(0f));
    }

    private IEnumerator EnableVoting(float seconds)
    {
        Debug.Log($"Entrou no EnableVoting");
        yield return new WaitForSeconds(seconds);
        _votingActive = true;
        skulls.gameObject.SetActive(true);
    }

    private void AddVote(int numberVote, int value)
    {
        Debug.Log($"Entrou no AddVote com numberVote {numberVote} e value {value}");
        _votesAnimators[numberVote].gameObject.SetActive(true);
        _votesAnimators[numberVote].Update(0);
        _votesText[numberVote].text = NumToColor(value);
    }
    
    private IEnumerator DisplayResults(float seconds)
    {
        Debug.Log($"Entrou no DisplayResults");
        yield return new WaitForSeconds(seconds);
        for (int i = 0; i < _votesAnimators.Length; i++)
        {
            _votesAnimators[i].SetTrigger(Reveal);
            StartCoroutine(ShowVote(0.5f, i));
        }

        (int numVotes, _, Player winner) = _votes
            .Zip(Enumerable.Range(0, _votes.Count), // Add vote position for breaking ties
                (vote, position) => (vote.from, vote.to, position))
            .GroupBy(vote => vote.to)
            .Max(grouping => (
                grouping.Count(), // Sort primarily by number of votes
                grouping.Min(vote => vote.position), // then by who was voted first (although we don't use this value)
                grouping.Key // and attach the voted player into the selected value
            ));
        
        StartCoroutine(ShowWinner(2.0f, winner, numVotes));
    }
    
    private IEnumerator ShowVote(float seconds, int index)
    {
        Debug.Log($"Entrou no ShowVote com index {index}");
        yield return new WaitForSeconds(seconds);
        _votesText[index].gameObject.SetActive(true);
    }
    
    private IEnumerator ShowWinner(float seconds, Player winner, int numVotes)
    {
        Debug.Log($"Entrou no ShowWinner com winner {winner} e numVotes {numVotes}");
        yield return new WaitForSeconds(seconds);

        for (int i = 0; i < _votes.Count; i++)
        {
            if (_votes[i].to != winner)
            {
                _votesAnimators[i].gameObject.SetActive(false);
            }
        }

        yield return new WaitForSeconds(1.0f);

        ScoreVote(winner, numVotes);

        // apply score to people

        yield return new WaitForSeconds(0);
        CheckDead();

        if (!_gameOver)
        {
            yield return new WaitForSeconds(0);
            PlaceBetTargets();
        }
    }

    private void ScoreVote(Player winnerPlayer, int numVotes)
    {
        Debug.Log($"Entrou no ScoreVote com winner {winnerPlayer} e numVotes {numVotes}");

        int winner = (int)winnerPlayer;
        
        switch (_currentQuestion)
        {
            case 0:
                _scores[winner] = Math.Max(0, _scores[winner] + _currentScore);
                break;
            case 1:
                _scores[winner] = Math.Max(0, _scores[winner] - _currentScore);
                break;
            case 2:
                if (numVotes == 4 && winner == _currentVoteTarget)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        _scores[i] += (i == winner) ? 2 : 1;
                    }
                } else {
                    for (int i = 0; i < 4; i++)
                    {
                        _scores[i] = Math.Max(0, _scores[i] - 1);
                    }
                }
                break;
            case 3:
                _scores[winner] = Math.Max(0, _scores[winner] + _currentScore);
                for (int i = 0; i < 4; i++)
                {
                    if (i != winner) _scores[i] = Math.Max(0, _scores[i] - _currentLoss);
                }

                break;
        }

        for (int i = 0; i < 4; i++)
        {
            scoreTexts[i].text = _scores[i].ToString();
        }
    }

    private void CheckDead()
    {
        Debug.Log($"Entrou no CheckDead");

        for (int i = 0; i < _scores.Length; i++)
        {
            if (!_deadPlayers[i] && _scores[i] >= deathCap)
            {
                _recentlyDeceased.Add(i);
                _deadPlayers[i] = true;
                scoreTexts[i].color = Color.red;
            }
        }

        // _recentlyDeceased.Any(); ...

        if (_deadPlayers.Count(p => p) >= 3)
        {
            // trigger da morte
            _gameOver = true;

            bool[] newDeads = (bool[]) _deadPlayers.Clone();
            foreach (var bet in _bets)
            {
                if (_deadPlayers[(int)bet.target] == (bet.type == Bet.Demise))
                {
                    newDeads[(int)bet.from] = false;
                }
            }

            _deadPlayers = newDeads;

            // for (int i = 0; i < _scores.Length; i++)
            // {
            // aliveImages[i].gameObject.SetActive(!_deadPlayers[i]);
            // deadImages[i].gameObject.SetActive(_deadPlayers[i]);
            // }

            if (newDeads.All(p => p))
            {
                StartCoroutine(DisplayAllLose(0.0f));
            }
            else
            {
                int highestScore = -1;
                int highestIndex = -1;
                for (int i = 0; i < 4; i++)
                {
                    if (!_deadPlayers[i])
                    {
                        if (_scores[i] > highestScore)
                        {
                            highestIndex = i;
                            highestScore = _scores[i];
                        }
                    }
                }
                StartCoroutine(DisplayWinner(0.0f, highestIndex));
            }
            StartCoroutine(UnanimousGameRestart());
        }
    }

    private IEnumerator UnanimousGameRestart()
    {
        yield return checkReady.PlayersBuzzAnyOrderAsync();
        Debug.Log("All players confirmed. Loading scene #0");
        SceneManager.LoadScene(0);
    }

    private void PlaceBetTargets()
    {
        Debug.Log($"Entrou no PlaceBetTargets com {_recentlyDeceased.Count} recentemente falecidos");
        _roundDone = true;

        foreach (var e in _votesAnimators)
        {
            e.gameObject.SetActive(false);
        }

        //nao pode ser só ciclo, tem de haver mecanismo para ser só um a dar bet de cada vez
        for (int i = 0; i < _recentlyDeceased.Count; i++)
        {
            while (!_bettingActive)
            {
                Debug.Log($"Entrou no Enabling do Bet para o jogador {i}");
                _canBet[_recentlyDeceased[i]] = true;
                commandText.gameObject.SetActive(false);
                _commandTextAnimator.Update(0);
                commandText.text = betTargetString.Replace("X", NumToColor(_recentlyDeceased[i]));
                commandText.gameObject.SetActive(true);
                _bettingActive = true;
                _currentBetter = _recentlyDeceased[i];
                _currentBetTarget = -1;
            }
        }
    }
    
    private void PlaceBetType()
    {
        Debug.Log($"Entrou no PlaceBetType");
        //nao pode ser só ciclo, tem de haver mecanismo para ser só um a dar bet de cada vez
        commandText.gameObject.SetActive(false);
        _commandTextAnimator.Update(0);
        commandText.text = betTypeString;
        commandText.gameObject.SetActive(true);
    }


    private IEnumerator WaitForBetType(float seconds)
    {
        Debug.Log($"Entrou no waitForBetType");
        _canBet[_currentBetter] = false;
        yield return new WaitForSeconds(seconds);
        PlaceBetType();
        yield return new WaitForSeconds(seconds);
        _canBet[_currentBetter] = true;
    }

    private IEnumerator WaitForNextBeter(float seconds)
    {
        Debug.Log($"Entrou no waitForNextBeter");
        yield return new WaitForSeconds(seconds);
        _bettingActive = false;
    }

    private IEnumerator AdvanceToDiscussion()
    {
        Debug.Log($"Entrou no AdvanceToDiscussion");
        _advanceCalled = true;
        checkReady.enabled = true;
        //start new round when all ready
        commandText.gameObject.SetActive(false);
        _commandTextAnimator.Update(0);
        commandText.text = discussionString;
        commandText.gameObject.SetActive(true);

        yield return checkReady.PlayersBuzzAnyOrderAsync();
        CleanUp();
    }

    private IEnumerator DisplayAllLose(float seconds)
    {
        Debug.Log($"Entrou no DisplayAllLose");
        yield return new WaitForSeconds(seconds);
        commandText.gameObject.SetActive(false);
        _commandTextAnimator.Update(0);
        commandText.text = allDeadString;
        commandText.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
    }

    private IEnumerator DisplayWinner(float seconds, int winner)
    {
        Debug.Log($"Entrou no DisplayWinner com winner {winner}");
        yield return new WaitForSeconds(seconds);
        commandText.gameObject.SetActive(false);
        _commandTextAnimator.Update(0);
        commandText.text = winnerString.Replace("X", NumToColor(winner));
        commandText.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
    }

}