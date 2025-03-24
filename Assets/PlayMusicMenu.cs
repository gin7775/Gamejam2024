using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class PlayMusicMenu : MonoBehaviour
{
    public MMFeedbacks musicFeedback;
    // Start is called before the first frame update
    void Start()
    {
        musicFeedback?.PlayFeedbacks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
