using ReactiveUI;
using NodeNetwork.Toolkit.ValueNode;
using NodeCalculatorApp.Views;

namespace NodeCalculatorApp.ViewModels
{
    /// <summary>
    /// 뷰-뷰모델 연결: ReactiveUI의 서비스 로케이터(Splat)를 통해, 이 뷰모델이 IntegerValueEditorView라는 특정 뷰(UI)와 짝이라는 것을 프레임워크에 등록합니다.
    /// 초기값 설정: 뷰모델이 생성될 때, 편집기의 기본값을 0으로 설정합니다.
    /// </summary>
    public class IntegerValueEditorViewModel : ValueEditorViewModel<int?>
    {
        //  정적 생성자입니다. 이 IntegerValueEditorViewModel 클래스가 프로그램에서 맨 처음 사용될 때 단 한 번만 실행
        static IntegerValueEditorViewModel()
        {
            // Splat.Locator: "이 뷰모델에 해당하는 뷰는 무엇이지?" 와 같은 질문에 답해주는 일종의 전화번호부 같은 역할을 합니다.
            // .Register(...): 전화번호부에 새로운 정보를 등록하는 메서드입니다.
            // 첫 번째 인자는 뷰(View)를 생성하는 방법을 알려주는 팩토리 함수
            // 두 번째 인자는 전화번호부에서 사용할 "키" 값입니다.
            // 즉, " IntegerValueEditorViewModel에 대한 뷰를 찾아줘"라는 요청이 오면, 방금 등록한 팩토리 함수를 사용하라고 알려주는 것입니다.
            // 요약하자면, 이 한 줄은 " IntegerValueEditorViewModel의 짝꿍 UI는 IntegerValueEditorView 입니다." 라고 애플리케이션 전체에 공식적으로 알리는 역할
            Splat.Locator.CurrentMutable.Register(() => new IntegerValueEditorView(), typeof(IViewFor<IntegerValueEditorViewModel>));
        }

        public IntegerValueEditorViewModel()
        {
            Value = 0;
        }
    }
}
