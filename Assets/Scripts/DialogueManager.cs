using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    private Queue<(string, DialogueState)> sentences;
    public TMP_Text dialogueText;
    [Range(0.01f, 0.09f)]
    [SerializeField] private float typingSpeed = 0.02f;
    private bool isTyping = false;
    public Animator animator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the instance alive across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures there's only one instance
        }
    }

    private void Start()
    {
        sentences = new Queue<(string, DialogueState)>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting dialogue");
        dialogue.SyncStates();

        sentences.Clear();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            sentences.Enqueue((dialogue.sentences[i], dialogue.states[i]));
        }

        displayNextSentence();
    }
    public void displayNextSentence()
    {
        if (!isTyping)
        {
            Debug.Log("animation");
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            var (sentence, state) = sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence, state));
        }
        else
        {
            Debug.Log("skipped");
            typingSpeed = 0;
        }
    }

    IEnumerator TypeSentence(string sentence, DialogueState state)
    {
        float skipSpeed = typingSpeed;
        isTyping = true;
        dialogueText.text = "";

        // Handle the state (e.g., change animation based on state)
        HandleState(state);

        for (int i = 0; i < sentence.Length; i++)
        {
            dialogueText.text += sentence[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingSpeed = skipSpeed;
    }

    void HandleState(DialogueState state)
    {
        switch (state)
        {
            case DialogueState.serio:
                animator.Play("CharacterSad");
                break;
            case DialogueState.normal:
                animator.Play("CharacterNormal");
                break;
            case DialogueState.sorprendido:
                animator.Play("CharacterSurprised");
                break;
            case DialogueState.feliz:
                animator.Play("CharacterHappy");
                break;
            case DialogueState.duda:
                animator.Play("CharacterDoubt");
                break;
        }
    }

    void EndDialogue()
    {
        LevelManager.Instance.LoadNextLevel();
        Debug.Log("End of conversation");
    }
}