namespace HInteractions
{
    // 상호작용 가능한 오브젝트를 '선택'하고 '가지고 있을 수 있는' 객체를 위한 인터페이스
    public interface IObjectHolder
    {
        // 현재 선택한 Interactable 오브젝트 (읽기 전용 프로퍼티)
        Interactable SelectedObject { get; }
    }
}
