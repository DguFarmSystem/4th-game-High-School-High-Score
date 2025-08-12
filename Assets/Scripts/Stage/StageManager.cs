using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stage
{
    public class StageManager : Singleton<StageManager>
    {
        public void NewGame()
        {
            SceneManager.LoadScene(1);
        }

        public void ContinueGame()
        {

        }

        public void Settings()
        {
            
        }
    }
}