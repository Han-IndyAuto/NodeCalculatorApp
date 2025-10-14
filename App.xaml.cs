using NodeNetwork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NodeCalculatorApp
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //NodeNetwork 뷰가 해당 뷰 모델과 연결
            NNViewRegistrar.RegisterSplat();
        }

    }
}
