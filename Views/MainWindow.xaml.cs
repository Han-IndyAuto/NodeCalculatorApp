using NodeCalculatorApp.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;

namespace NodeCalculatorApp.Views
{
    /// <summary>
    /// 뷰모델 생성: 애플리케이션의 모든 데이터와 로직을 관장하는 MainViewModel 객체를 생성합니다.
    /// 데이터 바인딩 설정: ReactiveUI를 사용하여 MainViewModel이 가진 데이터(예: 노드 목록, 네트워크 뷰, 결과값 텍스트)를 
    /// 화면의 각 UI 컨트롤(노드 목록 뷰, 네트워크 뷰, 텍스트 라벨 등)에 **연결(Binding)**합니다.
    /// MainViewModel의 데이터가 변경되면 화면이 자동으로 업데이트되는 반응형(Reactive) UI가 구현됩니다.
    /// 
    /// 
    /// IViewFor<MainViewModel>: **ReactiveUI**의 핵심 인터페이스입니다. 
    /// 이 MainWindow가 MainViewModel이라는 특정 뷰모델을 위한 **전용 뷰(View)**임을 명시하는 '계약'과 같습니다. 
    /// 이 계약을 맺어야 WhenActivated 같은 ReactiveUI의 마법 같은 기능들을 사용할 수 있습니다.
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainViewModel>
    {
        /// <summary>
        /// ViewModel이라는 속성을 WPF의 강력한 의존성 프로퍼티(Dependency Property) 시스템에 등록하는 과정입니다. 
        /// 이렇게 등록해야 XAML에서의 데이터 바인딩, 스타일링 등이 원활하게 작동합니다.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(MainViewModel), typeof(MainWindow), new PropertyMetadata(null));

        // IViewFor<MainViewModel> 인터페이스를 구현하고, 의존성 프로퍼티를 일반 C# 속성처럼 편리하게 사용하기 위한 표준적인 코드입니다.
        // ReactiveUI가 내부적으로 이 속성들을 사용하여 뷰와 뷰모델을 연결합니다.
        public MainViewModel ViewModel
        {
            get => (MainViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel)value;
        }

        public MainWindow()
        {
            InitializeComponent();

            //  애플리케이션의 '두뇌'를 생성하는 부분입니다.
            //  🧠 모든 데이터와 로직을 담고 있는 MainViewModel의 인스턴스를 만들어 이 창의 ViewModel 속성에 할당합니다.
            this.ViewModel = new MainViewModel();

            // this.WhenActivated(d => { ... });: ✨ ReactiveUI의 생명주기 관리 기능입니다.
            // {...} 안의 코드는 이 창이 화면에 활성화될 때(나타날 때) 실행됩니다.
            // 더 중요한 것은, 이 창이 비활성화될 때(닫힐 때) d라는 객체를 통해 여기에 등록된 모든 바인딩과 구독을 자동으로 정리(Dispose) 해준다는 점입니다.
            // 이를 통해 메모리 누수(memory leak)를 완벽하게 방지할 수 있습니다. 🧹
            this.WhenActivated(d =>
            {
                // this.OneWayBind(...): 데이터를 한쪽 방향으로 연결(바인딩)하는 ReactiveUI의 메서드입니다.
                // 🔗 데이터는 항상 ViewModel에서 View로 흐릅니다.
                // .DisposeWith(d): 각 바인딩이 WhenActivated의 생명주기에 맞춰 자동으로 해제되도록 등록하는 부분입니다.

                // ViewModel의 ListViewModel 속성을 (vm => vm.ListViewModel)
                // MainWindow의 nodeList라는 UI 컨트롤의 ViewModel 속성에 (v => v.nodeList.ViewModel) 연결합니다.
                // (nodeList는 XAML 파일에 정의된 노드 목록을 보여주는 커스텀 컨트롤일 것입니다.)
                this.OneWayBind(ViewModel, vm => vm.ListViewModel, v => v.nodeList.ViewModel).DisposeWith(d);

                // ViewModel의 NetworkViewModel 속성을 MainWindow의 viewHost라는 UI 컨트롤의 ViewModel 속성에 연결합니다.
                // (viewHost는 노드 네트워크 편집기 화면을 보여주는 컨트롤일 것입니다.)
                this.OneWayBind(ViewModel, vm => vm.NetworkViewModel, v => v.viewHost.ViewModel).DisposeWith(d);

                // ViewModel의 ValueLabel 속성(아마도 문자열)을
                // MainWindow의 valueLabel이라는 UI 라벨의 Content 속성(화면에 표시될 텍스트)에 연결합니다.
                this.OneWayBind(ViewModel, vm => vm.ValueLabel, v => v.valueLabel.Content).DisposeWith(d);
            });
        }
    }
}
