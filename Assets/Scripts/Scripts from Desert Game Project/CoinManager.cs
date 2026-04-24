using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int coinCount;
    public TMP_Text coinText; // assign a TextMeshPro - Text (UI) component in the Inspector

    void Start()
    {
        coinCount = 0;
    }

    void Update()
    {
        if (coinText != null)
        {
            coinText.text = coinCount.ToString() + ": ";
        }
    }
}
