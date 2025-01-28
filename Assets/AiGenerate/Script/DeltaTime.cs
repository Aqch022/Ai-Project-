using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    bool deltaFlag;
    int resultCnt = 0;
    // Update is called once per frame
    public void Update()
    {
        const int deltaCount = 1;
        resultCnt+= deltaCount;
        if(resultCnt>360)
        {
            deltaFlag = true;
           resultCnt= 0;
        }

    }
   
    public bool DeltaTimeFlag()
    {
        return deltaFlag;
    }

    public void SetDeltaFlag(bool _flag)
    {
        deltaFlag = _flag;
    }
}
