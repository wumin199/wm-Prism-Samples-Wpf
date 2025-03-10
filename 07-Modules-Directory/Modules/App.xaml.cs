﻿using Modules.Views;
using Prism.Modularity;
using Prism.Unity;
using Prism.Ioc;
using System.Windows;
using System.IO;

namespace Modules
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            // 有问题，先不测试
            var modulePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ModuleA");
            return new DirectoryModuleCatalog() { ModulePath = modulePath };
        }
    }
}
