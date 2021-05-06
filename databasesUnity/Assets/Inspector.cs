using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inspector : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] InputField name = null;
    [SerializeField] InputField id = null;
    [SerializeField] InputField city = null;
    [SerializeField] InputField country;
    [SerializeField] InputField note;

    void Start()
    {
       
    }

    private void Awake()
    {
        // set input field text to selected entity if possible
        
        name.text = FrontDesk.ddat.name;
        id.text = FrontDesk.ddat.id.ToString();
        city.text = FrontDesk.ddat.city;
        country.text = FrontDesk.ddat.country;
        note.text = FrontDesk.ddat.note;
    }

    public void ExportDat()
    {
       FrontDesk.ddat.name = name.text;
        FrontDesk.ddat.name = name.text;
        FrontDesk.ddat.name = name.text;
        FrontDesk.ddat.name = name.text;
        FrontDesk.ddat.name = note.text;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
