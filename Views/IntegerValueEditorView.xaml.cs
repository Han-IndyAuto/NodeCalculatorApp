using NodeCalculatorApp.ViewModels;
using ReactiveUI;
using System.Windows;
using System.Windows.Controls;

namespace NodeCalculatorApp.Views
{
    /// <summary>
    /// IntegerValueEditorView.xaml에 대한 상호 작용 논리
    /// 정수 값을 보여 주고 편집할 수 있는 작은 UI 컴포넌트 입니다.
    /// ReactiveUI 라이브러리를 사용하여 뷰모델의 Value 프로퍼티와 UI 컨트롤(아마도 숫자를 올리고 내리는 컨트롤)의 값을 
    /// 아주 간결하게 양방향으로 바인딩하는 것이 이 코드의 핵심
    /// 
    /// 
    /// IViewFor<IntegerValueEditorViewModel>: **ReactiveUI**의 핵심 인터페이스입니다. 
    /// 이 뷰가 IntegerValueEditorViewModel 타입을 위해 특별히 만들어진 전용 뷰임을 명시하는 계약과 같습니다. 
    /// 이 인터페이스를 구현해야 WhenActivated 같은 ReactiveUI의 자동화 기능을 사용할 수 있습니다.
    /// </summary>
    public partial class IntegerValueEditorView : UserControl, IViewFor<IntegerValueEditorViewModel>
    {
        #region ViewModel
        //이 뷰가 어떤 뷰모델과 연결될지를 정의하며, WPF와 ReactiveUI의 표준적인 구현 방식
        // ViewModel 프로퍼티를 WPF의 특별한 시스템인 **의존성 프로퍼티(Dependency Property)**로 등록
        // 이렇게 해야 XAML에서 데이터 바인딩을 하거나, 스타일을 적용하는 등 WPF의 강력한 기능들을 사용할 수 있습니다.
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(IntegerValueEditorViewModel), typeof(IntegerValueEditorView), new PropertyMetadata(null));

        // 코드에서 실제로 사용할 ViewModel 프로퍼티입니다. 내부적으로 GetValue와 SetValue를 호출하여 위에서 등록한 의존성 프로퍼티의 값을 읽고 씁니다.
        public IntegerValueEditorViewModel ViewModel
        {
            get => (IntegerValueEditorViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        // IViewFor 인터페이스가 요구하는 ViewModel 프로퍼티를 명시적으로 구현한 부분
        // ReactiveUI는 이 제네릭하지 않은 object 타입의 프로퍼티를 통해 어떤 뷰모델이든 공통적으로 접근할 수 있습니다.
        // 이 코드는 단순히 위에서 만든 강력한 타입의 ViewModel 프로퍼티로 값을 전달하거나 받아오는 다리 역할을 합니다.
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (IntegerValueEditorViewModel)value;
        }


        #endregion


        // XAML에서 이 뷰를 생성할 때 호출되는 생성자
        public IntegerValueEditorView()
        {
            InitializeComponent();

            // **ReactiveUI**의 핵심 기능 중 하나
            // 이 뷰가 화면에 표시될 때(Activated) 중괄호 안의 코드를 실행하고,
            // 화면에서 사라질 때(Deactivated) 관련 리소스(메모리 등)를 자동으로 정리해 줍니다.
            // 메모리 누수를 방지하는 매우 편리한 기능입니다.

            // this.Bind: ReactiveUI가 제공하는 바인딩 메서드
            // 첫 번째 인자 (ViewModel): 데이터의 소스 객체, 즉 뷰모델을 지정
            // 두 번째 인자 (vm => vm.Value): 뷰모델에서 바인딩할 프로퍼티를 지정합니다. 여기서는 ViewModel의 Value 프로퍼티입니다.
            // 세 번째 인자 (v => v.valueUpDown.Value): 뷰에서 바인딩할 UI 컨트롤의 프로퍼티를 지정합니다.
            // valueUpDown은 XAML 파일에 x:Name="valueUpDown"으로 정의된 숫자 입력 컨트롤일 것이며, 그 컨트롤의 Value 프로퍼티에 연결합니다.
            // 결론: 이 한 줄의 코드로 **ViewModel.Value**와 UI 컨트롤 valueUpDown.Value 사이에 양방향(Two-Way) 바인딩이 설정됩니다.
            // 사용자가 UI에서 숫자를 바꾸면 -> ViewModel.Value가 자동으로 업데이트됩니다.
            // 코드에서 ViewModel.Value를 바꾸면 -> UI의 숫자 표시가 자동으로 업데이트됩니다.
            this.WhenActivated(d => d(
                this.Bind(ViewModel, vm => vm.Value, v => v.valueUpDown.Value)
            ));
        }
    }
}
