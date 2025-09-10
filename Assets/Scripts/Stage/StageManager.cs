using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stage
{
    public class StageManager : Singleton<StageManager>
    {
        //StageState 설정 기반으로 씬 넘기기 설정
        //현재 진행되고 있는 스테이지 레벨 관리
        public static int currentLevel = 0;
    }
}