using System;
using System.Linq;
using DynamicData;
using NodeCalculatorApp.ViewModels.Nodes;
using NodeNetwork.Toolkit;
using NodeNetwork.Toolkit.NodeList;
using NodeNetwork.ViewModels;
using NodeNetwork;
using ReactiveUI;
using System.Reactive.Linq;
using NodeCalculatorApp.Views;
using System.Diagnostics;

namespace NodeCalculatorApp.ViewModels
{
    
    public class MainViewModel : ReactiveObject
    {
        // 이 클래스가 프로그램에서 맨 처음 사용될 때 단 한 번만 실행되는 정적 생성자
        static MainViewModel()
        {
            // MainViewModel에 해당하는 View가 필요하면, MainWindow를 사용하라고 프레임워크에 등록.
            Splat.Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<MainViewModel>));
        }

        // 화면 왼쪽에 표시될 '사용 가능한 노드 목록'에 대한 뷰모델
        public NodeListViewModel ListViewModel { get; } = new NodeListViewModel();

        // 노드들을 끌어다 놓고 연결하는 '메인 캔버스(작업 공간)'에 대한 뷰모델
        public NetworkViewModel NetworkViewModel { get; } = new NetworkViewModel();

        // ValueLabel: 최종 계산 결과를 화면에 보여주기 위한 문자열 속성
        private string _valueLabel;
        public string ValueLabel
        {
            get => _valueLabel;

            // 새로운 값이 기존 값과 다른 경우에만 _valueLabel 필드를 업데이트하고,
            // 이 속성에 바인딩된(연결된) UI 요소에게 "값이 바뀌었으니 화면을 갱신해!" 라는 신호를 보냅니다.
            set => this.RaiseAndSetIfChanged(ref _valueLabel, value);
        }

        public MainViewModel()
        {
            ListViewModel.AddNodeType(() => new SumNodeViewModel());
            ListViewModel.AddNodeType(() => new DivisionNodeViewModel());
            ListViewModel.AddNodeType(() => new ConstantNodeViewModel());

            // 모든 계산의 최종 결과를 표시할 OutputNodeViewModel을 하나 만듭니다.
            OutputNodeViewModel output = new OutputNodeViewModel();
            // NetworkViewModel.Nodes 컬렉션에 추가하여 캔버스에 기본적으로 표시되도록 합니다.
            NetworkViewModel.Nodes.Add(output);

            // 노드 네트워크에 변경이 생길 때마다 실행될 **'유효성 검사 규칙'**을 정의합니다
            // network 매개변수에는 현재 캔버스의 전체 노드 네트워크 정보가 들어옵니다.
            NetworkViewModel.Validator = network =>
            {
                // GraphAlgorithms.FindLoops(network): NodeNetwork 라이브러리가 제공하는 기능으로,
                // 네트워크에 루프(예: A 노드 -> B 노드 -> A 노드로 다시 연결되는 순환 구조)가 있는지 찾아냅니다.
                // .Any(): 찾아낸 루프가 하나라도 있는지 확인합니다.
                bool containsLoops = GraphAlgorithms.FindLoops(network).Any();
                if(containsLoops)
                {
                    return new NetworkValidationResult(false, false, new ErrorMessageViewModel("Network contains loops"));
                }

                // 2. 0으로 나누는 노드가 있는지 검사합니다.
                // GraphAlgorithms.GetConnectedNodesBubbling(output): output 노드부터 시작해서,
                // 이 노드에 연결된 모든 입력 노드들을 거꾸로 추적해 나갑니다.
                // .OfType<DivisionNodeViewModel>(): 추적한 모든 노드 중에서 DivisionNodeViewModel(나누기 노드)만 골라냅니다.
                // .Any(n => n.Input2.Value == 0): 골라낸 나누기 노드들 각각에 대해, 두 번째 입력(Input2, 나누는 수)의 값이 0인 경우가 하나라도 있는지 검사합니다.
                bool containsDivisionByZero = GraphAlgorithms.GetConnectedNodesBubbling(output)
                    .OfType<DivisionNodeViewModel>()
                    .Any(n => n.Input2.Value == 0);
                if (containsDivisionByZero)
                {
                    // 0으로 나누는 경우 에러 메시지와 함께 유효하지 않음 반환
                    return new NetworkValidationResult(false, true, new ErrorMessageViewModel("Network contains division by zero!"));
                }


                // 위의 두 가지 오류 검사를 모두 통과하면,
                // "네트워크는 유효하고(true), 경고는 없으며(false), 별도의 메시지는 없다(null)" 라는 결과를 반환
                return new NetworkValidationResult(true, false, null);
            };

            //  '반응형 프로그래밍'의 정수를 보여주는 멋진 부분입니다! 데이터의 흐름을 정의하고 있죠.
            // output.ResultInput.ValueChanged: output 노드의 입력값(ResultInput)이 바뀔 때마다 신호를 보내는 이벤트 스트림(Observable)입니다.
            // .Select(...): 흘러 들어온 신호(새로운 값, v)를 다른 형태로 변환합니다.
            // NetworkViewModel.LatestValidation?.IsValid ?? true: 가장 최근의 유효성 검사 결과가 '유효'한지 확인합니다.
            // (? 와 ??는 검사 결과가 null일 경우를 대비한 안전장치입니다.)
            // ? v.ToString() : "Error": 만약 유효하다면, 새로운 값 v를 문자열로 바꾸고, 유효하지 않다면 "Error"라는 문자열을 사용합니다.
            // .BindTo(this, vm => vm.ValueLabel): Select를 통해 변환된 최종 결과("123" 또는 "Error")를 this(MainViewModel 자신)의 ValueLabel 속성에 **연결(바인딩)**합니다.
            output.ResultInput.ValueChanged
            .Select(v => (NetworkViewModel.LatestValidation?.IsValid ?? true) ? v.ToString() : "Error")
            .BindTo(this, vm => vm.ValueLabel);

            // 결과적으로 이 한 줄의 코드는 "출력 노드의 값이 바뀔 때마다,
            // 네트워크 유효성을 검사해서, 유효하면 그 값을, 아니면 'Error'를 ValueLabel에
            // 자동으로 업데이트해줘!" 라는 복잡한 로직을 매우 간결하게 표현한 것입니다.

            output.ResultInput.ValueChanged.Subscribe(value =>
            {
                // 디버그 출력: ResultInput의 값이 바뀔 때마다 콘솔에 변경된 값을 출력
                Debug.WriteLine($"MainViewModel: Output ResultInput value changed to {value}");
            });
        }
    }
}
