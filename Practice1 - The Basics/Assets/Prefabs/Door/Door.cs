// Using statements define different libraries that we want to access within this script
using System.Collections;
using UnityEngine;

// In C#, all code is encapsulated inside a class.  This class defines how any object with this attached will behave.
// Classes are defined in the format
// scope class ClassName : OptionalDerivedClass, ...
// All code within a class is enclosed in curly braces { }
// In this instance we derive from MonoBehaviour.  Any class that should be attached to a GameObject will be derived from MonoBehaviour
// We also derive from an interface, IInteractable.  This allows us to declare functions elsewhere and have this script be called by those functions.
// This is useful when you have many objects that follow a similar pattern, i.e. they can be interacted with, or they can all be destroyed.
// Note that Unity has its own Destroy() function that we would normally use, but in some instances this action may need to be more nuanced.
public class Door : MonoBehaviour, IInteractable
{
    // Variables are defined in the format
    // scope VariableType variableName = value

    // Functions and variables have different scopes
    // private functions can only be accessed within the current class
    // public functions can be accessed by any class, provided they have a reference to an instance of this class
    // There are more scopes but these are the most common two

    // The [SerializeField] tag allows unity to see these variables in the editor.
    [SerializeField]
    // Due to unity always rotating from the center of an object, we use a parent object to define our pivot.
    private Transform pivot;

    // Manually defined where the open and closed rotations of the door are, so we have a target to move towards.
    [SerializeField]
    private Vector3 openRotation;
    [SerializeField]
    private Vector3 closedRotation;

    [SerializeField]
    // The range tag clamps the value, this shows up as a slider in the editor.
    [Range(0.1f,10.0f)]
    // Speed multiplier to fine tune how quickly the door opens and closes
    private float animationSpeed;

    // Doors start closed, this is used as a toggle.
    private bool open = false;
    // We keep track of whether the door is already moving to avoid attempting to move in both directions at once.
    private bool animationInProgress = false;

    // Functions are defined in the format
    // scope ReturnType FunctionName(OptionalParameterType optionalParameterName,...)
    // Functions enclose the code in curly braces { }
    // Functions can be called in the format FunctionName(optionalParameterName);
    private void OnTriggerStay(Collider collider)
    {
        Debug.Log("In range");
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    // The interact function is part of the IInteractable interface that has been made.
    // By deriving from IInteractable we can call this function on any interactable object, and the definition below
    // defines its behaviour.
    public void Interact()
    {
        if (animationInProgress)
            return;

        // Toggle open state.
        // ! is the symbol for NOT
        // !true == false
        open = !open;

        // Curly braces aren't required if the content that is usually enclosed is only a single line.
        if (open)
            // Coroutines are used to split a function over multiple frames.  Normally the script will attempt to run every
            // called function within a single frame.  For the sake of animations and timers we want to spread our logic
            // out over time.
            StartCoroutine(Animation(openRotation));
        else
            StartCoroutine(Animation(closedRotation));        

    }

    // Coroutines use IEnumerator to define its return type.  Specifics aren't required knowledge, just know that IEnumerator
    // means its a coroutine when found as the return type of the function.
    private IEnumerator Animation(Vector3 targetRotation)
    {
        // We define targetRotation as a Vector3 because it's easier to work with when defining rotations
        // Vector3 has (x,y,z) components.  Quaternion has (x,y,z,w).  You don't need to worry about the specifics, just know
        // that unity stores its rotations in Quaternions.  These can be converted to Vector3 using targetRotation.eulerAngles.
        // We do the opposite using Quaternion.Euler(Vector3);
        Quaternion targetRotationQuaternion = Quaternion.Euler(targetRotation);

        animationInProgress = true;

        // While loops run until the condition is met,  this can cause Unity to freeze if it never meets an exit condition
        // Our exit condition checks the angle between the target rotation and 
        while (Quaternion.Angle(pivot.rotation, targetRotationQuaternion) > 2.5f)
        {
            // Linear interpolation. Quaternion.Lerp(a,b,t). Give it two values: a, and b.  It will grab an intermediate value
            // between a and b, based on the value t. When t = 0, the value is a. When t = 1, the value is b.
            // Time.deltaTime gives us the amount of time this frame has been active.  1.0/frame rate
            pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotationQuaternion, Time.deltaTime * animationSpeed);

            // Coroutines use the yield function, which tells the coroutine to wait until the next frame.
            // If you wanted to specify a time you could use
            // yield return new WaitForSeconds(float)
            yield return null;
        }
        // Exit condition has been met, we are no longer animating
        animationInProgress = false;
        // Stop this coroutine running
        StopCoroutine("Animation");
    }
}