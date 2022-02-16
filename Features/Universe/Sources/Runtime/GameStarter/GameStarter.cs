
using UnityEngine;
using Universe.SceneTask.Runtime;

public class GameStarter : MonoBehaviour
{
    public TaskData taskData;
    
    void Awake()
    {
        Task.ULoadTask(null, taskData);
    }   
}