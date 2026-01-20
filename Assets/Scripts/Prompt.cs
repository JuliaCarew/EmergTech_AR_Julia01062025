using System.Linq;
using TMPro;
using UnityEngine;

public class Prompt : MonoBehaviour
{
    // adjectives
    private string[] promptOne = 
        { "Big", "Small", "Happy", "Sad", "Lonely", "Giant", "Tiny",
        "Fast", "Slow", "Calm", "Ugly", "Beautiful", "Loud", "Quiet", "Bald",
        "Fancy", "Clean", "Polite", "Obnoxious", "Itchy", "Grumpy", "Scary", "Elegant"};
    // nouns
    private string[] promptTwo = 
        { "Butterfly", "Cat", "Fish", "Helicopter", "Dorito", 
        "Snowman", "Tree", "Bird", "House", "Flower", "Frog",
        "Banana", "Flamingo", "Family", "Planet", "Picnic", 
        "River", "Forest", "Mountain", "Uncle", "Stars"};
    
    public TextMeshProUGUI promptTxt;

    private void Start()
    {
        ShowPrompt();
    }

    public void ShowPrompt()
    {
        // get random
        string newPromptOne = promptOne[Random.Range(0, promptOne.Length)];
        string newPromptTwo = promptTwo[Random.Range(0, promptTwo.Length)];

        // set text
        promptTxt.text = newPromptOne + " " + newPromptTwo;
    }
}