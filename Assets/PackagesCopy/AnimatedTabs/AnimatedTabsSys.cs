using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTabsSys : MonoBehaviour
{
    public int StartPageID = 0;


    public GameObject TabsHolder;
    public GameObject[] Tabs;


    public GameObject Page1;
    public GameObject Page2;

    public bool Animated = false;
    public Animation Anim;
    public GameObject RayBlocker;

    int Qveve = -1;
    bool InAnim = false;
    int ActualPage;
    int LastPage;

    private void Start()
    {
        MapGlobals.Instance.AnimatedTabsSys = this;

        for (int i = 0; i < Tabs.Length; i++)
        {
            Tabs[i].transform.SetParent(TabsHolder.transform);
            Tabs[i].SetActive(false);
        }
        ActualPage = StartPageID;
        LastPage = StartPageID;
        Tabs[ActualPage].SetActive(true);
    }

    void Update()
    {

        if (!InAnim)
        {
            if (Qveve > -1)
            {
                SetPage(Qveve);
                Qveve = -1;
            }
        }
        RayBlocker.SetActive(InAnim);
    }

    public void SetPage(int page)
    {
        SetPage(page, true, true);
    }

    public void SetPage(int page, bool state)
    {
        SetPage(page, false, state);
    }


    public void SetPage(int page, bool toggled = true, bool state = true)
    {
        
        if ((state == false)&&(toggled == false))
        {
            SetPage(0);
            return;
        }
        
        if (ActualPage == page)
        {
            if (ActualPage == 0) return;

            if (toggled == false)
                return;

            SetPage(0);
            return;
        }

        if (InAnim)
        {
            Qveve = page;
            return;
        }

        ActualPage = page;
        for (int i = 0; i < Tabs.Length; i++)
        {
            Tabs[i].transform.SetParent(TabsHolder.transform);
            Tabs[i].SetActive(false);
        }

        Tabs[LastPage].transform.SetParent(Page1.transform); Tabs[LastPage].transform.position = Page1.transform.position;
        Tabs[ActualPage].transform.SetParent(Page2.transform); Tabs[ActualPage].transform.position = Page2.transform.position;

        Tabs[LastPage].SetActive(true);
        Tabs[ActualPage].SetActive(true);


        Tabs[LastPage].transform.localScale = new Vector3(1, 1, 1);
        Tabs[ActualPage].transform.localScale = new Vector3(1, 1, 1);
        InAnim = true;
        if (Animated)
        {
            Anim.Play();
        }
        else
        {
            AnimEnd();
        }
    }

    public void SetLastPage()
    {
        SetPage(LastPage);
    }

    public void AnimEnd()
    {
        for (int i = 0; i < Tabs.Length; i++)
        {
            Tabs[i].transform.SetParent(TabsHolder.transform);
            Tabs[i].SetActive(false);
        }

        Tabs[ActualPage].SetActive(true);

        LastPage = ActualPage;
        InAnim = false;
    }
}
