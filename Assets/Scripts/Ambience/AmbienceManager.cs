using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{

    public static AmbienceManager Instance;

    //管理主相机的位置
    public Camera camera;

    //相机初始位置
    private float defaultPositionY = 0f;
    private float defaultPositionZ = 0.7f;

    //相机极限位置
    private float inestPositionY = -1.5f;
    private float inestPositionZ = 1.3f;
    private float outestPositionY = 0.5f;

    //位置变化的敏感度
    private float sensity = 15f;
    private float snapInY;
    private float snapOutY;
    private float snapZ;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        #region 初始化相机位置变化的的敏感度
        snapInY = (inestPositionY - defaultPositionY) / sensity;
        snapOutY = (outestPositionY - defaultPositionY) / sensity;
        snapZ = (inestPositionZ - defaultPositionZ) / sensity;
        #endregion

        EventCenter.GetInstance().AddEventListener(MyEventType.ZOOMIN, DoZoomIn);
        EventCenter.GetInstance().AddEventListener(MyEventType.ZOOMOUT, DoZoomOut);
    }

    #region 画面的放大与缩小
    void DoZoomIn()
    {
        //在原点附近进行一次位置校准
        if(System.Math.Abs(camera.transform.position.y + snapInY - defaultPositionY) < 0.02f)
        {
            camera.GetComponent<Transform>().position = new Vector3(0, defaultPositionY,defaultPositionZ);
        }
        
        if (camera.transform.position.y > defaultPositionY)
        {
            camera.GetComponent<Transform>().position = camera.GetComponent<Transform>().position - new Vector3(0, snapOutY, 0);
        }
        else if(camera.transform.position.y > inestPositionY)
        {
            camera.GetComponent<Transform>().position = camera.GetComponent<Transform>().position + new Vector3(0, snapInY, snapZ);
        } 
    }

    void DoZoomOut()
    {
        //在原点附近校准一次位置
        if (System.Math.Abs(camera.transform.position.y + snapOutY - defaultPositionY) < 0.02f)
        {
            camera.GetComponent<Transform>().position = new Vector3(0, defaultPositionY, defaultPositionZ);
        }
        
        if (camera.transform.position.y < defaultPositionY)
        {
            camera.GetComponent<Transform>().position = camera.GetComponent<Transform>().position - new Vector3(0, snapInY, snapZ);

        }
        else if(camera.transform.position.y < outestPositionY)
        {
            camera.GetComponent<Transform>().position = camera.GetComponent<Transform>().position + new Vector3(0, snapOutY, 0);

        }
    }
    #endregion

    private void OnDestroy()
    {
        EventCenter.GetInstance().Clear();
    }

}
