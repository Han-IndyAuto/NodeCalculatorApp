using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using NodeNetwork.Toolkit.ValueNode;
using ReactiveUI;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeCalculatorApp.ViewModels.Nodes
{
    public class OutputNodeViewModel : NodeViewModel
    {
        static OutputNodeViewModel()
        {
            // Splat.Locator.CurrentMutable.Register(...): ReactiveUI의 서비스 로케이터(Splat)에 이 뷰모델과 짝이 될 뷰(View)를 등록하는 코드입니다.
            // () => new NodeView(): 이 뷰모델(OutputNodeViewModel)에 대한 UI를 생성하는 방법을 지정합니다.
            // 여기서는 NodeNetwork가 제공하는 **기본 노드 뷰(NodeView)**를 사용하라고 설정했습니다.
            // 즉, 이 노드는 특별한 커스텀 디자인 없이 표준 노드의 모양을 그대로 따릅니다.
            // typeof(IViewFor<OutputNodeViewModel>): 이 등록 정보가 OutputNodeViewModel 타입을 위한 것임을 명시합니다.
            // 결론: 이 부분은 " OutputNodeViewModel은 별도의 UI 디자인 없이, 그냥 기본 노드 모양으로 화면에 그려주세요." 라고 프레임워크에 알려주는 역할을 합니다.
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<OutputNodeViewModel>));
        }

        // 이 노드가 가질 **입력 포트(Input Port)**를 정의하는 프로퍼티입니다.
        // ValueNodeInputViewModel: NodeNetwork 툴킷이 제공하는, 단일 값을 받는 입력 포트를 위한 뷰모델입니다.
        // <int?>: 이 입력 포트가 null 값을 허용하는 정수(Nullable<int>) 타입의 데이터를 받는다는 것을 명시합니다.
        // ResultInput: 이 프로퍼티의 이름
        public ValueNodeInputViewModel<int?> ResultInput { get; }

        public OutputNodeViewModel()
        {
            // Name = "Output";: 부모 클래스(NodeViewModel)로부터 물려받은 Name 프로퍼티를 설정합니다. 이 값은 UI에서 노드의 제목으로 표시됩니다.
            Name = "Output";


            // 이 노드를 사용자가 UI에서 마우스 클릭 등으로 삭제할 수 없도록 설정합니다. 출력 노드는 보통 필수적이므로 삭제를 막는 경우가 많습니다.
            this.CanBeRemovedByUser = false;

            // 프로퍼티에 실제 입력 포트 객체를 생성하여 할당
            ResultInput = new ValueNodeInputViewModel<int?>
            {
                // 입력 포트 옆에 표시될 텍스트 라벨을 "Value"로 설정
                Name = "Value",
                // 매우 중요한 부분입니다. 🧩 이 입력 포트의 **편집기(Editor)**로 이전에 만들었던 IntegerValueEditorViewModel을 지정합니다.
                // 이 설정 덕분에, 이 입력 포트가 다른 노드와 연결되어 있지 않을 때,
                // 그 자리에 숫자를 직접 입력할 수 있는 UI(숫자 올림/내림 컨트롤)가 나타나게 됩니다.
                Editor = new IntegerValueEditorViewModel()
            };

            // NodeViewModel이 관리하는 Inputs라는 컬렉션(목록)에 방금 만든 ResultInput 포트를 추가합니다.
            // 이 코드를 실행해야 비로소 노드에 입력 포트가 시각적으로 표시되고 작동하게 됩니다.
            this.Inputs.Add(ResultInput);
        }
    }
}
