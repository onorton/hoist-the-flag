using UnityEngine;
using UnityEngine.UI;

public class TutorialScreen : MonoBehaviour
{

    private Button _closeButton;
    private Translator _translator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _closeButton = transform.Find("Close Button").GetComponent<Button>();
        _translator = FindAnyObjectByType<Translator>();
        Debug.Log(_closeButton);
        _closeButton.onClick.AddListener(() => _translator.StartLevel());
        _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
}
