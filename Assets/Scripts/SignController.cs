using UnityEngine;
using UnityEngine.UI;

public class SignController : MonoBehaviour
{
    public Text signText;
    public string defaultText = "BEANS BREWS AND BLOODSHED invites players into a whimsically mysterious VR barista simulator. Amidst the comforting aroma of freshly brewed coffee and the warmth of friendly faces, a sinister murder has taken place nearby, thrusting players into the role of detective. This captivating blend of puzzle-solving and therapeutic gameplay captures the serene atmosphere of coffee shops, while seamlessly combining the suspenseful intrigue of solving a murder mystery.";
    
    private string currentLookedAtObject;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            string objectTag = hitObject.tag; // You can use tag or any other identifier

            // Check if the looked-at object has changed
            if (objectTag != currentLookedAtObject)
            {
                // Update the text sign based on the looked-at object
                switch (objectTag)
                {
                    case "Fridge":
                        signText.text = "This is where you can grab various milks or creamers.";
                        break;
                    case "Syrups":
                        signText.text = "Here you can add flavors such as Carmel, Chocolate, and Vanilla.";
                        break;
                    case "Mug":
                        signText.text = "You can use this to hold the many combinations of coffee.";
                        break;
                    case "Grinder/MilkFrother":
                        signText.text = "Here you can grind the coffee beans for the espresso and froth the milk for a creamy top.";
                        break;
                    case "SportMan":
                        signText.text = "SportMan: I hear the killers signarture move is to put the victims hair into a ponytail.";
                        break;
                    case "CollegeGirl":
                        signText.text = "CollegeGirl: Have you seen on the news that the killer is a fan of the band 'The Beatles'.";
                        break;
                    case "JacketMan":
                        signText.text = "JacketMan: My freind said the killer is someone you would never suspect.";
                        break;
                    case "EspressoMachine":
                        signText.text = "This is where you can make the espresso shots for the coffee.";
                        break;
                    // Add more cases as needed for other objects
                }

                currentLookedAtObject = objectTag;
            }
        }
        else
        {
            // If the raycast doesn't hit any objects, set the default text
            signText.text = defaultText;
            currentLookedAtObject = null;
        }
    }
}
