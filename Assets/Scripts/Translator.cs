using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Translator : MonoBehaviour
{

    private TextMeshProUGUI _textToTranslateUi;
    private TextMeshProUGUI _currentTranslationTextUi;

    [SerializeField]
    private Button _undoButton;

    private Transform _flagPole;

    [SerializeField]
    private Transform _translationContent;

    private List<Word> _words;

    private Word _currentWord;

    private int _wordLength = 0;
    private int _maxWordLength = 3;

    private float _timeLeft = 90;
    private TextMeshProUGUI _timerText;


    [SerializeField]
    private GameObject _flagOnPolePrefab;

    [SerializeField]
    private GameObject _translationPrefab;

    [SerializeField]
    private AudioManager _audioManager;

    [SerializeField]
    private AudioClip _victoryAudioClip;
    [SerializeField]
    private AudioClip _gameOverAudioClip;
    [SerializeField]
    private AudioClip _correctAudioClip;
    [SerializeField]
    private AudioClip _errorAudioClip;
    [SerializeField]
    private AudioClip _flagSelectedAudioClip;

    private List<string> _phrasesToTranslate = new()
    {
        "Retreat now",
        "Go west",
        "Go east",
        "Slow immediately",
        "500"
    };

    private int _currentPhraseToTranslateIndex;

    private bool _currentlySpelling = false;




    private Dictionary<string, List<Translation>> _translations = new()
    {
        ["78"] = new List<Translation>() { new() { Value = "Retreat", Known = true } },
        ["74"] = new List<Translation>() { new() { Value = "Go", Known = true } },
        ["144"] = new List<Translation>() { new() { Value = "North", Known = true } },
        ["244"] = new List<Translation>() { new() { Value = "West", Known = true } },
        ["54"] = new List<Translation>() { new() { Value = "East", Known = true } },
        ["194"] = new List<Translation>() { new() { Value = "South", Known = true } },
        ["49"] = new List<Translation>() { new() { Value = "Slow", Known = true } },
        ["40"] = new List<Translation>() { new() { Value = "Now", Known = true }, new() { Value = "Immediately", Known = false } },
    };

    //TODO Disable special flags where appropriate

    private void Start()
    {
        PopulateTranslations(true);
        _textToTranslateUi = transform.Find("Text to Translate").GetComponent<TextMeshProUGUI>();
        _currentTranslationTextUi = transform.Find("Current Translation/Current Translation").GetComponent<TextMeshProUGUI>();
        _currentTranslationTextUi.text = "";
        _timerText = transform.Find("Corner/Timer").GetComponent<TextMeshProUGUI>();
        _words = new List<Word>();
        _flagPole = transform.Find("Flag Pole");

        var flags = GetComponentInChildren<FlagsGrid>().GetFlags();
        foreach (var flag in flags)
        {
            flag.GetComponentInChildren<Button>().onClick.AddListener(delegate { AddFlag(flag.FlagData); });
        }

        _currentPhraseToTranslateIndex = 0;
        LoadText();
    }

    private void Translate()
    {
        // Allow player to automatically finish a word

        FinishWord();

        var targetWords = _textToTranslateUi.text.Split(" ");

        if (targetWords.Count() != _words.Count)
        {
            Debug.Log("Failed to translate as number of words is not the same");
            _words.Clear();
            _currentTranslationTextUi.color = Color.red;
            return;
        }

        var successfulTranslation = true;
        var translatedWords = new List<string>();

        for (var i = 0; i < targetWords.Length; i++)
        {
            var targetWord = targetWords[i];
            var enteredWord = _words[i];
            if (enteredWord.Numeric)
            {
                if (int.TryParse(targetWord, out var result))
                {
                    if (result != int.Parse(enteredWord.Value))
                    {
                        Debug.Log($"{enteredWord.Value} is not equal to {result}");
                        successfulTranslation = false;
                    }
                }
                else
                {
                    Debug.Log($"{targetWord} is not a number");
                    successfulTranslation = false;
                }
                translatedWords.Add(_words[i].Value);

            }
            else if (enteredWord.Spelt)
            {
                if (enteredWord.Value.ToLower() == targetWord.ToLower())
                {
                    translatedWords.Add(enteredWord.Value);
                }
                else
                {
                    Debug.Log($"{enteredWord.Value} does not equal {targetWord}");
                    successfulTranslation = false;
                }
            }
            else
            {
                if (_translations.TryGetValue(enteredWord.Value, out var translation))
                {
                    var match = translation.FirstOrDefault(t => t.Value.ToLower() == targetWords[i].ToLower());

                    if (match == null)
                    {
                        successfulTranslation = false;
                        translatedWords.Add(translation.FirstOrDefault(t => t.Known)?.Value ?? _words[i].Value);
                    }
                    else
                    {
                        if (!match.Known)
                        {
                            match.Known = true;
                            Debug.Log("Found an alias");
                            PopulateTranslations();
                        }
                    }
                }
                else
                {
                    Debug.Log($"{_words[i]} not found in dictionary");
                    successfulTranslation = false;
                    translatedWords.Add(_words[i].Value);
                }
            }
        }

        Debug.Log($"Expected: {string.Join(" ", targetWords)},  Actual: {string.Join(" ", translatedWords)}, Successful: {successfulTranslation}");

        Debug.Log($"Translate {string.Join(" ", _words)}");

        _words.Clear();

        if (successfulTranslation)
        {
            _audioManager.Play(_correctAudioClip);
            _currentPhraseToTranslateIndex += 1;
            if (_currentPhraseToTranslateIndex == _phrasesToTranslate.Count())
            {
                _audioManager.Play(_victoryAudioClip);
                Debug.Log("You have translated all phrases, congrats");
            }
            else
            {
                LoadText();
            }
        }
        else
        {
            _audioManager.Play(_errorAudioClip);
            _currentTranslationTextUi.color = Color.red;
        }

    }

    private void FinishWord()
    {
        Debug.Log("Finished word");


        if (_currentWord != null)
        {
            if (_currentWord.IsLetter)
            {
                var lastWord = _words.LastOrDefault();
                if (lastWord != null && lastWord.Spelt && _currentlySpelling)
                {
                    lastWord.Value += _currentWord.LetterValue;
                }
                else
                {
                    _words.Add(new Word() { Spelt = true, Value = _currentWord.LetterValue });
                    _currentlySpelling = true;
                }
            }
            else
            {
                _currentlySpelling = false;
                _words.Add(_currentWord);
            }

            // Translate
            var targetWords = _textToTranslateUi.text.Split(" ");

            if (_currentWord.Numeric)
            {
                _currentTranslationTextUi.text += $"{_currentWord.Value} ";
            }
            else if (_currentWord.IsLetter)
            {
                _currentTranslationTextUi.text += _currentWord.LetterValue;
            }
            else
            {
                if (_translations.TryGetValue(_currentWord.Value, out var translation))
                {
                    if (_words.Count < targetWords.Count())
                    {
                        var match = translation.FirstOrDefault(t => t.Value.ToLower() == targetWords[_words.Count - 1].ToLower());
                        if (match == null)
                        {
                            _currentTranslationTextUi.text += $"{translation.FirstOrDefault(t => t.Known)?.Value ?? _currentWord.Value} ";
                        }
                        else
                        {
                            _currentTranslationTextUi.text += $"{match.Value} ";
                        }
                    }
                    else
                    {
                        _currentTranslationTextUi.text += $"{translation.FirstOrDefault(t => t.Known)?.Value ?? _currentWord.Value} ";
                    }
                }
                else
                {
                    _currentTranslationTextUi.text += $"{_currentWord.Value} ";
                }
            }
        }

        _currentWord = null;
        _wordLength = 0;
    }

    private void AddFlag(FlagData flagData)
    {
        _currentTranslationTextUi.color = Color.white;
        _audioManager.Play(_flagSelectedAudioClip);

        if (_currentWord == null)
        {
            if (_words.Count == 0)
            {
                _currentTranslationTextUi.text = "";
            }

            if (flagData.SignalType == SignalType.Value)
            {
                _currentWord = new Word();
                while (_flagPole.childCount != 0)
                {
                    DestroyImmediate(_flagPole.GetChild(0).gameObject);
                }
            }
        }

        var flagOnPole = Instantiate(_flagOnPolePrefab, _flagPole);
        flagOnPole.GetComponent<Image>().sprite = Sprite.Create(flagData.Image, new Rect(0, 0, flagData.Image.width, flagData.Image.height), Vector2.zero);

        switch (flagData.SignalType)
        {
            case SignalType.Value:
                _currentWord.Value += flagData.Value;
                _wordLength += 1;
                break;
            case SignalType.Finishing:
                Translate();
                break;
            case SignalType.EndOfWord:
                FinishWord();
                break;
            case SignalType.Numeric:
                _currentWord.Numeric = true;
                FinishWord();
                break;
            case SignalType.EndOfSpelling:
                FinishWord();
                if (_currentlySpelling)
                {
                    _currentTranslationTextUi.text += " ";
                }
                _currentlySpelling = false;
                break;
        }

    }

    public void UndoLastWord()
    {
        Debug.Log("Undo Last Word");
        if (_currentWord != null)
        {
            _currentWord = null;
            _wordLength = 0;
        }
        else if (_words.Count > 0)
        {
            if (_currentlySpelling)
            {
                var spellingWord = _words.Last();
                spellingWord.Value = spellingWord.Value[..(spellingWord.Value.Count() - 1)];

                if (spellingWord.Value == "")
                {
                    _currentlySpelling = false;
                    _words.Remove(spellingWord);
                }
            }
            else
            {
                _words.RemoveAt(_words.Count - 1);
            }
        }

        _currentTranslationTextUi.text = "";

        for (var i = 0; i < _words.Count; i++)
        {
            var word = _words[i];
            if (word.Spelt || word.Numeric)
            {
                _currentTranslationTextUi.text += $"{word.Value} ";
            }
            else
            {

                // Translate
                var targetWords = _textToTranslateUi.text.Split(" ");

                if (_translations.TryGetValue(_currentWord.Value, out var translation))
                {
                    if (_words.Count < targetWords.Count())
                    {
                        var match = translation.FirstOrDefault(t => t.Value.ToLower() == targetWords[_words.Count - 1].ToLower());
                        if (match == null)
                        {
                            _currentTranslationTextUi.text += $"{translation.FirstOrDefault(t => t.Known)?.Value ?? _currentWord.Value} ";
                        }
                        else
                        {
                            _currentTranslationTextUi.text += $"{match.Value} ";
                        }
                    }
                    else
                    {
                        _currentTranslationTextUi.text += $"{translation.FirstOrDefault(t => t.Known)?.Value ?? _currentWord.Value} ";
                    }
                }
                else
                {
                    _currentTranslationTextUi.text += $"{_currentWord.Value} ";
                }
            }
        }

        while (_flagPole.childCount != 0)
        {
            DestroyImmediate(_flagPole.GetChild(0).gameObject);
        }

    }


    private void Update()
    {
        // Limit max word length
        var flags = GetComponentInChildren<FlagsGrid>().GetFlags();
        foreach (var flag in flags)
        {
            if (flag.FlagData.SignalType == SignalType.Value)
            {
                var button = flag.GetComponentInChildren<Button>();
                button.interactable = _wordLength < _maxWordLength;
            }
        }
        _undoButton.interactable = !(_currentWord == null && _words.Count == 0);

        if (_timeLeft == 0.0f)
        {
            return;
        }

        _timeLeft = Math.Max(0.0f, _timeLeft - Time.deltaTime);
        TimeSpan time = TimeSpan.FromSeconds(_timeLeft);
        _timerText.text = time.ToString(@"mm\:ss");
        if (_timeLeft == 0.0f)
        {
            Debug.Log("Ran out of time");
            _audioManager.Play(_gameOverAudioClip);
        }

    }

    private void LoadText()
    {
        _textToTranslateUi.text = _phrasesToTranslate[_currentPhraseToTranslateIndex];
        _currentTranslationTextUi.text = "";

    }

    private void PopulateTranslations(bool initial = false)
    {
        if (initial)
        {
            for (var c = 'A'; c <= 'Z'; c++)
            {
                var number = (c - 'A') + 1;
                _translations[number.ToString()] = new List<Translation>() { new() { Value = c.ToString(), Known = true } };
            }
        }
        else
        {
            while (_translationContent.childCount != 0)
            {
                DestroyImmediate(_translationContent.GetChild(0).gameObject);
            }
        }

        foreach (var item in _translations)
        {
            var translation = Instantiate(_translationPrefab, _translationContent);

            var translationText = string.Join("/\n", item.Value.Where(t => t.Known).Select(t => t.Value));
            translation.GetComponent<TextMeshProUGUI>().text = $"{item.Key} = {translationText}";

        }

    }

    [Serializable]
    public class Translation
    {
        public bool Known;
        public string Value;
    }

    public class Word
    {
        public string Value;
        public bool Numeric;
        // TODO overloading Letters and Words 
        public bool Spelt;

        public bool IsLetter => Value != "" && int.Parse(Value) >= 1 && int.Parse(Value) <= 26 && !Numeric && !Spelt;
        public string LetterValue => IsLetter ? ((char)('A' + (int.Parse(Value) - 1))).ToString() : "";

    }

}
