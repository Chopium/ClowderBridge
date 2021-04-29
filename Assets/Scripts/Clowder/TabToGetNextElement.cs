using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class TabToGetNextElement : MonoBehaviour
{
    EventSystem system;

    void Start()
    {
        system = EventSystem.current;// EventSystemManager.currentSystem;

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("once"+this.gameObject);
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {

                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
            //else Debug.Log("next nagivation element not found");

        }
    }
}
//EventSystem system;
//public GameObject overrideElement;
//void Start()
//{
//    system = EventSystem.current;// EventSystemManager.currentSystem;

//}
//public bool nonuniversal = false;
//// Update is called once per frame
//void Update()
//{
//    if (Input.GetKeyDown(KeyCode.Tab) && system.currentSelectedGameObject == this.gameObject)
//    {
//        if (overrideElement == null)
//        {
//            Debug.Log("automatic tab element");
//            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

//            if (next != null)
//            {

//                InputField inputfield = next.GetComponent<InputField>();
//                if (inputfield != null)
//                    inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

//                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
//            }
//            //else Debug.Log("next nagivation element not found");
//        }
//        else
//        {
//            Debug.Log("manual tab element");
//            system.SetSelectedGameObject(overrideElement, new BaseEventData(system));
//        }


//    }
//}