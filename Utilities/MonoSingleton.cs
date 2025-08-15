using UnityEngine;

/// <summary>
/// 同时满足Mono和单例，需要挂载到unity场景中的游戏对象上
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = true;
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance =(T)FindObjectOfType<T>();
            }
            return instance;
        }

    }

    void Start()
    {
        if (global) {
            if (instance != null && instance != this.gameObject.GetComponent<T>()) {
                //单例不为空且不是当前脚本，检测自己是否是多余的单例
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject.gameObject.GetComponent<T>(); 
        }
        this.OnStart();
    }
     
    protected virtual void OnStart()
    {

    }
}