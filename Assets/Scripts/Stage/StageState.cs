namespace Stage
{
    public enum StageState
    {
        NotStart,   // 스테이지 시작 전
        Playing,    // 스테이지 실행 중
        Clear,      // 스테이지 클리어
        Over,       // 스테이지 클리어 못하고 종료
    }
}