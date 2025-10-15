using DynamicData;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeCalculatorApp.ViewModels.Nodes
{
    public class ConstantNodeViewModel : NodeViewModel
    {
        static ConstantNodeViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<ConstantNodeViewModel>));
        }

        public IntegerValueEditorViewModel ValueEditor { get; } = new IntegerValueEditorViewModel();

        public ValueNodeOutputViewModel<int?> Output { get; }

        public ConstantNodeViewModel()
        {
            this.Name = "constant";

            Output = new ValueNodeOutputViewModel<int?>
            {
                Name = "Value",
                Editor = ValueEditor,
                Value = this.WhenAnyValue(vm => vm.ValueEditor.Value)
            };
            this.Outputs.Add(Output);
        }
    }
}
