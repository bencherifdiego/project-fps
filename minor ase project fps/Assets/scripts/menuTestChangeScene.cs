using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuTestChangeScene : MonoBehaviour
{
    public Button btn;

    private void Start()
    {
        btn.onClick.AddListener(change);
    }

    void change()
    {
        SceneManager.LoadScene("1v1 test");
    }
}
