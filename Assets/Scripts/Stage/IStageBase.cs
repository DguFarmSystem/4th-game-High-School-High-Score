using System;

namespace Stage
{
    public interface IStageBase
    {
        // 스테이지가 끝났을 때 호출할 델리게이트, 인자로 true == 클리어, false == 클리어하지 못하고 종료
        public Action<bool/*isCleared*/> OnStageEnded { get; } 
        // 스테이지를 처음 생성할 때 호출할 함수
        public void OnStageStart();

    }
}