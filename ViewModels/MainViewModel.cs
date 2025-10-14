using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using NodeCalculatorApp.ViewModels.Nodes;
using NodeNetwork.Toolkit;
using NodeNetwork.Toolkit.NodeList;
using NodeNetwork.ViewModels;
using NodeNetwork;
using ReactiveUI;
using System.Reactive.Linq;

namespace NodeCalculatorApp.ViewModels
{
    
    public class MainViewModel : ReactiveObject
    {
        // 이 클래스가 프로그램에서 맨 처음 사용될 때 단 한 번만 실행되는 정적 생성자
        static MainViewModel()
        {
            
            Splat.Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<MainViewModel>));
        }

        public NodeListViewModel ListViewModel { get; } = new NodeListViewModel();
        public NetworkViewModel NetworkViewModel { get; } = new NetworkViewModel();

        private string _valueLabel;
        public string ValueLabel
        {
            get => _valueLabel;
            set => this.RaiseAndSetIfChanged(ref _valueLabel, value);
        }

        public MainViewModel()
        {
            OutputNodeViewModel output = new OutputNodeViewModel();
            NetworkViewModel.Nodes.Add(output);

            NetworkViewModel.Validator = network =>
            { 
                bool containsLoops = GraphAlgorithms.FindLoops(network).Any();
                if(containsLoops)
                {
                    return new NetworkValidationResult(false, false, new ErrorMessageViewModel("Network contains loops"));
                }

                return new NetworkValidationResult(true, false, null);
            };

            output.ResultInput.ValueChanged.Select(v => (NetworkViewModel.LatestValidation?.IsValid ?? true) ? v.ToString() : "Error")
                .BindTo(this, vm => vm.ValueLabel);
        }
    }
}
