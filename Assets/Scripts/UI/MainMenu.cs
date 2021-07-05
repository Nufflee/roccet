
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Dropdown exampleDropdown;
    public InputField pathInput;

    public string mainScene = "SampleScene";

    private string filename;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void loadFromPath()
    {
        filename = pathInput.text;
        load();
    }

    public void loadFromExample()
    {
        int value = exampleDropdown.value;
        filename = Application.dataPath + "/flight" + (value + 2).ToString() + ".csv";
        load();
    }

    void load()
    {
        print(filename);
        PlayerPrefs.SetString("filepath", filename);
        SceneManager.LoadScene(mainScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
